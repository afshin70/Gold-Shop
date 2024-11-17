using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AccessLevelModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AccessLevelViewModels;
using Gold.Domain.Entities;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Gold.ApplicationService.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;

        public UserService(IUnitOfWork unitOfWork, IOptions<FilePathAddress> filePathAddressOptions, ILogManager logManager, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
        }


        public async Task<CommandResult<List<SelectListItem>>> GetAdminMenusAsSelectListByUserTypeAsync(byte selected = 0, CancellationToken cancellationToken = default)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {

                var adminMenus = await _unitOfWork.AdminMenuRepository.GetAllAsIQueryable().Data.ToListAsync(cancellationToken);
                if (adminMenus is null)
                {
                    return CommandResult<List<SelectListItem>>.Failure(string.Format(UserMessages.NotFound, Captions.AdminMenus), list);
                }

                list = adminMenus.Select(x => new SelectListItem
                {
                    Selected = (x.Id == selected),
                    Text = x.Title,
                    Value = x.Id.ToString()
                }).ToList();
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetUsersAsSelectListByUserTypeAsync(int selected = 0, UserType userType = default, CancellationToken cancellationToken = default)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {

                var users = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data.Where(x => x.UserType == userType).ToListAsync(cancellationToken);
                if (users is null)
                {
                    return CommandResult<List<SelectListItem>>.Failure(string.Format(UserMessages.NotFound, Captions.User), list);
                }

                list = users.Select(x => new SelectListItem
                {
                    Selected = (x.Id == selected),
                    Text = x.UserName,
                    Value = x.Id.ToString()
                }).ToList();
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }
        public async Task<CommandResult<List<SelectListItem>>> GetUsersFullNameAsSelectListByUserTypeAsync(int selected = 0, UserType userType = default, CancellationToken cancellationToken = default)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {

                var users = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data.Where(x => x.UserType == userType).ToListAsync(cancellationToken);
                if (users is null)
                    return CommandResult<List<SelectListItem>>.Failure(string.Format(UserMessages.NotFound, Captions.User), list);
                if (userType==UserType.Admin)
                    users = users.Where(x => x.UserName == "siteAdmin").ToList();
                
                list = users.Select(x => new SelectListItem
                {
                    Selected = (x.Id == selected),
                    Text =x.UserType==UserType.Seller? $"{x.FullName} ({Captions.Seller})":x.UserType==UserType.Manager? $"{x.FullName} ({Captions.Manager})": $"{x.FullName} ({Captions.Admin})",
                    Value = x.Id.ToString()
                }).ToList();
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }

        public CommandResult<IQueryable<AccessLevelModel>> GetAccessLevelListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.PermissionRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    IQueryable<AccessLevelModel> finallQuery = result.Data.Select(x => new AccessLevelModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                    }).AsQueryable();

                    return CommandResult<IQueryable<AccessLevelModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<AccessLevelModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<AccessLevelModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<IQueryable<AccessLevelReportModel>> GetAccessLevelReportListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.PermissionRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    IQueryable<AccessLevelReportModel> finallQuery = result.Data
                        .Include(x => x.PermissionAccesses)
                        .ThenInclude(x => x.AdminMenu)
                        .Select(x => new AccessLevelReportModel
                        {
                            Id = x.Id,
                            Title = x.Title,
                            PermissionsTitle = x.PermissionAccesses.Select(x => x.AdminMenu.Title).ToList()
                        }).AsQueryable();

                    return CommandResult<IQueryable<AccessLevelReportModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<AccessLevelReportModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<AccessLevelReportModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        //public async Task<CommandResult<byte>> SendResetPasswordCodeSmsToUserAsync(string username)
        //{
        //    //get user valid reset code for send by sms to user mobile
        //    if (true)
        //    {
        //        //username is valid 
        //        //then => get username reset pass code
        //        //send code by sms


        //        return new CommandResult<byte>
        //        {
        //            Message = "sms is sended to user",
        //            //HasError = false,
        //            IsSuccess = true,
        //            Data = 120 //=>120 second timer for input reset code(reset code expire time)
        //        };
        //    }
        //    else
        //    {
        //        //invalid username
        //        return new CommandResult<byte>
        //        {
        //            Message = "user not valid",
        //            //HasError = false,
        //            IsSuccess = false
        //        };
        //    }
        //}
        public async Task<CommandResult> VerifyResetPasswordCode(string username, string code)
        {
            //check is valid code and username
            var result = await _unitOfWork.UserRepository.IsValidResetPasswordCodeAsync(username, code);
            return result;
        }
        public async Task<CommandResult> ChangePasswordAsync(string username, string newPassword, CancellationToken cancellationToken)
        {
            try
            {
                //validate username =>if valid=> return user salt key for hash new password 
                var userResult = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username, cancellationToken);
                if (userResult.IsSuccess)
                {
                    userResult.Data.PasswordHash = Encryptor.Encrypt(newPassword, userResult.Data.PasswordSalt);
                    userResult.Data.LastPasswordChangeDate = DateTime.Now;
                    userResult.Data.SecurityStamp = Guid.NewGuid();

                    _unitOfWork.UserRepository.Update(userResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.ThePasswordIsChanged);
                    else
                        return CommandResult.Failure(updateResult.Message);
                }
                else
                {
                    return CommandResult.Failure(userResult.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<string>> GetUserNameByIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.UserRepository.GetUserNameByIdAsync(userId, cancellationToken);
                if (result.IsSuccess)
                    return CommandResult<string>.Success(result.Message, result.Data);
                else
                    return CommandResult<string>.Failure(result.Message, result.Data);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<List<int>>> GetManagerPermissionsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            List<int> managerPermissions = new List<int>();
            try
            {
                var permissions = await _unitOfWork.ManagerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Permission).ThenInclude(x => x.PermissionAccesses)
                    .FirstOrDefaultAsync(cancellationToken);

                if (permissions is not null)
                {
                    if (permissions.Permission is not null)
                        if (permissions.Permission.PermissionAccesses is not null)
                            foreach (var item in permissions.Permission.PermissionAccesses)
                            {
                                managerPermissions.Add(item.AdminMenuId);
                            }
                    return CommandResult<List<int>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, managerPermissions);
                }
                return CommandResult<List<int>>.Failure(string.Format(UserMessages.NotFound, string.Empty), managerPermissions);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<int>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, managerPermissions);
            }
        }

        public async Task<CommandResult<CreateOrEditAccessLevelViewModel>> GetAccessLevelForEditAsync(int permissionId, CancellationToken cancellationToken)
        {
            try
            {
                CreateOrEditAccessLevelViewModel? model = await _unitOfWork.PermissionRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == permissionId)
                    .Include(x => x.PermissionAccesses)
                    .Select(x => new CreateOrEditAccessLevelViewModel
                    {
                        AccessLevelId = x.Id,
                        Title = x.Title,
                        AdminMenuIds = x.PermissionAccesses.Select(x => x.AdminMenuId).ToList(),
                    }).FirstOrDefaultAsync(cancellationToken);

                // if (model is null)
                //    return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.AccessLevel), model);
                if (model is null)
                    model = new CreateOrEditAccessLevelViewModel() { AccessLevelId = 0, AdminMenuIds = new List<byte>() };
                model.AdminMenuGroups = await _unitOfWork.AdminMenuGroupsRepository.GetAllAsIQueryable().Data
                    .Include(x => x.AdminMenus)
                    .OrderBy(x=>x.OrderNo)
                    .Select(x => new AdminMenuGroupsModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        AdminMenus = x.AdminMenus.OrderBy(x=>x.OrderNo).Select(x => new AdminMenusModel { Id = x.Id, Title = x.Title }).ToList(),
                    }).ToListAsync(cancellationToken);
                return CommandResult<CreateOrEditAccessLevelViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new CreateOrEditAccessLevelViewModel());
            }
        }

        public async Task<CommandResult<CreateOrEditAccessLevelViewModel>> CreateOrEditAccessLevelAsync(CreateOrEditAccessLevelViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (model.AdminMenuIds is null)
                {
                    return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(UserMessages.SelectingAtLeastOneAccessLevelItemRequired, model);
                }
                var selectedAdminMenuListResult = await _unitOfWork.AdminMenuRepository.GetAllByIdsAsync(model.AdminMenuIds, cancellationToken);
                if (model.AccessLevelId <= 0)
                {
                    //add
                    bool isDuplicateTitle = await _unitOfWork.PermissionRepository.GetAllAsIQueryable().Data.AnyAsync(x => x.Title == model.Title, cancellationToken);
                    if (isDuplicateTitle)
                        return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);

                    var item = new Permission
                    {
                        Title = model.Title,
                    };
                    // await _unitOfWork.PermissionRepository.InsertAsync(item, cancellationToken);

                    if (selectedAdminMenuListResult.IsSuccess)
                    {
                        if (selectedAdminMenuListResult.Data is not null)
                        {
                            item.PermissionAccesses = selectedAdminMenuListResult.Data.Select(x => new PermissionAccess
                            {
                                PermissionId = item.Id,
                                AdminMenuId = x.Id,
                            }).ToList();
                        }
                    }
                    await _unitOfWork.AddAsync(item, cancellationToken);
                    await ResetManagersSecurityStampAsync(item.Id, cancellationToken);
                }
                else
                {
                    //edit
                    var item = await _unitOfWork.PermissionRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Id == model.AccessLevelId)
                        .Include(x => x.PermissionAccesses)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (item is null)
                        return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.AccessLevel), model);
                    if (model.Title != item.Title)
                    {
                        bool isDuplicateTitle = await _unitOfWork.PermissionRepository.GetAllAsIQueryable().Data.AnyAsync(x => x.Title == model.Title, cancellationToken);
                        if (isDuplicateTitle)
                            return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);
                    }

                    if (item.PermissionAccesses.Any())
                        _unitOfWork.PermissionAccessRepository.DeleteRange(item.PermissionAccesses.ToList());

                    if (selectedAdminMenuListResult.IsSuccess)
                    {
                        if (selectedAdminMenuListResult.Data is not null)
                        {
                            List<PermissionAccess> permissionAccesses = selectedAdminMenuListResult.Data.Select(x => new PermissionAccess
                            {
                                PermissionId = item.Id,
                                AdminMenuId = x.Id,
                            }).ToList();
                            await _unitOfWork.PermissionAccessRepository.InsertRangeAsync(permissionAccesses, cancellationToken);
                        }
                    }
                    if (model.Title != item.Title)
                    {
                        item.Title = model.Title;
                        _unitOfWork.PermissionRepository.Update(item);
                    }
                    await ResetManagersSecurityStampAsync(item.Id, cancellationToken);
                    //resolve login for current user if reset security stamp

                }

                var saveChangeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (saveChangeResult.IsSuccess)
                {
                    return CommandResult<CreateOrEditAccessLevelViewModel>.Success(saveChangeResult.Message, model);
                }
                else
                    return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(saveChangeResult.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditAccessLevelViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        private async Task ResetManagersSecurityStampAsync(int permissionId, CancellationToken cancellationToken = default)
        {
            //update managers security stamp
            var managers = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                .Include(x => x.Manager)
                .Where(x => x.UserType == UserType.Manager & x.Manager.PermissionId == permissionId)
                .ToListAsync(cancellationToken);
            if (managers != null)
            {
                managers.ForEach(x => x.SecurityStamp = Guid.NewGuid());
                await _unitOfWork.UserRepository.UpdateRange(managers);
            }

        }

        public async Task<CommandResult<string>> RemoveAccessLevelAsync(byte id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _unitOfWork.PermissionRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == id)
                    .Include(x => x.PermissionAccesses)
                    .FirstOrDefaultAsync(cancellationToken);
                if (item is null)
                {
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.AccessLevel), string.Empty);
                }
                else
                {
                    if (item.PermissionAccesses.Any())
                        _unitOfWork.PermissionAccessRepository.DeleteRange(item.PermissionAccesses.ToList());
                    _unitOfWork.PermissionRepository.Delete(item);

                    await ResetManagersSecurityStampAsync(item.Id, cancellationToken);

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, item.Title);
                    else
                        return CommandResult<string>.Failure(result.Message, item.Title);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetAccessLevelAsSelectListItemAsync(int selectedId = 0, CancellationToken cancellationToken = default)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            try
            {
                var result = _unitOfWork.PermissionRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    selectListItems = result.Data.OrderBy(x => x.Title).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Title,
                        Selected = (x.Id == selectedId)
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, selectListItems);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(UserMessages.ErrorInLoadAccessLevels, selectListItems);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, selectListItems);
            }
        }

        public CommandResult<IQueryable<EditInformationRequestModel>> EditInformationRequestListAsQuerable(bool? isActive)
        {
            try
            {
                var result = _unitOfWork.EditInformationRequestRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    if (isActive.HasValue)
                        result.Data = result.Data.Where(x => x.IsActive == isActive.Value);

                    IQueryable<EditInformationRequestModel> finallQuery = result.Data
                        .Include(x => x.Customer).ThenInclude(x => x.User)
                        .OrderByDescending(x => x.RegisterDate)
                        .Select(x => new EditInformationRequestModel
                        {
                            Id = x.Id,
                            FullName = x.Customer.User.FullName,
                            ImageName = x.ImageName,
                            IsActive = x.IsActive,
                            Message = x.Description,
                            NationalCode = x.Customer.NationalCode,
                            PersianRequestDate = $"{x.RegisterDate.GeorgianToPersian(ShowMode.OnlyTime)} - {x.RegisterDate.GeorgianToPersian(ShowMode.OnlyDate)}"
                        }).AsQueryable();
                    return CommandResult<IQueryable<EditInformationRequestModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<EditInformationRequestModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<EditInformationRequestModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult> RemoveEditInformationRequestAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var itemFindeResult = await _unitOfWork.EditInformationRequestRepository.GetByIdAsync(id, cancellationToken);
                if (itemFindeResult.IsSuccess)
                {

                    _unitOfWork.EditInformationRequestRepository.Delete(itemFindeResult.Data);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                    {
                        //remove image
                        _fileService.DeleteFile(itemFindeResult.Data.ImageName, _filePathAddress.EditInformationRequest);

                        return CommandResult.Success(UserMessages.DataDeletedSuccessfully);
                    }
                    else
                        return CommandResult.Failure(result.Message);
                }
                else
                {
                    return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.EditInformationRequest));
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult> ArchiveEditInformationRequestAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var itemFindeResult = await _unitOfWork.EditInformationRequestRepository.GetByIdAsync(id, cancellationToken);
                if (itemFindeResult.IsSuccess)
                {
                    itemFindeResult.Data.IsActive = false;
                    _unitOfWork.EditInformationRequestRepository.Update(itemFindeResult.Data);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult.Success(UserMessages.DataSavedSuccessfully);
                    else
                        return CommandResult.Failure(result.Message);
                }
                else
                {
                    return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.EditInformationRequest));
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult> LogUserActivityAsync(LogUserActivityModel model, CancellationToken cancellationToken = default, bool saveLog = true)
        {
            try
            {
                var activity = new AdminActivity
                {
                    ActivityType = model.ActivityType,
                    Date = DateTime.Now,
                    AdminMenuID = model.AdminMenuId,
                    Description = model.DescriptionPattern,
                    UserId = model.UserId,
                };
                if (model.Parameters is not null)
                {
                    foreach (var param in model.Parameters)
                    {
                        string paramIndex = "{" + param.Key + "}";
                        model.DescriptionPattern = model.DescriptionPattern.Replace(paramIndex, param.Value);
                    }
                    activity.Description = model.DescriptionPattern;
                }

                var addResult = await _unitOfWork.AdminActivityRepository.InsertAsync(activity, cancellationToken);
                if (saveLog)
                {
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult.Success(result.Message);
                    else
                        return CommandResult.Failure(result.Message);
                }
                else
                {
                    if (addResult.IsSuccess)
                        return CommandResult.Success(addResult.Message);
                    else
                        return CommandResult.Failure(addResult.Message);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<bool>> IsExistUserByUserNameAsync(string? nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                if (await _unitOfWork.UserRepository.GetAllAsIQueryable().Data.AnyAsync(x=>x.UserName==nationalCode,cancellationToken))
                    return CommandResult<bool>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, true);
                else
                    return CommandResult<bool>.Success(string.Format(OperationResultMessage.NotFound,Captions.User), false);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware,false);
            }
        }
        //public async Task<CommandResu lt<string>> SendResetPasswordToUserEmailAsync(string username)
        //{
        //    //get user info
        //    //get html template from settings for email body
        //    //send mail
        //    var commandResult = await _unitOfWork.UserRepository.GetUserResetCodeAndEmailInfoAsync(username);

        //    if (commandResult.IsSuccess)
        //    {
        //        if (await EmailService.SendMailAsync(Captions.ResetPassword, commandResult.Data.ResetPsswordCode, commandResult.Data.UserMail))
        //        {
        //            return new CommandResult<string>
        //            {
        //                IsSuccess = true,
        //                HasError = false,
        //                CommandMessage = OperationResultMessage.ResetCodeIsSended
        //            };
        //        }
        //        else
        //        {
        //            return new CommandResult<string>
        //            {
        //                IsSuccess = false,
        //                HasError = false,
        //                CommandMessage = OperationResultMessage.OperationIsFailure
        //            };
        //        }
        //    }
        //    else
        //    {
        //        return new CommandResult<string>
        //        {
        //            IsSuccess = false,
        //            HasError = false,
        //            CommandMessage = UserMessages.UserNotFoundByUsername, username)
        //        };
        //    }

        //}


    }
}
