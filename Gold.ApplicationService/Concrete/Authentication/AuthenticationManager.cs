using FluentValidation;
using FluentValidation.Results;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AuthenticationModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels.RegisterCustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.Domain.Contract.DTOs.UserModels;
using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Infrastracture.Configurations.JWTSystem;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.ConstData;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.JWT;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Security;
using Gold.SharedKernel.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Gold.ApplicationService.Concrete.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly ILogManager _logManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ISMSSender _smsSender;

        public AuthenticationManager(IUnitOfWork unitOfWork, ILogManager logManager, IUserService userService, ISMSSender smsSender)
        {
            _logManager = logManager;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _smsSender = smsSender;
        }

        private async Task<CommandResult<User>> UnlockAccountAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                if (user.LockDate.Value.AddMinutes(5) <= DateTime.Now)
                {
                    user.LockDate = null;
                    user.IsLocked = false;
                    user.WrongPasswordCount = 0;
                    _unitOfWork.UserRepository.Update(user);
                    var commitChangesResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (commitChangesResult.IsSuccess)
                        return CommandResult<User>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, user);
                    else
                        return CommandResult<User>.Failure(commitChangesResult.Message, user);
                }
                else
                {
                    int time = Convert.ToInt32(Math.Ceiling((user.LockDate.Value.AddMinutes(5) - DateTime.Now).TotalSeconds / 60));
                    return CommandResult<User>.Failure(string.Format(UserMessages.TryToLoginAfterXMin, time.ToString()), user);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<User>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, user);
            }
        }

        public async Task<CommandResult<UserType>> LoginUserAsync(HttpContext httpContext, LoginViewModel model, CancellationToken cancellationToken)
        {
            try
            {

                CommandResult<User> userResult = await _unitOfWork.UserRepository.GetUserByUsernameAsync(model.UserName.ToEnglishNumbers(), cancellationToken);
                if (userResult.IsSuccess)
                {
                    if (userResult.Data.LockDate.HasValue)
                    {
                        var unlockResult = await UnlockAccountAsync(userResult.Data, cancellationToken);
                        if (unlockResult.IsSuccess)
                            userResult.Data = unlockResult.Data;
                        else
                            return CommandResult<UserType>.Failure(unlockResult.Message, unlockResult.Data.UserType);
                    }


                    if (!userResult.Data.IsLocked)
                    {
                        if (await IsValidUserPasswordAsync(userResult.Data.PasswordHash, userResult.Data.PasswordSalt, model.Password.ToEnglishNumbers()))
                        {
                            //valid password
                            if (userResult.Data.IsActive)
                            {
                                //if seller gallery is not active, user account is blocked!
                                if (userResult.Data.UserType == UserType.Seller)
                                {
                                    var sellerValidationResult = await ValidateSellerLoginRouls(userResult.Data.Id, cancellationToken);
                                    if (!sellerValidationResult.IsSuccess)
                                        return CommandResult<UserType>.Failure(UserMessages.YourAccountIsBlocked, userResult.Data.UserType);
                                }

                                #region Login
                                List<Claim> claims = new List<Claim>()
                                                    {
                                                        new Claim(GoldClaimType.UserId,userResult.Data.Id.ToString()),
                                                        new Claim(GoldClaimType.Name,userResult.Data.FullName),
                                                        new Claim(GoldClaimType.Mobile,userResult.Data.Mobile),
                                                        new Claim(GoldClaimType.UserName,userResult.Data.UserName),
                                                        new Claim(GoldClaimType.UserType,userResult.Data.UserType.ToString()),
                                                        new Claim(GoldClaimType.SecurityStamp,userResult.Data.SecurityStamp.ToString()),
                                                    };

                                //fill manager user type permissions
                                if (userResult.Data.UserType == UserType.Manager)
                                {
                                    var permissionsResult = await _userService.GetManagerPermissionsByUserIdAsync(userResult.Data.Id, cancellationToken);
                                    if (permissionsResult.IsSuccess)
                                    {
                                        foreach (var item in permissionsResult.Data)
                                        {
                                            Claim permissionClaim = new Claim(GoldClaimType.ManagerPermission, item.ToString());
                                            claims.Add(permissionClaim);
                                        }
                                    }
                                }
                                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                                AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                                {
                                    IsPersistent = model.RememberMe,
                                };
                                await httpContext.SignInAsync(claimsPrincipal, authenticationProperties);
                                #endregion


                                //update last login date and reset Wrong Password Count  of user 
                                userResult.Data.LastLoginDate = DateTime.Now;
                                userResult.Data.WrongPasswordCount = 0;
                                _unitOfWork.UserRepository.Update(userResult.Data);
                                var commitChangesResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                                if (commitChangesResult.IsSuccess)
                                    return CommandResult<UserType>.Success(UserMessages.SuccessfullyLogin, userResult.Data.UserType);

                                await httpContext.SignOutAsync();
                                return CommandResult<UserType>.Failure(UserMessages.FailureLogin, userResult.Data.UserType);
                            }
                            else
                            {
                                //user is not active
                                return CommandResult<UserType>.Failure(UserMessages.UserNotActive, userResult.Data.UserType);
                            }
                        }
                        else
                        {
                            //invalid password
                            if (userResult.Data.WrongPasswordCount >= 5)
                            {
                                //lock account if WrongPasswordCount is more than 5  
                                userResult.Data.IsLocked = true;
                                userResult.Data.LockDate = DateTime.Now;
                            }
                            else
                            {
                                //update user wrong password count
                                userResult.Data.WrongPasswordCount += 1;
                            }
                            var updateWrongPasswordCountResult = _unitOfWork.UserRepository.Update(userResult.Data);
                            var commitChangesResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                            return CommandResult<UserType>.Failure(UserMessages.TheUsernameAndPasswordIsIncorrect, userResult.Data.UserType);
                        }
                    }
                    else
                    {
                        //user account is locked
                        return CommandResult<UserType>.Failure(UserMessages.YourAccountIsBlocked, userResult.Data.UserType);
                    }
                }
                else
                {
                    //user not founded by username
                    return CommandResult<UserType>.Failure(UserMessages.TheUsernameAndPasswordIsIncorrect, UserType.Customer);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<UserType>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, UserType.Customer);
            }
        }

        private async Task<CommandResult> ValidateSellerLoginRouls(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var seler = await _unitOfWork.SellerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Gallery)
                    .FirstOrDefaultAsync(cancellationToken);
                if (seler is null)
                {
                    return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.Seller));
                }
                else
                {
                    if (seler.Gallery is null)
                    {
                        return CommandResult.Failure(string.Format(UserMessages.NotFound, $"{Captions.Seller} - {Captions.Gallery}"));
                    }
                    else
                    {
                        if (seler.Gallery.IsActive)
                            return CommandResult.Success(string.Format(UserMessages.IsActive, Captions.Gallery));
                        else
                            return CommandResult.Failure(string.Format(UserMessages.IsNotActive, Captions.Gallery));
                    }
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<UserType>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, UserType.Customer);
            }
        }

        public async Task<CommandResult> LogOutUserAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            try
            {
                await httpContext.SignOutAsync();
                return CommandResult.Success(UserMessages.SuccessfullyLogOut);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.OperationIsFailure);
            }
        }

        public async Task<bool> IsValidUserPasswordAsync(string passwordHash, string passwordSaltKey, string password)
        {
            return await Task.FromResult(passwordHash == Encryptor.Encrypt(password, passwordSaltKey));
        }



        public async Task<CommandResult<VerifyResetPasswordCodeViewModel>> ProcessForgetPasswordAsync(ForgetPasswordViewModel model, CancellationToken cancellationToken)
        {
            VerifyResetPasswordCodeViewModel resultModel = new();
            try
            {
                var user = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .FirstOrDefaultAsync(x => x.UserName == model.UserName.ToEnglishNumbers() & x.Mobile == model.Mobile.ToEnglishNumbers() & x.IsActive);
                if (user is null)
                {
                    return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(UserMessages.TheUsernameOrMobileIsIncorrect, resultModel);
                }
                else
                {
                    var resetSmsTemps = await _unitOfWork.SendSmsTempRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Type == SendSmsType.ResetPassword)
                        .ToListAsync(cancellationToken);
                    //remove expired records
                    if (resetSmsTemps.Any(x => x.ExpireDate <= DateTime.Now))
                    {
                        var expiredRecords = resetSmsTemps.Where(x => x.ExpireDate < DateTime.Now).ToList();
                        _unitOfWork.SendSmsTempRepository.DeleteRange(expiredRecords);
                        expiredRecords.ForEach(item => resetSmsTemps.Remove(item));
                    }
                    //generate verify otp code and send sms
                    string otpCode = GeneratorTools.GenerateOTP(6);
                    bool isSendSms = false;
                    SendSmsTemp? sendSmsTemp = resetSmsTemps.FirstOrDefault(x => x.Mobile == user.Mobile.ToEnglishNumbers() & x.UserName == user.UserName.ToEnglishNumbers());
                    CommandResult<int> result = CommandResult<int>.Success(string.Empty, 0);
                    if (sendSmsTemp is not null)
                    {
                        //update sms record
                        if (sendSmsTemp.SendDate.AddMinutes(1) <= DateTime.Now)
                        {
                            //regenerate otp
                            sendSmsTemp.SendDate = DateTime.Now;
                            _unitOfWork.SendSmsTempRepository.Update(sendSmsTemp);
                            isSendSms = true;
                            otpCode = sendSmsTemp.Code;
                            result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        }
                    }
                    else
                    {
                        //add sms record
                        sendSmsTemp = new SendSmsTemp
                        {
                            UserName = user.UserName,
                            Mobile = user.Mobile,
                            Code = otpCode,
                            Token = Guid.NewGuid(),
                            ExpireDate = DateTime.Now.AddMinutes(10),
                            SendDate = DateTime.Now,
                            Type = SendSmsType.ResetPassword
                        };
                        await _unitOfWork.SendSmsTempRepository.InsertAsync(sendSmsTemp, cancellationToken);
                        isSendSms = true;
                        result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    }


                    if (result.IsSuccess)
                    {
                        //send sms to user
                        //await  _smsSender.SendAsync(user.Mobile, otpCode, cancellationToken);

                        if (isSendSms)
                        {
                            Dictionary<string, string> smsParameters = new Dictionary<string, string>();
                            smsParameters.Add("CODE", otpCode);
                            await _smsSender.SendByVerifyAsync(user.Mobile, 857226, smsParameters, cancellationToken);
                        }

                        resultModel.UserName = sendSmsTemp.UserName;
                        resultModel.Mobile = sendSmsTemp.Mobile;
                        resultModel.Timer = sendSmsTemp.SendDate.AddMinutes(1).TimeOfDay - DateTime.Now.TimeOfDay;
                        resultModel.Token = sendSmsTemp.Token;
                        return CommandResult<VerifyResetPasswordCodeViewModel>.Success(UserMessages.ResetPasswordCodeIsSendedToMobileNumber, resultModel);
                    }
                    else
                        return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(result.Message, resultModel);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(OperationResultMessage.OperationIsFailure, resultModel);
            }
        }
        
        public async Task<CommandResult<RegisterCustomerVerifyCodeViewModel>> ProcessRegisterCustomerOTPCodeAsync(RegisterCustomerBaseInfoViewModel model, CancellationToken cancellationToken)
        {
            RegisterCustomerVerifyCodeViewModel resultModel = new();
            try
            {
                var resetSmsTemps = await _unitOfWork.SendSmsTempRepository.GetAllAsIQueryable().Data
                          .Where(x => x.Type == SendSmsType.RegisterCustomer)
                          .ToListAsync(cancellationToken);
                //remove expired records
                if (resetSmsTemps.Any(x => x.ExpireDate <= DateTime.Now))
                {
                    var expiredRecords = resetSmsTemps.Where(x => x.ExpireDate < DateTime.Now).ToList();
                    _unitOfWork.SendSmsTempRepository.DeleteRange(expiredRecords);
                    expiredRecords.ForEach(item => resetSmsTemps.Remove(item));
                }
                //generate verify otp code and send sms
                string otpCode = GeneratorTools.GenerateOTP(6);
                bool isSendSms = false;
                SendSmsTemp? sendSmsTemp = resetSmsTemps.FirstOrDefault(x => x.Mobile == model.Mobile.ToEnglishNumbers() & x.UserName == model.NationalCode.ToEnglishNumbers());
                CommandResult<int> result = CommandResult<int>.Success(string.Empty, 0);
                if (sendSmsTemp is not null)
                {
                    //update sms record
                    if (sendSmsTemp.SendDate.AddMinutes(1) <= DateTime.Now)
                    {
                        //regenerate otp
                        sendSmsTemp.SendDate = DateTime.Now;
                        _unitOfWork.SendSmsTempRepository.Update(sendSmsTemp);
                        isSendSms = true;
                        otpCode = sendSmsTemp.Code;
                        result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    }
                }
                else
                {
                    //add sms record
                    sendSmsTemp = new SendSmsTemp
                    {
                        UserName = model.NationalCode,
                        Mobile = model.Mobile,
                        Code = otpCode,
                        Token = Guid.NewGuid(),
                        ExpireDate = DateTime.Now.AddMinutes(10),
                        SendDate = DateTime.Now,
                        Type = SendSmsType.RegisterCustomer
                    };
                    await _unitOfWork.SendSmsTempRepository.InsertAsync(sendSmsTemp, cancellationToken);
                    isSendSms = true;
                    result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                }


                if (result.IsSuccess)
                {
                    //send sms to user
                    //await  _smsSender.SendAsync(user.Mobile, otpCode, cancellationToken);

                    if (isSendSms)
                    {
                        Dictionary<string, string> smsParameters = new Dictionary<string, string>();
                        smsParameters.Add("CODE", otpCode);
                        await _smsSender.SendByVerifyAsync(model.Mobile, 661849, smsParameters, cancellationToken);
                    }

                    resultModel.NationalityType = model.NationalityType;
                    resultModel.NationalCode = sendSmsTemp.UserName;
                    resultModel.Mobile = sendSmsTemp.Mobile;
                    resultModel.Timer = sendSmsTemp.SendDate.AddMinutes(1).TimeOfDay - DateTime.Now.TimeOfDay;
                    resultModel.Token = sendSmsTemp.Token;
                    return CommandResult<RegisterCustomerVerifyCodeViewModel>.Success(UserMessages.RegisterCustomerCodeIsSendedToMobileNumber, resultModel);
                }
                else
                    return CommandResult<RegisterCustomerVerifyCodeViewModel>.Failure(result.Message, resultModel);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<RegisterCustomerVerifyCodeViewModel>.Failure(OperationResultMessage.OperationIsFailure, resultModel);
            }
        }

        public async Task<CommandResult<RegisterCustomerVerifyCodeViewModel>> VerifyRegisterCustomerOTPCodeAsync(RegisterCustomerVerifyCodeViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                model.NationalCode ??= string.Empty;
                model.Mobile ??= string.Empty;
                model.VerifyCode ??= string.Empty;

                var smsTemp = await _unitOfWork.SendSmsTempRepository.GetAllAsIQueryable().Data
                     .FirstOrDefaultAsync(x => x.UserName == model.NationalCode.ToEnglishNumbers() & x.Token == model.Token & x.Mobile == model.Mobile.ToEnglishNumbers() & x.Type == SendSmsType.RegisterCustomer);
                if (smsTemp is not null)
                {
                    if (smsTemp.ExpireDate < DateTime.Now)
                        return CommandResult<RegisterCustomerVerifyCodeViewModel>.Failure(UserMessages.TheOtpCodeIsExpired, model);
                    else
                    {
                        if (smsTemp.Code == model.VerifyCode.ToEnglishNumbers())
                        {
                            model.Token = smsTemp.Token;
                            //_unitOfWork.SendSmsTempRepository.Delete(smsTemp);
                            //_unitOfWork.CommitChangesAsync(cancellationToken);
                            return CommandResult<RegisterCustomerVerifyCodeViewModel>.Success(UserMessages.TheOtpCodeIsVerified, model);
                        }
                        else
                        {
                            return CommandResult<RegisterCustomerVerifyCodeViewModel>.Failure(UserMessages.TheOtpCodeIsInvalid, model);
                        }
                    }
                }
                else
                    return CommandResult<RegisterCustomerVerifyCodeViewModel>.Failure(UserMessages.TheUsernameOrMobileIsIncorrect, model);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<RegisterCustomerVerifyCodeViewModel>.Failure(OperationResultMessage.OperationIsFailure, model);
            }
        }
        
        public async Task<CommandResult<VerifyResetPasswordCodeViewModel>> VerifyResetPasswordOTPCodeAsync(VerifyResetPasswordCodeViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                model.UserName ??= string.Empty;
                model.Mobile ??= string.Empty;
                model.VerifyCode ??= string.Empty;

                var smsTemp = await _unitOfWork.SendSmsTempRepository.GetAllAsIQueryable().Data
                     .FirstOrDefaultAsync(x => x.UserName == model.UserName.ToEnglishNumbers() & x.Token == model.Token & x.Mobile == model.Mobile.ToEnglishNumbers() & x.Type == SendSmsType.ResetPassword);
                if (smsTemp is not null)
                {
                    if (smsTemp.ExpireDate < DateTime.Now)
                        return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(UserMessages.TheOtpCodeIsExpired, model);
                    else
                    {
                        if (smsTemp.Code == model.VerifyCode.ToEnglishNumbers())
                        {
                            model.Token = smsTemp.Token;
							//_unitOfWork.SendSmsTempRepository.Delete(smsTemp);
							//_unitOfWork.CommitChangesAsync(cancellationToken);
							return CommandResult<VerifyResetPasswordCodeViewModel>.Success(UserMessages.TheOtpCodeIsVerified, model);
                        }
                        else
                        {
                            return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(UserMessages.TheOtpCodeIsInvalid, model);
                        }
                    }
                }
                else
                    return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(UserMessages.TheUsernameOrMobileIsIncorrect, model);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(OperationResultMessage.OperationIsFailure, model);
            }
        }

        public async Task<CommandResult> ChangePasswordByTokenAsync(ChangePasswordByTokenViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                model.UserName ??= string.Empty;
                model.Password ??= string.Empty;

                var smsTemp = await _unitOfWork.SendSmsTempRepository.GetAllAsIQueryable().Data
                     .FirstOrDefaultAsync(x => x.UserName == model.UserName.ToEnglishNumbers() & x.Token == model.Token & x.Type == SendSmsType.ResetPassword, cancellationToken);

                if (smsTemp is null)
                    return CommandResult.Failure($"{UserMessages.TheOtpCodeIsInvalid} - {UserMessages.RetryResetPassword}");
                var user = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .FirstOrDefaultAsync(x => x.UserName == model.UserName, cancellationToken);

                if (user is not null)
                {
                    //is valid for change password
                    user.PasswordHash = Encryptor.Encrypt(model.Password.ToEnglishNumbers(), user.PasswordSalt);
                    user.SecurityStamp = Guid.NewGuid();
                    user.LastModifiedDate = DateTime.Now;
                    user.LastPasswordChangeDate = DateTime.Now;
                    user.IsLocked = false;

                    _unitOfWork.SendSmsTempRepository.Delete(smsTemp);
                    _unitOfWork.UserRepository.Update(user);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult.Success(result.Message);
                    else
                        return CommandResult.Failure(result.Message);
                }
                else
                {
                    return CommandResult.Failure(UserMessages.TheUsernameOrMobileIsIncorrect);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.OperationIsFailure);
            }
        }

        public async Task<CommandResult<List<AdminMenuGroupModel>>> GetAdminPanelMenuListAsync(CancellationToken cancellationToken = default)
        {
            List<AdminMenuGroupModel> list = new List<AdminMenuGroupModel>();

            try
            {
                list = await _unitOfWork.AdminMenuGroupsRepository.GetAllAsIQueryable().Data
                    .OrderBy(x => x.OrderNo)
                    .Include(x => x.AdminMenus)
                    .Select(x => new AdminMenuGroupModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        IconName = x.IconName,
                        AdminMenus = x.AdminMenus
                        .OrderBy(c => c.OrderNo)
                        .Select(c => new AdminMenuModel
                        {
                            Id = c.Id,
                            MenuGroupId = x.Id,
                            Title = c.Title,
                            IconName = c.IconName,
                            ActionName = c.ActionName,
                            ControllerName = c.ControllerName
                        }).ToList()
                    }).ToListAsync(cancellationToken);

                return CommandResult<List<AdminMenuGroupModel>>.Failure(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<AdminMenuGroupModel>>.Failure(OperationResultMessage.OperationIsFailure, list);
            }
        }

        public async Task<CommandResult> ChangePasswordAsync(ChangeUserPasswordModel changeUserPasswordModel, CancellationToken cancellationToken)
        {
            try
            {
                var userResult = await _unitOfWork.UserRepository.GetUserByUsernameAsync(changeUserPasswordModel.UserName.ToEnglishNumbers(), cancellationToken);
                if (userResult.IsSuccess)
                {
                    if (userResult.Data.PasswordHash == Encryptor.Encrypt(changeUserPasswordModel.CurrentPassword.ToEnglishNumbers(), userResult.Data.PasswordSalt))
                    {
                        userResult.Data.PasswordHash = Encryptor.Encrypt(changeUserPasswordModel.NewPassword.ToEnglishNumbers(), userResult.Data.PasswordSalt);
                        userResult.Data.SecurityStamp = Guid.NewGuid();

                        _unitOfWork.UserRepository.Update(userResult.Data);
                        var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (result.IsSuccess)
                            return CommandResult.Success(result.Message);
                        else
                            return CommandResult.Failure(result.Message);
                    }
                    else
                    {
                        return CommandResult.Failure(string.Format(ValidationMessages.Invalid, Captions.CurrentPassword));
                    }

                }
                else
                {
                    return CommandResult.Failure(UserMessages.UserNotFound);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.OperationIsFailure);
            }
        }

        public async Task<CommandResult<VerifyResetPasswordCodeViewModel>> ResendVerifyResetPasswordOTPCodeAsync(string? userName, string? mobile, CancellationToken cancellationToken)
        {
            try
            {
                var forgetPasswordModel = new ForgetPasswordViewModel
                {
                    Mobile = mobile ?? string.Empty,
                    UserName = userName ?? string.Empty
                };
                return await ProcessForgetPasswordAsync(forgetPasswordModel, cancellationToken);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<VerifyResetPasswordCodeViewModel>.Failure(OperationResultMessage.OperationIsFailure, new());
            }
        }
        
        public async Task<CommandResult<RegisterCustomerVerifyCodeViewModel>> ResendRegisterCustomerVerifyOTPCodeAsync(NationalityType nationalityType,string? userName, string? mobile, CancellationToken cancellationToken)
        {
            try
            {
                var model = new RegisterCustomerBaseInfoViewModel
                {
                    Mobile = mobile ?? string.Empty,
                    NationalityType=nationalityType,
                    NationalCode = userName ?? string.Empty,
                };
                return await ProcessRegisterCustomerOTPCodeAsync(model, cancellationToken);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<RegisterCustomerVerifyCodeViewModel>.Failure(OperationResultMessage.OperationIsFailure, new());
            }
        }

        public async Task<CommandResult<TimeSpan>> GetResetPasswordSmsTimerAsync(VerifyResetPasswordCodeViewModel model, CancellationToken cancellationToken)
        {
            TimeSpan timeSpan = TimeSpan.Zero;
            try
            {
                model.UserName ??= string.Empty;
                model.Mobile ??= string.Empty;
                var smsTemp = await _unitOfWork.SendSmsTempRepository.GetAllAsIQueryable().Data
                   .FirstOrDefaultAsync(x => x.UserName == model.UserName.ToEnglishNumbers() & x.Mobile == model.Mobile.ToEnglishNumbers() & x.Token == model.Token, cancellationToken);
                if (smsTemp is null)
                {
                    return CommandResult<TimeSpan>.Failure(OperationResultMessage.OperationIsFailure, timeSpan);
                }
                else
                {
                    timeSpan = smsTemp.SendDate.AddMinutes(1).TimeOfDay - DateTime.Now.TimeOfDay;
                    if (timeSpan.TotalMilliseconds < 0)
                    {
                        timeSpan = TimeSpan.Zero;
                    }
                    return CommandResult<TimeSpan>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, timeSpan);
                }

            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<TimeSpan>.Failure(OperationResultMessage.OperationIsFailure, timeSpan);
            }
        }
        
        public async Task<CommandResult<TimeSpan>> GetRegisterCustomerSmsTimerAsync(RegisterCustomerVerifyCodeViewModel model, CancellationToken cancellationToken)
        {
            TimeSpan timeSpan = TimeSpan.Zero;
            try
            {
                model.NationalCode ??= string.Empty;
                model.Mobile ??= string.Empty;
                var smsTemp = await _unitOfWork.SendSmsTempRepository.GetAllAsIQueryable().Data
                   .FirstOrDefaultAsync(x =>x.Type==SendSmsType.RegisterCustomer & x.UserName == model.NationalCode.ToEnglishNumbers() & x.Mobile == model.Mobile.ToEnglishNumbers() & x.Token == model.Token, cancellationToken);
                if (smsTemp is null)
                {
                    return CommandResult<TimeSpan>.Failure(OperationResultMessage.OperationIsFailure, timeSpan);
                }
                else
                {
                    timeSpan = smsTemp.SendDate.AddMinutes(1).TimeOfDay - DateTime.Now.TimeOfDay;
                    if (timeSpan.TotalMilliseconds < 0)
                    {
                        timeSpan = TimeSpan.Zero;
                    }
                    return CommandResult<TimeSpan>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, timeSpan);
                }

            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<TimeSpan>.Failure(OperationResultMessage.OperationIsFailure, timeSpan);
            }
        }

        public async Task<CommandResult> ChangeCustomerPasswordAsync(ChangeUserPasswordViewModel model, string userName, CancellationToken cancellationToken)
        {
            try
            {
                var userResult = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                  .Where(x => x.UserName == userName & x.UserType == UserType.Customer & x.IsActive)
                  .Include(x => x.Customer)
                  .FirstOrDefaultAsync(cancellationToken);
                if (userResult == null)
                    return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.Customer));
                if (userResult is not null)
                {
                    if (userResult.PasswordHash == Encryptor.Encrypt(model.CurrentPassword.ToEnglishNumbers(), userResult.PasswordSalt))
                    {
                        userResult.PasswordHash = Encryptor.Encrypt(model.NewPassword.ToEnglishNumbers(), userResult.PasswordSalt);
                        userResult.SecurityStamp = Guid.NewGuid();

                        _unitOfWork.UserRepository.Update(userResult);
                        var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (result.IsSuccess)
                            return CommandResult.Success(result.Message);
                        else
                            return CommandResult.Failure(result.Message);
                    }
                    else
                    {
                        return CommandResult.Failure(string.Format(ValidationMessages.Invalid, Captions.CurrentPassword));
                    }
                }
                else
                {
                    return CommandResult.Failure(UserMessages.UserNotFound);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.OperationIsFailure);
            }
        }
    }
}
