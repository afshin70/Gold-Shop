using FluentValidation;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerPymentsModel;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SendMessageViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.ApplicationService.Utility.ExtentionMethods;
using Gold.ApplicationService.Utility.StaticData;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Enums;
using Gold.Infrastracture.Repositories;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.DTO.PaginationModels;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Helpers;
using Gold.SharedKernel.Security;
using Gold.SharedKernel.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using System.Transactions;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Document = Gold.Domain.Entities.Document;


namespace Gold.ApplicationService.Imp
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;
        private readonly IProvinceService _provinceService;
        private readonly ISettingService _settingService;
        private readonly IFileService _fileService;
        private readonly IGalleryService _galleryService;
        private readonly ISellerService _sellerService;
        private readonly ISMSSender _smsSender;
        private readonly IUserService _userService;
        private readonly FilePathAddress _filePathAddress;

        public CustomerService(IUnitOfWork unitOfWork,
            ILogManager logManager,
            IProvinceService provinceService,
            ISettingService settingService,
             IFileService fileService,
            IOptions<FilePathAddress> filePathAddressOptions,
            IGalleryService galleryService,
            ISellerService sellerService,
            IUserService userService,
            ISMSSender smsSender)
        {
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _provinceService = provinceService;
            _settingService = settingService;
            _fileService = fileService;
            _galleryService = galleryService;
            _sellerService = sellerService;
            _filePathAddress = filePathAddressOptions.Value;
            _userService = userService;
            _smsSender = smsSender;
        }



        public async Task<CommandResult<CreateOrEditCustomerViewModel>> CreateCustomerAsync(CreateOrEditCustomerViewModel model, CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    DateTime? birthDate = null;
                    if (!string.IsNullOrEmpty(model.BirthDate))
                    {
                        birthDate = DateTimeTools.ParsePersianToGorgian(model.BirthDate);
                        if (birthDate is null)
                            return CommandResult<CreateOrEditCustomerViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.BirthDate), model);
                        if (birthDate >= DateTime.Now)
                            return CommandResult<CreateOrEditCustomerViewModel>.Failure(string.Format(ValidationMessages.NotMoreThan, Captions.BirthDate, Captions.Today), model);
                    }

                    var proviancesResult = await _provinceService.GetAllProvincesSelectListItemAsync(0, cancellationToken);
                    if (proviancesResult.IsSuccess)
                    {
                        model.Proviances = proviancesResult.Data.ToList();
                    }
                    if (model.CityId.HasValue)
                    {
                        var citiesResult = await _provinceService.GetCitiesOfProvinceSelectListItemAsync(model.CityId.Value, cancellationToken);
                        if (citiesResult.IsSuccess)
                        {
                            foreach (var item in citiesResult.Data)
                            {
                                if (item.Value == model.CityId.Value.ToString())
                                {
                                    item.Selected = true;
                                }
                                model.Cities.Add(item);
                            }
                        }
                    }

                    //check uniq username
                    if (_unitOfWork.UserRepository.GetAllAsIQueryable().Data.Any(x => x.UserName == model.NationalCode))
                    {
                        return CommandResult<CreateOrEditCustomerViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.NationalCode), model);
                    }

                    int? birthDateYear = null;
                    byte? birthDateMonth = null;
                    byte? birthDateDay = null;
                    if (!string.IsNullOrEmpty(model.BirthDate))
                    {
                        var array = model.BirthDate.Split('/');
                        if (int.TryParse(array[0].ToEnglishNumbers(), out int year))
                            birthDateYear = year;
                        if (byte.TryParse(array[1].ToEnglishNumbers(), out byte month))
                            birthDateMonth = month;
                        if (byte.TryParse(array[2].ToEnglishNumbers(), out byte day))
                            birthDateDay = day;
                    }
                    string saltKey = Guid.NewGuid().ToString();
                    User user = new()
                    {
                        UserName = model.NationalCode,
                        FullName = model.FullName,
                        PasswordSalt = saltKey,
                        PasswordHash = Encryptor.Encrypt(model.Mobile, saltKey),
                        IsActive = true,
                        Mobile = model.Mobile,
                        IsLocked = false,
                        RegisterDate = DateTime.Now,
                        UserType = UserType.Customer,
                        Customer = new()
                        {
                            Address = model.Address,
                            CityId = model.CityId,
                            FatherName = model.FatherName,
                            NationalCode = model.NationalCode,
                            PostalCode = model.PostalCode,
                            SanaCode = model.SanaCode,
                            JobTitle = model.JobTitle,
                            BirthDate = birthDate,
                            BirthDateYear = birthDateYear,
                            BirthDateMonth = birthDateMonth,
                            BirthDateDay = birthDateDay,
                            Gender = model.Gender,
                            Nationality = model.Nationality,
                            EssentialTels = new List<EssentialTel>()
                        {
                            new EssentialTel()
                            {
                            OrderNo = 1,
                            RelationShip = model.EssentialTels?.RelationShip,
                            Tel =model.EssentialTels is null?string.Empty: model.EssentialTels.Tel,
                            }
                        }
                        }
                    };

                    if (!string.IsNullOrEmpty(model.CardNumber) & !string.IsNullOrEmpty(model.CardNumberOwner))
                        user.Customer.BankCardNo = new List<BankCardNo>
                        {
                            new BankCardNo{Number=model.CardNumber,Owner=model.CardNumberOwner,OrderNo=1}
                        };

                    var addResult = await _unitOfWork.AddAsync(user, cancellationToken);
                    if (addResult.IsSuccess)
                    {
                        var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                        if (result.IsSuccess)
                        {
                            transaction.Complete();
                            //send sms for customer
                            Dictionary<string, string> smsParameters = new Dictionary<string, string>();
                            smsParameters.Add("USERNAME", user.UserName);
                            smsParameters.Add("PASSWORD", user.Mobile);
                            await _smsSender.SendByVerifyAsync(user.Mobile, 856348, smsParameters, cancellationToken);
                            return CommandResult<CreateOrEditCustomerViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                        }
                        else
                            return CommandResult<CreateOrEditCustomerViewModel>.Failure(result.Message, model);
                    }
                    else
                    {
                        return CommandResult<CreateOrEditCustomerViewModel>.Failure(addResult.Message, model);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    //_unitOfWork.Rollback();
                    await _logManager.RaiseLogAsync(ex, cancellationToken);
                    return CommandResult<CreateOrEditCustomerViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new CreateOrEditCustomerViewModel());
                }
            }

        }
        public async Task<CommandResult<CreateOrEditCustomerViewModel>> UpdateCustomerAsync(CreateOrEditCustomerViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                DateTime? birthDate = null;
                if (!string.IsNullOrEmpty(model.BirthDate))
                {
                    birthDate = DateTimeTools.ParsePersianToGorgian(model.BirthDate);
                    if (birthDate is null)
                        return CommandResult<CreateOrEditCustomerViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.BirthDate), model);
                    if (birthDate >= DateTime.Now)
                        return CommandResult<CreateOrEditCustomerViewModel>.Failure(string.Format(ValidationMessages.NotMoreThan, Captions.BirthDate, Captions.Today), model);
                }

                var proviancesResult = await _provinceService.GetAllProvincesSelectListItemAsync(0, cancellationToken);
                if (proviancesResult.IsSuccess)
                {
                    model.Proviances = proviancesResult.Data.ToList();
                }
                if (model.CityId.HasValue)
                {
                    var citiesREsult = await _provinceService.GetCitiesOfProvinceSelectListItemAsync(model.CityId.Value, cancellationToken);
                    if (citiesREsult.IsSuccess)
                    {
                        foreach (var item in citiesREsult.Data)
                        {
                            if (item.Value == model.CityId.Value.ToString())
                            {
                                item.Selected = true;
                            }
                            model.Cities.Add(item);
                        }

                    }
                }

                string saltKey = Guid.NewGuid().ToString();
                var customerResult = await _unitOfWork.CustomerRepository.IsExistAsync(model.Id, cancellationToken);

                if (customerResult.IsSuccess)
                {
                    CommandResult<User> userResult = await _unitOfWork.UserRepository.GetUserByCustomerIdAsync(model.Id, cancellationToken);

                    if (!userResult.IsSuccess)
                        return CommandResult<CreateOrEditCustomerViewModel>.Failure(UserMessages.UserNotFound, model);

                    //check uniq nationalCode(username)
                    if (userResult.Data.UserName != model.NationalCode)
                        if (_unitOfWork.UserRepository.GetAllAsIQueryable().Data.Any(x => x.UserName == model.NationalCode))
                            return CommandResult<CreateOrEditCustomerViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.NationalCode), model);

                    //edit customer fields
                    userResult.Data.FullName = model.FullName;
                    userResult.Data.Customer.FatherName = model.FatherName;
                    userResult.Data.Customer.Address = model.Address;
                    if (userResult.Data.Customer.NationalCode != model.NationalCode)
                    {
                        userResult.Data.UserName = model.NationalCode;
                        userResult.Data.Customer.NationalCode = model.NationalCode;
                    }
                    userResult.Data.Customer.Gender = model.Gender;
                    userResult.Data.Customer.Nationality = model.Nationality;
                    userResult.Data.Customer.BirthDateYear = model.BirthDate is null ? null : int.Parse(model.BirthDate.Split('/')[0].ToEnglishNumbers());
                    userResult.Data.Customer.BirthDateMonth = model.BirthDate is null ? null : byte.Parse(model.BirthDate.Split('/')[1].ToEnglishNumbers());
                    userResult.Data.Customer.BirthDateDay = model.BirthDate is null ? null : byte.Parse(model.BirthDate.Split('/')[2].ToEnglishNumbers());
                    userResult.Data.Customer.BirthDate = birthDate;


                    userResult.Data.Mobile = model.Mobile;
                    userResult.Data.Customer.CityId = model.CityId;
                    userResult.Data.Customer.PostalCode = model.PostalCode;
                    userResult.Data.Customer.SanaCode = model.SanaCode;
                    userResult.Data.Customer.JobTitle = model.JobTitle;
                    userResult.Data.LastModifiedDate = DateTime.Now;
                    userResult.Data.IsActive = model.IsActive;

                    var updateResult = _unitOfWork.Update(userResult.Data);
                    if (updateResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult<CreateOrEditCustomerViewModel>.Success(UserMessages.DataEditedSuccessfully, model);
                        else
                            return CommandResult<CreateOrEditCustomerViewModel>.Failure(saveResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditCustomerViewModel>.Failure(updateResult.Message, model);
                }
                else
                    return CommandResult<CreateOrEditCustomerViewModel>.Failure(customerResult.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditCustomerViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new CreateOrEditCustomerViewModel());
            }
        }
        public async Task<CommandResult<CreateOrEditCustomerViewModel>> GetCustomerInfoForEditAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<Customer> result = await _unitOfWork.UserRepository.GetCustomerInfoAsync(customerId, cancellationToken);
                CreateOrEditCustomerViewModel model = new CreateOrEditCustomerViewModel();
                if (result.IsSuccess)
                {

                    model.Id = result.Data.Id;
                    model.Address = result.Data.Address;
                    model.CityId = result.Data.CityId;
                    model.FatherName = result.Data.FatherName;
                    model.FullName = result.Data.User.FullName;
                    model.Mobile = result.Data.User.Mobile;
                    model.IsActive = result.Data.User.IsActive;
                    model.NationalCode = result.Data.NationalCode;
                    model.PostalCode = result.Data.PostalCode;
                    model.SanaCode = result.Data.SanaCode;
                    model.BirthDate = result.Data.BirthDate.HasValue ? result.Data.BirthDate.Value.GeorgianToPersian(ShowMode.OnlyDate) : null;
                    model.JobTitle = result.Data.JobTitle;
                    model.Gender = result.Data.Gender;
                    model.Nationality = result.Data.Nationality;
                    model.EssentialTels = new();
                    model.Address = result.Data.Address;

                    model.CardNumberOwner = string.Empty;
                    model.CardNumber = string.Empty;

                    return CommandResult<CreateOrEditCustomerViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                {
                    return CommandResult<CreateOrEditCustomerViewModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.CustomerInfo), model);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditCustomerViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public CommandResult<IQueryable<CustomerReportModel>> GetCustomerReportListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.CustomerRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data
                        .Include(x => x.User)
                        .Include(x => x.City)
                        .Include(x => x.EssentialTels)
                        .Include(x => x.BankCardNo)
                        .Select(x => new CustomerReportModel
                        {
                            Id = x.Id,
                            FullName = x.User.FullName,
                            Nationaity = x.Nationality != null ? x.Nationality.GetDisplayName() : string.Empty,
                            BirthDate = x.BirthDate != null ? x.BirthDate.Value.GeorgianToPersian(ShowMode.OnlyDate) : string.Empty,
                            Gender = x.Gender != null ? x.Gender.Value.GetDisplayName() : string.Empty,
                            IsActive = x.User.IsActive,
                            Mobile = x.User.Mobile,
                            NationalCode = x.NationalCode,
                            Address = x.Address,
                            CityName = x.City.Title,
                            FatherName = x.FatherName,
                            JobTitle = x.JobTitle,
                            PostalCode = x.PostalCode,
                            SanaCode = x.SanaCode,
                            EssentialTel = x.EssentialTels.OrderBy(x => x.OrderNo).FirstOrDefault().Tel,
                            RelationShip = x.EssentialTels.OrderBy(x => x.OrderNo).FirstOrDefault().RelationShip,
                            BankCardNo = x.BankCardNo.OrderBy(x => x.OrderNo).FirstOrDefault().Number,
                            BankCardOwnerName = x.BankCardNo.OrderBy(x => x.OrderNo).FirstOrDefault().Owner,
                        });
                    return CommandResult<IQueryable<CustomerReportModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<CustomerReportModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CustomerReportModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<string>> GetCustomerAccountStatus(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                string result = string.Empty;
                var allInstallments = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.CustomerId == customerId & x.Status != DocumentStatus.Deleted)
                    .Include(x => x.Installments)
                    .SelectMany(x => x.Installments).ToListAsync();
                if (allInstallments.Count == 0)
                {
                    //نه خوش حساب نه بد حساب
                    result = string.Empty;
                }
                else
                {
                    long countOfPaydedInstallmentWithDelay = 0;
                    foreach (var installment in allInstallments)
                    {
                        if (installment.IsPaid & installment.DelayDays.HasValue)
                            countOfPaydedInstallmentWithDelay += installment.DelayDays.Value;
                        else
                            if (installment.Date.Date < DateTime.Now.Date)
                            countOfPaydedInstallmentWithDelay += Convert.ToInt64((DateTime.Now.Date - installment.Date.Date).TotalDays);
                    }

                    if (countOfPaydedInstallmentWithDelay > (allInstallments.Count * 2))
                    {
                        //بد حساب
                        result = AccountStatusType.DeadBeat.GetDisplayName();
                    }
                    else
                    {
                        //خوش حساب
                        result = AccountStatusType.GoodPay.GetDisplayName();
                    }
                }
                return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, result);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        private async Task<string> CalculateCustomerAccountStatus(int customerId)
        {
            try
            {
                string result = string.Empty;
                var allInstallments = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.CustomerId == customerId & x.Status != DocumentStatus.Deleted)
                    .Include(x => x.Installments)
                    .SelectMany(x => x.Installments).ToListAsync();
                if (allInstallments.Count == 0)
                {
                    //نه خوش حساب نه بد حساب
                    result = string.Empty;
                }
                else
                {
                    long countOfPaydedInstallmentWithDelay = 0;
                    foreach (var installment in allInstallments)
                    {
                        if (installment.IsPaid & installment.DelayDays.HasValue)
                            countOfPaydedInstallmentWithDelay += installment.DelayDays.Value;
                        else
                            if (installment.Date.Date < DateTime.Now.Date)
                            countOfPaydedInstallmentWithDelay += Convert.ToInt64((DateTime.Now.Date - installment.Date.Date).TotalDays);
                    }

                    if (countOfPaydedInstallmentWithDelay > (allInstallments.Count * 2))
                    {
                        //بد حساب
                        result = AccountStatusType.DeadBeat.GetDisplayName();
                    }
                    else
                    {
                        //خوش حساب
                        result = AccountStatusType.GoodPay.GetDisplayName();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                //_logManager.RaiseLog(ex);
                return string.Empty;
            }
        }
        public CommandResult<IQueryable<CustomerBankCardNoModel>> GetCustomerBankCardNumberListAsQuerable(string? BankCardNumber)
        {
            try
            {
                IQueryable<CustomerBankCardNoModel> iQuerableData = null;

                if (!string.IsNullOrWhiteSpace(BankCardNumber))
                {
                    iQuerableData = _unitOfWork.BankCardNoRepository.GetAllAsIQueryable().Data
                                        .Where(x => x.Number.Contains(BankCardNumber))
                                        .Include(x => x.Customer).ThenInclude(x => x.User)
                                        .Select(x => new CustomerBankCardNoModel
                                        {
                                            FullName = x.Customer.User.FullName,
                                            Mobile = x.Customer.User.Mobile,
                                            Number = x.Number,
                                            Owner = x.Owner
                                        });
                }
                return CommandResult<IQueryable<CustomerBankCardNoModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CustomerBankCardNoModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<IQueryable<CustomerPhoneNumberModel>> GetCustomerPhoneNumberListAsQuerable(string? PhoneNumber)
        {
            try
            {
                IQueryable<CustomerPhoneNumberModel> iQuerableData = null;

                if (!string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    //iQuerableData = _unitOfWork.EssentialNumberRepository.GetAllAsIQueryable().Data
                    //					.Include(x => x.Customer).ThenInclude(x => x.User)
                    //					.Where(x => x.Tel.Contains(PhoneNumber) | x.Customer.User.Mobile.Contains(PhoneNumber))
                    //					.Select(x => new CustomerPhoneNumberModel
                    //					{
                    //						FullName = x.Customer.User.FullName,
                    //						Mobile = x.Customer.User.Mobile,
                    //						EssentialTel = x.Tel,
                    //						EssentialTelRatio = x.RelationShip
                    //					});
                    iQuerableData = _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                                        .Include(x => x.User)
                                        .Include(x => x.EssentialTels)
                                        .Where(x => x.User.Mobile.Contains(PhoneNumber) | x.EssentialTels.Any(x => x.Tel.Contains(PhoneNumber)))
                                        .Select(x => new CustomerPhoneNumberModel
                                        {
                                            FullName = x.User.FullName,
                                            Mobile = x.User.Mobile,
                                            EssentialTel = x.EssentialTels.Any() ?
                                            (x.EssentialTels.Any(x => x.Tel.Contains(PhoneNumber)) ?
                                            x.EssentialTels.Where(x => x.Tel.Contains(PhoneNumber)).FirstOrDefault().Tel :
                                            x.EssentialTels.FirstOrDefault().Tel) : string.Empty,

                                            EssentialTelRatio = x.EssentialTels.Any() ?
                                            (x.EssentialTels.Any(x => x.Tel.Contains(PhoneNumber)) ?
                                            x.EssentialTels.Where(x => x.Tel.Contains(PhoneNumber)).FirstOrDefault().RelationShip :
                                            x.EssentialTels.FirstOrDefault().RelationShip) : string.Empty,
                                        });

                }
                return CommandResult<IQueryable<CustomerPhoneNumberModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CustomerPhoneNumberModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }
        public CommandResult<IQueryable<CustomerModel>> GetCustomerListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.CustomerRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data
                        .Include(x => x.ProfileImages)
                        .Include(x => x.User)
                        .Select(x => new CustomerModel
                        {
                            Id = x.Id,
                            FullName = x.User.FullName,
                            IsActive = x.User.IsActive,
                            Mobile = x.User.Mobile,
                            NationalCode = x.NationalCode,
                            HasProfileImage = x.ProfileImages.Any()
                        });
                    return CommandResult<IQueryable<CustomerModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<CustomerModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CustomerModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult> IsExistCustomerByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.CustomerRepository.IsExistAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
                }
                else
                {
                    return CommandResult.Failure(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }
        public async Task<CommandResult> IsExistCustomerByMobileAsync(int id, string mobile, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isExist = false;
                if (id <= 0)
                    isExist = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                        .AnyAsync(x => x.UserType == UserType.Customer & x.Mobile == mobile, cancellationToken);
                else
                    isExist = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                        .Include(x => x.Customer)
                       .AnyAsync(x => x.UserType == UserType.Customer & x.Mobile == mobile & x.Customer.Id != id, cancellationToken);

                if (isExist)
                    return CommandResult.Success(DBOperationMessages.DataFoundedCorrectly);
                else
                    return CommandResult.Failure(DBOperationMessages.DataWasNotFound);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<int>> GetUserIdOfCustomerAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<int> result = await _unitOfWork.CustomerRepository.GetUserIdAsync(customerId, cancellationToken);
                if (result.IsSuccess)
                    return CommandResult<int>.Success(result.Message, result.Data);
                else
                    return CommandResult<int>.Failure(result.Message, result.Data);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<EssentialTelViewModel>> AddEssentialTelAsync(EssentialTelViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<List<EssentialTel>> essentialTels = await _unitOfWork.EssentialNumberRepository.GetAllByCustomerIdAsync(model.CustomerId, cancellationToken);
                int maxOrderNumber = 0;
                if (essentialTels.IsSuccess)
                {
                    if (essentialTels.Data.Any())
                    {
                        maxOrderNumber = essentialTels.Data.Max(x => x.OrderNo);
                    }
                    //check duplicate tel
                    if (essentialTels.Data.Any(x => x.Tel == model.Tel))
                        return CommandResult<EssentialTelViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Tel), model);
                }
                EssentialTel essentialTel = new()
                {
                    CustomerId = model.CustomerId,
                    OrderNo = maxOrderNumber + 1,
                    RelationShip = model.RelationShip,
                    Tel = model.Tel
                };
                await _unitOfWork.EssentialNumberRepository.InsertAsync(essentialTel, cancellationToken);
                var addResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (addResult.IsSuccess)
                    return CommandResult<EssentialTelViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                else
                    return CommandResult<EssentialTelViewModel>.Failure(addResult.Message, model);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EssentialTelViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<CardNumberViewModel>> AddCardNumberAsync(CardNumberViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<List<BankCardNo>> cardNumbers = await _unitOfWork.BankCardNoRepository.GetAllByCustomerIdAsync(model.CustomerId, cancellationToken);
                int maxOrderNumber = 0;
                if (cardNumbers.IsSuccess)
                {
                    if (cardNumbers.Data.Any())
                    {
                        maxOrderNumber = cardNumbers.Data.Max(x => x.OrderNo);
                    }
                    //check duplicate tel
                    if (cardNumbers.Data.Any(x => x.Number == model.CardNumber))
                        return CommandResult<CardNumberViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.CardNumber), model);
                }
                BankCardNo cardNumber = new()
                {
                    CustomerId = model.CustomerId,
                    OrderNo = maxOrderNumber + 1,
                    Owner = model.Owner,
                    Number = model.CardNumber
                };
                await _unitOfWork.BankCardNoRepository.InsertAsync(cardNumber, cancellationToken);
                var addResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (addResult.IsSuccess)
                    return CommandResult<CardNumberViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                else
                    return CommandResult<CardNumberViewModel>.Failure(addResult.Message, model);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CardNumberViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<EssentialTelViewModel>> EditEssentialTelAsync(EssentialTelViewModel model, CancellationToken cancellationToken)
        {
            try
            {

                CommandResult<List<EssentialTel>> essentialTels = await _unitOfWork.EssentialNumberRepository.GetAllByCustomerIdAsync(model.CustomerId, cancellationToken);
                if (essentialTels.IsSuccess)
                {
                    var essentialTel = essentialTels.Data.FirstOrDefault(x => x.Id == model.EssentialTelId);
                    if (essentialTel is not null)
                    {
                        //item is exist
                        //check duplicate tel
                        if (essentialTels.Data.Any(x => x.Tel == model.Tel & x.Id != model.EssentialTelId))
                            return CommandResult<EssentialTelViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Tel), model);

                        essentialTel.RelationShip = model.RelationShip;
                        essentialTel.Tel = model.Tel;

                        _unitOfWork.EssentialNumberRepository.Update(essentialTel);

                        var addResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                        if (addResult.IsSuccess)
                            return CommandResult<EssentialTelViewModel>.Success(UserMessages.DataEditedSuccessfully, model);
                        else
                            return CommandResult<EssentialTelViewModel>.Failure(addResult.Message, model);
                    }
                    else
                    {
                        //item not is exist
                        return CommandResult<EssentialTelViewModel>.Failure(UserMessages.ItemNotFount, model);
                    }

                }
                else
                {
                    return CommandResult<EssentialTelViewModel>.Failure(essentialTels.Message, model);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EssentialTelViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<CardNumberViewModel>> EditCardNumberAsync(CardNumberViewModel model, CancellationToken cancellationToken)
        {
            try
            {

                CommandResult<List<BankCardNo>> cardNumbers = await _unitOfWork.BankCardNoRepository.GetAllByCustomerIdAsync(model.CustomerId, cancellationToken);
                if (cardNumbers.IsSuccess)
                {
                    var cardNumber = cardNumbers.Data.FirstOrDefault(x => x.Id == model.CardNumberId);
                    if (cardNumber is not null)
                    {
                        //item is exist
                        //check duplicate tel
                        if (cardNumbers.Data.Any(x => x.Number == model.CardNumber & x.Id != model.CardNumberId))
                            return CommandResult<CardNumberViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Tel), model);

                        cardNumber.Owner = model.Owner;
                        cardNumber.Number = model.CardNumber;

                        _unitOfWork.BankCardNoRepository.Update(cardNumber);

                        var addResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                        if (addResult.IsSuccess)
                            return CommandResult<CardNumberViewModel>.Success(UserMessages.DataEditedSuccessfully, model);
                        else
                            return CommandResult<CardNumberViewModel>.Failure(addResult.Message, model);
                    }
                    else
                    {
                        //item not is exist
                        return CommandResult<CardNumberViewModel>.Failure(UserMessages.ItemNotFount, model);
                    }
                }
                else
                {
                    return CommandResult<CardNumberViewModel>.Failure(cardNumbers.Message, model);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CardNumberViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }


        public async Task<CommandResult<EssentialTelModel>> GetEssentialTelAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.EssentialNumberRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    EssentialTelModel essentialTel = new()
                    {
                        CustomerId = result.Data.CustomerId,
                        Id = result.Data.Id,
                        RelationShip = result.Data.RelationShip,
                        Tel = result.Data.Tel
                    };
                    return CommandResult<EssentialTelModel>.Success(result.Message, essentialTel);
                }
                else
                {
                    return CommandResult<EssentialTelModel>.Failure(result.Message, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EssentialTelModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }


        public async Task<CommandResult> ChangeEssentialTelOrderNumberAsync(long id, bool isUp, CancellationToken cancellationToken)
        {
            try
            {
                var essentialTelResult = await _unitOfWork.EssentialNumberRepository.GetByIdAsync(id, cancellationToken);

                if (essentialTelResult.IsSuccess)
                {
                    if (isUp)
                    {
                        //up
                        var previousItem = _unitOfWork.EssentialNumberRepository.GetAllAsIQueryable().Data
                            .Where(x => x.CustomerId == essentialTelResult.Data.CustomerId & x.OrderNo < essentialTelResult.Data.OrderNo)
                            .OrderByDescending(x => x.OrderNo).FirstOrDefault();
                        if (previousItem is not null)
                        {
                            var tempItem = previousItem.OrderNo;
                            previousItem.OrderNo = essentialTelResult.Data.OrderNo;
                            essentialTelResult.Data.OrderNo = tempItem;
                            _unitOfWork.EssentialNumberRepository.Update(previousItem);
                        }
                    }
                    else
                    {
                        //down
                        var nextItem = _unitOfWork.EssentialNumberRepository.GetAllAsIQueryable().Data.Where(x => x.CustomerId == essentialTelResult.Data.CustomerId & x.OrderNo > essentialTelResult.Data.OrderNo).OrderBy(x => x.OrderNo).FirstOrDefault();
                        if (nextItem is not null)
                        {
                            var tempItem = nextItem.OrderNo;
                            nextItem.OrderNo = essentialTelResult.Data.OrderNo;
                            essentialTelResult.Data.OrderNo = tempItem;
                            _unitOfWork.EssentialNumberRepository.Update(nextItem);
                        }
                    }
                    //update
                    _unitOfWork.EssentialNumberRepository.Update(essentialTelResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);
                }
                else
                {
                    return CommandResult.Failure(essentialTelResult.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }


        public async Task<CommandResult> ChangeCardNumberOrderNumberAsync(long id, bool isUp, CancellationToken cancellationToken)
        {
            try
            {
                var cardNumberResult = await _unitOfWork.BankCardNoRepository.GetByIdAsync(id, cancellationToken);

                if (cardNumberResult.IsSuccess)
                {
                    if (isUp)
                    {
                        //up
                        var previousItem = _unitOfWork.BankCardNoRepository.GetAllAsIQueryable().Data.Where(x => x.CustomerId == cardNumberResult.Data.CustomerId & x.OrderNo < cardNumberResult.Data.OrderNo).OrderByDescending(x => x.OrderNo).FirstOrDefault();

                        if (previousItem is not null)
                        {
                            var tempItem = previousItem.OrderNo;
                            previousItem.OrderNo = cardNumberResult.Data.OrderNo;
                            cardNumberResult.Data.OrderNo = tempItem;
                            _unitOfWork.BankCardNoRepository.Update(previousItem);
                        }
                    }
                    else
                    {
                        //down
                        var nextItem = _unitOfWork.BankCardNoRepository.GetAllAsIQueryable().Data.Where(x => x.CustomerId == cardNumberResult.Data.CustomerId & x.OrderNo > cardNumberResult.Data.OrderNo).OrderBy(x => x.OrderNo).FirstOrDefault();
                        if (nextItem is not null)
                        {
                            var tempItem = nextItem.OrderNo;
                            nextItem.OrderNo = cardNumberResult.Data.OrderNo;
                            cardNumberResult.Data.OrderNo = tempItem;
                            _unitOfWork.BankCardNoRepository.Update(nextItem);
                        }
                    }
                    //update
                    _unitOfWork.BankCardNoRepository.Update(cardNumberResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);

                }
                else
                {
                    return CommandResult.Failure(cardNumberResult.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public CommandResult<IQueryable<EssentialTelModel>> GetEssentialTelListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.EssentialNumberRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new EssentialTelModel
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        OrderNumber = x.OrderNo,
                        RelationShip = x.RelationShip,
                        Tel = x.Tel
                    });
                    return CommandResult<IQueryable<EssentialTelModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<EssentialTelModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<EssentialTelModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<IQueryable<CardNumberModel>> GetCardNumberListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.BankCardNoRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new CardNumberModel
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        OrderNumber = x.OrderNo,
                        Owner = x.Owner,
                        Number = x.Number
                    });
                    return CommandResult<IQueryable<CardNumberModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<CardNumberModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CardNumberModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }


        public async Task<CommandResult<List<EssentialTelModel>>> GetCustomerEssentialTelListAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.EssentialNumberRepository.GetAllByCustomerIdAsync(customerId, cancellationToken);
                if (result.IsSuccess)
                {
                    var data = result.Data.Select(x => new EssentialTelModel
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        OrderNumber = x.OrderNo,
                        RelationShip = x.RelationShip,
                        Tel = x.Tel
                    }).ToList();
                    return CommandResult<List<EssentialTelModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, data);
                }
                else
                {
                    return CommandResult<List<EssentialTelModel>>.Failure(result.Message, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<EssentialTelModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<CardNumberModel>>> GetCustomerCardNumberListAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.BankCardNoRepository.GetAllByCustomerIdAsync(customerId, cancellationToken);
                if (result.IsSuccess)
                {
                    var data = result.Data.Select(x => new CardNumberModel
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        OrderNumber = x.OrderNo,
                        Owner = x.Owner,
                        Number = x.Number
                    }).ToList();
                    return CommandResult<List<CardNumberModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, data);
                }
                else
                    return CommandResult<List<CardNumberModel>>.Failure(result.Message, new());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<CardNumberModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<CardNumberModel>>> GetCustomerCardNumberListByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Include(x => x.BankCardNo)
                    .SelectMany(x => x.BankCardNo)
                    .ToListAsync(cancellationToken);
                if (list is not null)
                {
                    var data = list.Select(x => new CardNumberModel
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        OrderNumber = x.OrderNo,
                        Owner = x.Owner,
                        Number = x.Number
                    }).ToList();
                    return CommandResult<List<CardNumberModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, data);
                }
                else
                    return CommandResult<List<CardNumberModel>>.Failure(string.Format(UserMessages.NotFound, Captions.BankCardNumbers), new());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<CardNumberModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<EssentialTelModel>> GetEssentialTelAsync(long id, int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.EssentialNumberRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    if (result.Data.CustomerId == customerId)
                    {
                        EssentialTelModel model = new()
                        {
                            Id = result.Data.Id,
                            CustomerId = result.Data.CustomerId,
                            RelationShip = result.Data.RelationShip,
                            Tel = result.Data.Tel
                        };
                        return CommandResult<EssentialTelModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                    }
                    else
                    {
                        return CommandResult<EssentialTelModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.EssentialTel), new());

                    }
                }
                else
                {
                    return CommandResult<EssentialTelModel>.Failure(result.Message, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EssentialTelModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        public async Task<CommandResult<CardNumberModel>> GetCardNumberAsync(long id, int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.BankCardNoRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    if (result.Data.CustomerId == customerId)
                    {
                        CardNumberModel model = new()
                        {
                            Id = result.Data.Id,
                            CustomerId = result.Data.CustomerId,
                            Owner = result.Data.Owner,
                            Number = result.Data.Number
                        };
                        return CommandResult<CardNumberModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                    }
                    else
                    {
                        return CommandResult<CardNumberModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.EssentialTel), new());

                    }
                }
                else
                {
                    return CommandResult<CardNumberModel>.Failure(result.Message, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CardNumberModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        public async Task<CommandResult<string>> ResetCustomerPasswordAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = await _unitOfWork.CustomerRepository
                    .GetAllAsIQueryable().Data.Where(x => x.Id == customerId)
                    .Select(x => x.UserId).FirstOrDefaultAsync(cancellationToken);
                var userResult = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
                if (userResult.IsSuccess)
                {
                    //user was founded
                    userResult.Data.PasswordHash = Encryptor.Encrypt(userResult.Data.Mobile, userResult.Data.PasswordSalt);
                    userResult.Data.SecurityStamp = Guid.NewGuid();
                    userResult.Data.LockDate = null;
                    userResult.Data.IsLocked = false;
                    userResult.Data.WrongPasswordCount = 0;
                    userResult.Data.LastModifiedDate = DateTime.Now;
                    userResult.Data.LastPasswordChangeDate = DateTime.Now;
                    _unitOfWork.UserRepository.Update(userResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                    {
                        //send sms for customer
                        Dictionary<string, string> smsParameters = new Dictionary<string, string>();
                        //smsParameters.Add("NEWPASSWORD", userResult.Data.Mobile);
                        //await _smsSender.SendByVerifyAsync(userResult.Data.Mobile, 599534, smsParameters, cancellationToken);

                        smsParameters.Add("USERNAME", userResult.Data.UserName);
                        smsParameters.Add("PASSWORD", userResult.Data.Mobile);
                        await _smsSender.SendByVerifyAsync(userResult.Data.Mobile, 856348, smsParameters, cancellationToken);

                        return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, userResult.Data.FullName);
                    }
                    else
                    {
                        //update failed
                        return CommandResult<string>.Failure(updateResult.Message, userResult.Data.FullName);
                    }
                }
                else
                {
                    //user not found
                    return CommandResult<string>.Failure(UserMessages.UserNotFound, userResult.Data.FullName);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<string>> RemoveCustomerAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var customerResult = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
                var customerEssentialTelsResult = await _unitOfWork.EssentialNumberRepository.GetAllByCustomerIdAsync(customerId, cancellationToken);
                var userResult = await _unitOfWork.UserRepository.GetByIdAsync(customerResult.Data.UserId, cancellationToken);

                //remove user and customer and essentialTels
                _unitOfWork.EssentialNumberRepository.DeleteRange(customerEssentialTelsResult.Data);
                _unitOfWork.CustomerRepository.Delete(customerResult.Data);
                _unitOfWork.UserRepository.Delete(userResult.Data);

                var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (removeResult.IsSuccess)
                    //return CommandResult.Success(string.Format(UserMessages.CustomerInfoByName0IsDeleted, userResult.Data.FullName));
                    return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, userResult.Data.FullName);
                else
                    return CommandResult<string>.Failure(removeResult.Message, userResult.Data.FullName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<string>> RemoveEssentialNumberAsync(long id, int customerId, CancellationToken cancellationToken)
        {
            try
            {
                int countOfCustomerEssentialNumbers = _unitOfWork.EssentialNumberRepository.GetAllAsIQueryable().Data.Count(x => x.CustomerId == customerId);
                if (countOfCustomerEssentialNumbers > 1)
                {
                    //only can remove if count of essential numbers > 1
                    CommandResult<EssentialTel> essentialNumberResult = await _unitOfWork.EssentialNumberRepository.GetByIdAndCustomerIdAsync(id, customerId, cancellationToken);
                    if (essentialNumberResult.IsSuccess)
                    {
                        //remove essentialTel
                        _unitOfWork.EssentialNumberRepository.Delete(essentialNumberResult.Data);
                        var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                        if (removeResult.IsSuccess)
                            return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, essentialNumberResult.Data.Tel);
                        else
                            return CommandResult<string>.Failure(removeResult.Message, essentialNumberResult.Data.Tel);
                    }
                    else
                        return CommandResult<string>.Failure(essentialNumberResult.Message, string.Empty);

                }
                else if (countOfCustomerEssentialNumbers == 1)
                    return CommandResult<string>.Failure(UserMessages.AtLeastOneEssentialNumberIsRequiredForTheCustomer, string.Empty);
                else
                    return CommandResult<string>.Failure(UserMessages.ThisCustomerHaventAEssentialNumber, string.Empty);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }


        public async Task<CommandResult<string>> RemoveCardNumberAsync(long id, int customerId, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<BankCardNo> cardNumberResult = await _unitOfWork.BankCardNoRepository.GetByIdAndCustomerIdAsync(id, customerId, cancellationToken);
                if (cardNumberResult.IsSuccess)
                {
                    //remove CardNumber
                    _unitOfWork.BankCardNoRepository.Delete(cardNumberResult.Data);
                    var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (removeResult.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, cardNumberResult.Data.Number);
                    else
                        return CommandResult<string>.Failure(removeResult.Message, cardNumberResult.Data.Number);
                }
                else
                    return CommandResult<string>.Failure(cardNumberResult.Message, string.Empty);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }
        public async Task<CommandResult<string>> RemoveProfileImageAsync(long id, int customerId, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<ProfileImage> profileImageResult = await _unitOfWork.ProfileImageRepository.GetByIdAndCustomerIdAsync(id, customerId, cancellationToken);
                if (profileImageResult.IsSuccess)
                {
                    //remove profile image
                    _unitOfWork.ProfileImageRepository.Delete(profileImageResult.Data);

                    if (!string.IsNullOrEmpty(profileImageResult.Data.ImageName))
                        _fileService.DeleteFile(profileImageResult.Data.ImageName, _filePathAddress.CustomerProfileImage);
                    var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (removeResult.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, profileImageResult.Data.ImageName);
                    else
                        return CommandResult<string>.Failure(removeResult.Message, profileImageResult.Data.ImageName);
                }
                else
                    return CommandResult<string>.Failure(profileImageResult.Message, string.Empty);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }


        public async Task<CommandResult<int>> GetCustomerIdByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                var customerId = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserName == nationalCode)
                    .Include(x => x.Customer)
                    .Select(x => x.Customer.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (customerId > 0)
                {
                    return CommandResult<int>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerId);
                }
                else
                {
                    return CommandResult<int>.Failure(string.Format(OperationResultMessage.NotFound, Captions.Customer), 0);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<bool>> IsExistDocumentByStatusAnNumberAsync(DocumentStatus documentStatus, int documentNumber, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<bool> isExistResult = await _unitOfWork.DocumentRepository.IsExistDocumentNumberBySatusAsync(documentStatus, documentNumber, cancellationToken);
                if (isExistResult.IsSuccess)
                    return CommandResult<bool>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, true);
                else
                    return CommandResult<bool>.Failure(string.Format(OperationResultMessage.NotFound, Captions.Document), false);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }
        public async Task<CommandResult<DocumentOwnerInformationModel>> GetDocumentOwnerInfoAsync(string nationalCose, CancellationToken cancellationToken)
        {
            try
            {
                DocumentOwnerInformationModel? customer = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.NationalCode == nationalCose)
                    .Include(x => x.User)
                    .Include(x => x.City)
                    .Include(x => x.EssentialTels)
                    .Include(x => x.Documents.Where(x => x.Status != DocumentStatus.Deleted)).ThenInclude(x => x.Installments)
                    .Select(c => new DocumentOwnerInformationModel
                    {
                        CustomerId = c.Id,
                        FullName = c.User.FullName,
                        CityId = c.CityId == null ? 0 : c.CityId.Value,
                        CityName = c.City == null ? string.Empty : c.City.Title,
                        Address = string.IsNullOrEmpty(c.Address) ? string.Empty : c.Address,
                        EssentialTel = c.EssentialTels == null ? string.Empty : c.EssentialTels.OrderBy(c => c.OrderNo).FirstOrDefault().Tel,
                        EssentialTelRatio = c.EssentialTels == null ? string.Empty : c.EssentialTels.OrderBy(c => c.OrderNo).FirstOrDefault().RelationShip,
                        FatherName = string.IsNullOrEmpty(c.FatherName) ? string.Empty : c.FatherName,
                        Mobile = c.User.Mobile,
                        NationalCode = c.NationalCode,
                        Documents = c.Documents.Where(x => x.Status != DocumentStatus.Deleted).Select(x => new DocumentSummaryInfo
                        {
                            Id = x.Id,
                            DocumentNo = x.DocumentNo,
                            Status = x.Status,
                            NotPaidInstallmentCount = x.Installments.Count(i => !i.IsPaid & i.Date.Date < DateTime.Now.Date)
                        }).ToList(),
                    }).FirstOrDefaultAsync();
                if (customer is null)
                {
                    return CommandResult<DocumentOwnerInformationModel>.Failure(OperationResultMessage.OperationIsFailure, null);
                }
                else
                {
                    CommandResult<City> province = await _unitOfWork.CityRepository.GetProvinceAsync(customer.CityId, cancellationToken);
                    if (province.IsSuccess)
                    {
                        customer.ProvinceName = province.Data.Title;
                    }
                    return CommandResult<DocumentOwnerInformationModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customer);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<DocumentOwnerInformationModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<bool>> IsExistCustomerByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                var isExistCustomerReult = await _unitOfWork.CustomerRepository.IsExistByNationalCodeAsync(nationalCode, cancellationToken);
                if (isExistCustomerReult.IsSuccess)
                {

                    return CommandResult<bool>.Success(UserMessages.OneUserIsFounded, true);
                }
                else
                {
                    return CommandResult<bool>.Failure(UserMessages.UserNotFound, false);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<long>> InstallmentAmountCalculator(long remainAmount, byte installmentCount, CancellationToken cancellationToken)
        {
            try
            {
                var loanSettingResult = await _settingService.GetSettingAsync<LoanSettingsViewModel>(Domain.Enums.SettingType.LoanSetting, cancellationToken);
                if (loanSettingResult.IsSuccess)
                {
                    if (decimal.TryParse(loanSettingResult.Data.MonthlyProfitPercentage, out decimal monthlyProfitPercentage))
                    {
                        long proccessResult = 0;
                        proccessResult = (long)((1 + (installmentCount * monthlyProfitPercentage / 100)) * remainAmount) / installmentCount;
                        if (proccessResult > 0)
                        {
                            proccessResult = NumberTools.RoundUpNumber(proccessResult.ToString(), 4);
                        }
                        return CommandResult<long>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, proccessResult);
                    }
                    else
                    {
                        return CommandResult<long>.Failure(UserMessages.MonthlyProfitPercentageNotRegisterd, 0);
                    }
                }
                else
                {
                    return CommandResult<long>.Failure(UserMessages.LoanSettingsNotRegisterd, 0);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<bool>> IsActiveCustomerByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                var isActiveCustomerReult = await _unitOfWork.CustomerRepository.IsActiveByNationalCodeAsync(nationalCode, cancellationToken);
                if (isActiveCustomerReult.IsSuccess)
                {

                    return CommandResult<bool>.Success(UserMessages.UserIsActive, true);
                }
                else
                {
                    return CommandResult<bool>.Failure(UserMessages.UserIsNotActive, false);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public CommandResult<List<InstallmentDateModel>> CalculateInstallmentDate(byte installmentCount, string installmentDate)
        {
            try
            {
                DateTime? miladiDate = DateTimeTools.ParsePersianToGorgian(installmentDate);
                if (miladiDate.HasValue)
                {
                    List<InstallmentDateModel> list = new List<InstallmentDateModel>();
                    string[] installmentDateSplit = installmentDate.Split("/");
                    int currentDay = int.Parse(installmentDateSplit[2]);
                    int currentMonth = int.Parse(installmentDateSplit[1]);
                    int currentYear = int.Parse(installmentDateSplit[0]);
                    string nextDate = string.Empty;

                    for (int i = 1; i <= installmentCount; i++)
                    {
                        if (currentMonth + 1 <= 12)
                        {
                            currentMonth += 1;
                        }
                        else
                        {
                            currentMonth = 1;
                            currentYear += 1;
                        }
                        nextDate = $"{currentYear}/{(currentMonth < 10 ? "0" + currentMonth.ToString() : currentMonth)}/{(currentDay < 10 ? "0" + currentDay.ToString() : currentDay)}";

                        list.Add(new InstallmentDateModel
                        {
                            Row = i,
                            PersianDate = nextDate,
                            GorgianDate = DateTimeTools.ParsePersianToGorgian(nextDate).Value,
                        });
                    }

                    return CommandResult<List<InstallmentDateModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
                }
                else
                {
                    return CommandResult<List<InstallmentDateModel>>.Failure(string.Format(ValidationMessages.Invalid, Captions.Date), null);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<InstallmentDateModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetCollateralTypesAsync(int selectedId, CancellationToken cancellationToken)
        {
            try
            {
                var collateralTypesResult = await _unitOfWork.CollateralTypeRepository.GetAllAsync(cancellationToken);
                if (collateralTypesResult.IsSuccess)
                {
                    var listItems = collateralTypesResult.Data.Select(x => new SelectListItem
                    {
                        Selected = x.Id == selectedId,
                        Text = x.Title,
                        Value = x.Id.ToString()
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, listItems);
                }
                return CommandResult<List<SelectListItem>>.FailureInRetrivingData(new List<SelectListItem>());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new List<SelectListItem>());
            }
        }



        public async Task<CommandResult<EditCustomerSummaryInfoViewModel>> GetCustomerSummaryInfo(string nationalCode, CancellationToken cancellationToken)
        {
            try
            {
                var customerInfo = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.NationalCode == nationalCode)
                    .Include(u => u.User)
                    .Select(x => new EditCustomerSummaryInfoViewModel
                    {
                        CustomerId = x.Id,
                        FatherName = x.FatherName,
                        Gender = x.Gender,
                        CustomerName = x.User.FullName,
                        CustomerMobile = x.User.Mobile,
                    }).FirstOrDefaultAsync();

                if (customerInfo is not null)
                    return CommandResult<EditCustomerSummaryInfoViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerInfo);
                else
                    return CommandResult<EditCustomerSummaryInfoViewModel>.Failure(UserMessages.UserNotFound, null);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EditCustomerSummaryInfoViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<EditCustomerSummaryInfoViewModel>> UpdateCustomerSummaryInfoAsync(EditCustomerSummaryInfoViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var userResult = await _unitOfWork.UserRepository.GetUserByCustomerIdAsync(model.CustomerId, cancellationToken);

                if (!userResult.IsSuccess)
                {
                    //user not exist
                    return CommandResult<EditCustomerSummaryInfoViewModel>.Failure(UserMessages.UserNotFound, model);
                }
                userResult.Data.Mobile = model.CustomerMobile;
                userResult.Data.FullName = model.CustomerName;
                userResult.Data.Customer.FatherName = model.FatherName;
                userResult.Data.Customer.Gender = model.Gender;

                _unitOfWork.Update(userResult.Data);
                var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (updateResult.IsSuccess)
                {
                    return CommandResult<EditCustomerSummaryInfoViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                {
                    return CommandResult<EditCustomerSummaryInfoViewModel>.Failure(updateResult.Message, model);
                }


            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EditCustomerSummaryInfoViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<CreateDocumentViewModel>> CreateDocumentAsync(CreateDocumentViewModel model, CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int newCustomerId = 0;
                    //CommandResult<bool> existDocumentResult = await IsExistDocumentByStatusAnNumberAsync(DocumentStatus.Active, model.DocumentNo, cancellationToken);
                    bool isExist = _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data.Any(x => x.Status == DocumentStatus.Active && x.DocumentNo == model.DocumentNo);
                    if (isExist)
                        return CommandResult<CreateDocumentViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.DocumentNumber), model);

                    var galleryResult = await _unitOfWork.GalleryRepository.GetByIdAsync(model.GalleryId, cancellationToken);
                    if (!galleryResult.IsSuccess)
                        return CommandResult<CreateDocumentViewModel>.Failure(UserMessages.GalleryIsNotValid, model);


                    var sellerResult = await _unitOfWork.SellerRepository.GetByIdAsync(model.SellerId, cancellationToken);
                    if (!sellerResult.IsSuccess)
                        return CommandResult<CreateDocumentViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Seller), model);

                    if (!sellerResult.Data.HasAccessToRegisterLoan)
                        return CommandResult<CreateDocumentViewModel>.Failure(UserMessages.SellerHasNotAccessToRegisterDocument, model);

                    //CommandResult<int> customerIdResult = await _unitOfWork.CustomerRepository.GetCustomerIdByNationalCodeAsync(model.NationalCode, cancellationToken);
                    var customer = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                        .Where(x => x.NationalCode == model.NationalCode)
                        .Include(x => x.User)
                        .FirstOrDefaultAsync(cancellationToken);
                    DateTime docDate = DateTimeTools.ParsePersianToGorgianDateTime(model.DocumentDate).Value;

                    var docDateArray = model.DocumentDate.Split("/");
                    if (byte.Parse(docDateArray[2]) == 31 | byte.Parse(docDateArray[2]) == 30)
                    {
                        return CommandResult<CreateDocumentViewModel>.Failure(UserMessages.PersianDateCanNotBe31And30, model);
                    }
                    //save collaterals images
                    List<Collateral> collaterals = new List<Collateral>();
                    string path = _filePathAddress.CollateralsDocs;
                    foreach (var item in model.Collaterals)
                    {
                        string fileName = string.Empty;
                        if (item.ImageFile is not null)
                        {
                            fileName = Guid.NewGuid().ToString();
                            if (!await _fileService.UploadFileAsync(item.ImageFile, path, fileName))
                                return CommandResult<CreateDocumentViewModel>.Failure(UserMessages.ErrorInUploadFile, model);
                        }
                        collaterals.Add(new Collateral
                        {
                            CollateralTypeId = item.CollateralTypeId,
                            ImageName = item.ImageFile is null ? null : $"{fileName}{Path.GetExtension(item.ImageFile.FileName)}",
                            Description = item.Description,
                        });
                    }


                    Dictionary<string, string> messageParameters = new Dictionary<string, string>();
                    List<Installment> installment = CalculateInstallment(model.InstallmentCount, model.DocumentDate, long.Parse(model.InstallmentAmount.Replace(",", "")));
                    Domain.Entities.Document newDocument = new()
                    {
                        AdminDescription = model.AdminDescription,
                        DocumentDate = docDate,
                        DocumentNo = model.DocumentNo,
                        GalleryId = model.GalleryId,
                        SellerId = model.SellerId,
                        Status = DocumentStatus.Active,
                        RemainAmount = long.Parse(model.RemainAmount.Replace(",", "")),
                        RegisterDate = DateTime.Now,
                        DayOfMonth = byte.Parse(docDateArray[2]),
                        InstallmentCount = model.InstallmentCount,
                        PrepaymentAmount = long.Parse(model.PrepaymentAmount.Replace(",", "")),
                        InstallmentAmount = long.Parse(model.InstallmentAmount.Replace(",", "")),
                        Collaterals = collaterals,
                        Installments = installment
                    };
                    #region sms parameters
                    string customerMobileNumber = string.Empty;
                    string userName = string.Empty;
                    string password = string.Empty;



                    #endregion
                    messageParameters.Add("3", newDocument.DocumentNo.ToString());//شماره سند
                    messageParameters.Add("4", newDocument.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate));//تاریخ سند
                    messageParameters.Add("5", newDocument.InstallmentCount.ToString());//تعداد کل اقساط به عدد
                    messageParameters.Add("6", newDocument.InstallmentCount.ToString().ToPersianAlphabetNumber());//تعداد کل اقساط به حروف
                    messageParameters.Add("7", newDocument.InstallmentAmount.ToString("N0"));//مبلغ اقساط
                    messageParameters.Add("8", galleryResult.Data.Name);//گالری
                    messageParameters.Add("9", newDocument.PrepaymentAmount.ToString("N0"));//مبلغ پیش پرداخت
                    messageParameters.Add("10", (newDocument.PrepaymentAmount + newDocument.RemainAmount).ToString("N0"));//مبلغ فاکتور
                                                                                                                          //if (customerIdResult.IsSuccess)
                    if (customer is not null)
                    {
                        //exist customer
                        newDocument.CustomerId = customer.Id;
                        await _unitOfWork.AddAsync(newDocument, cancellationToken);

                        messageParameters.Add("0", customer.Gender.HasValue ? (customer.Gender.Value == GenderType.Men ? Captions.Mr : Captions.Lady) : string.Empty);//جنسیت
                        messageParameters.Add("1", customer.User.FullName);//نام و نام خانوادگی
                        messageParameters.Add("2", customer.NationalCode);//کد ملی
                    }
                    else
                    {
                        DateTime? birthDate = null;
                        if (!string.IsNullOrEmpty(model.BirthDate))
                        {
                            birthDate = DateTimeTools.ParsePersianToGorgian(model.BirthDate);
                            if (birthDate is null)
                                return CommandResult<CreateDocumentViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.BirthDate), model);
                            if (birthDate >= DateTime.Now)
                                return CommandResult<CreateDocumentViewModel>.Failure(string.Format(ValidationMessages.NotMoreThan, Captions.BirthDate, Captions.Today), model);
                        }

                        CommandResult<City> cityResult = null;
                        if (model.CityId.HasValue)
                        {
                            cityResult = await _unitOfWork.CityRepository.GetCityAsync(model.CityId.Value, cancellationToken);
                            if (!cityResult.IsSuccess)
                                return CommandResult<CreateDocumentViewModel>.Failure(UserMessages.SelectedCityIsValid, model);
                        }

                        //new customer
                        List<Domain.Entities.Document> customerDocuments = new();
                        customerDocuments.Add(newDocument);

                        string saltKey = Guid.NewGuid().ToString();
                        User newUser = new()
                        {
                            UserName = model.NationalCode,
                            FullName = model.FullName,
                            PasswordSalt = saltKey,
                            PasswordHash = Encryptor.Encrypt(model.Mobile, saltKey),
                            IsActive = true,
                            Mobile = model.Mobile,
                            IsLocked = false,
                            RegisterDate = DateTime.Now,
                            UserType = UserType.Customer,
                            Customer = new()
                            {
                                NationalCode = model.NationalCode,
                                Documents = customerDocuments,
                                CityId = model.CityId,
                                Gender = model.Gender,
                                Nationality = model.Nationality,
                                BirthDate = birthDate,
                                Address = model.Address,
                                BirthDateYear = model.BirthDate is null ? null : int.Parse(model.BirthDate.Split('/')[0]),
                                BirthDateMonth = model.BirthDate is null ? null : byte.Parse(model.BirthDate.Split('/')[1]),
                                BirthDateDay = model.BirthDate is null ? null : byte.Parse(model.BirthDate.Split('/')[2]),
                                FatherName = model.FatherName,
                                JobTitle = model.JobTitle,
                                PostalCode = model.PostalCode,
                            }
                        };

                        messageParameters.Add("0", newUser.Customer.Gender.HasValue ? (newUser.Customer.Gender.Value == GenderType.Men ? Captions.Mr : Captions.Lady) : string.Empty);//جنسیت
                        messageParameters.Add("1", newUser.FullName);//نام و نام خانوادگی
                        messageParameters.Add("2", newUser.Customer.NationalCode);//کد ملی

                        if (!string.IsNullOrEmpty(model.CardNumber))
                        {
                            newUser.Customer.BankCardNo = new List<BankCardNo>
                            {
                                new BankCardNo
                            {
                                Number = model.CardNumber,
                                OrderNo = 1,
                                Owner = model.CardNumberOwner
                            }
                            };
                        }
                        if (!string.IsNullOrEmpty(model.EssentialTel))
                        {
                            newUser.Customer.EssentialTels = new List<EssentialTel> {
                                new EssentialTel
                            {
                                Tel = model.EssentialTel,
                                OrderNo = 1,
                                RelationShip = model.EssentialTelRatio
                            }
                            };
                        }
                        password = model.Mobile;
                        userName = model.NationalCode;
                        customerMobileNumber = model.Mobile;
                        await _unitOfWork.AddAsync(newUser, cancellationToken);

                        newCustomerId = newUser.Customer.Id;
                    }

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                    {
                        //send customer message
                        #region send message and sms for customer
                        if (customer is null)
                        {
                            //new user
                            //send sms for customer
                            //string smsContent = $"با حساب کاربری زیر می توانید از امکانات سامانه استفاده کنید.\nنام کاربری:{userName}\nرمز ورود:{password}\nسایت:gold.com";
                            //await _smsSender.SendAsync(customerMobileNumber, smsContent, cancellationToken);
                            Dictionary<string, string> smsParameters = new Dictionary<string, string>();
                            smsParameters.Add("USERNAME", userName);
                            smsParameters.Add("PASSWORD", password);
                            await _smsSender.SendByVerifyAsync(customerMobileNumber, 856348, smsParameters, cancellationToken);

                            //customerIdResult = await _unitOfWork.CustomerRepository.GetCustomerIdByNationalCodeAsync(model.NationalCode, cancellationToken);
                        }

                        var message = new CreateCustomerMessageModel
                        {
                            CustomerId = newCustomerId != 0 ? customer.Id : newCustomerId,
                            DocumentId = newDocument.Id,
                            installmentId = null,
                            MessageSettingType = SettingType.MessageType_DocumentRegistration,
                            MessageType = CustomerMessageType.RegisterDocument,
                            Parameters = messageParameters,
                            Title = Captions.MessageTitle_RegisterDocument,
                        };
                        await SendMessageToCustomerAsync(message);
                        var sendMessageResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        transaction.Complete();
                        #endregion
                        return CommandResult<CreateDocumentViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                    }
                    else
                        return CommandResult<CreateDocumentViewModel>.Failure(result.Message, model);
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    await _logManager.RaiseLogAsync(ex, cancellationToken);
                    return CommandResult<CreateDocumentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
                }
            }


        }
        public async Task<CommandResult<EditDocumentViewModel>> EditDocumentInfoAsync(EditDocumentViewModel model, CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var docStatusResult = await GetDocumentStatusByIdAsync(model.DocumentId, cancellationToken);
                    if (docStatusResult.IsSuccess)
                    {

                        if (docStatusResult.Data == DocumentStatus.Active)
                        {
                            var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                                .Where(x => x.Id == model.DocumentId)
                                .Include(x => x.Installments).ThenInclude(x => x.Payments)
                                .Include(x => x.Installments).ThenInclude(x => x.CustomerMessages)
                                .FirstOrDefaultAsync(cancellationToken);
                            var docDate = model.DocumentDate.ParsePersianToGorgian();
                            var docDateArray = model.DocumentDate.Split("/");
                            if (byte.Parse(docDateArray[2]) == 31 | byte.Parse(docDateArray[2]) == 30)
                            {
                                return CommandResult<EditDocumentViewModel>.Failure(UserMessages.PersianDateCanNotBe31And30, model);
                            }
                            if (document is not null)
                            {
                                //شماره سند نباید تکراری باشد
                                //اگر شماره سند تغییر کرده باشد، چک میکنیم
                                if (model.DocumentNo != document.DocumentNo)
                                {
                                    bool isExist = _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data.Any(x => x.Status == DocumentStatus.Active && x.DocumentNo == model.DocumentNo);
                                    if (isExist)
                                        return CommandResult<EditDocumentViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.DocumentNumber), model);
                                }
                                //تعداد اقساط نمیتواند بیشتر از تعداد اقساطی باشد که برای آنها پرداخت ثبت شده است
                                int countOfInstallmentsHavePayment = document.Installments.Count(x => x.Payments.Any());
                                if (model.InstallmentCount < countOfInstallmentsHavePayment)
                                {
                                    return CommandResult<EditDocumentViewModel>.Failure(string.Format(UserMessages.TheInstallmentsCountMustBeMoreThan0, countOfInstallmentsHavePayment), model);
                                }
                                model.InstallmentAmount ??= string.Empty;
                                model.PrepaymentAmount ??= string.Empty;
                                model.RemainAmount ??= string.Empty;

                                long installmentAmount = long.Parse(model.InstallmentAmount.Replace(",", ""));
                                long prepaymentAmount = long.Parse(model.PrepaymentAmount.Replace(",", ""));
                                long remainAmount = long.Parse(model.RemainAmount.Replace(",", ""));


                                List<Installment> newInstallments = CalculateInstallment(model.InstallmentCount, model.DocumentDate, installmentAmount);
                                #region قسط یندی جدید

                                //حذف اقساط قبلی که پرداختی ندارند
                                foreach (var item in document.Installments)
                                {
                                    if (item.IsPaid)
                                    {
                                        //remove customer message
                                        var customerMessage = item.CustomerMessages.FirstOrDefault(x => x.Title == Captions.MessageTitle_Payment);
                                        if (customerMessage != null)
                                            _unitOfWork.CustomerMessageRepository.Delete(customerMessage);
                                    }
                                    if (!item.Payments.Any())
                                    {
                                        _unitOfWork.InstallmentRepository.Delete(item);
                                    }
                                }
                                foreach (var installment in newInstallments)
                                {
                                    var existInstallment = document.Installments.Where(x => x.Number == installment.Number).FirstOrDefault();

                                    if (existInstallment is not null)
                                    {
                                        //شماره قسط موجود در لیست اقساط
                                        //لغو پرداخت اقساطی که قبلا پرداخت شده
                                        existInstallment.Date = installment.Date;
                                        existInstallment.Amount = installment.Amount;
                                        existInstallment.DelayDays = null;
                                        existInstallment.Description = null;
                                        existInstallment.IsPaid = false;
                                        existInstallment.PaymentType = null;
                                        _unitOfWork.InstallmentRepository.Update(existInstallment);
                                    }
                                    else
                                    {
                                        //شماره قسط ناموجود در لیست اقساط
                                        var newInstallment = new Installment
                                        {
                                            DocumentId = model.DocumentId,
                                            Date = installment.Date,
                                            Amount = installment.Amount,
                                            Number = installment.Number,
                                        };
                                        await _unitOfWork.InstallmentRepository.InsertAsync(newInstallment, cancellationToken);
                                    }
                                }
                                #endregion

                                if (model.SellerId.HasValue)
                                {
                                    var sellerResult = await _unitOfWork.SellerRepository.GetByIdAsync(model.SellerId.Value, cancellationToken);
                                    if (!sellerResult.IsSuccess)
                                        return CommandResult<EditDocumentViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Seller), model);

                                    if (!sellerResult.Data.HasAccessToRegisterLoan)
                                        return CommandResult<EditDocumentViewModel>.Failure(UserMessages.SellerHasNotAccessToRegisterDocument, model);
                                }


                                document.InstallmentCount = model.InstallmentCount;
                                document.InstallmentAmount = installmentAmount;
                                document.PrepaymentAmount = prepaymentAmount;
                                document.RemainAmount = remainAmount;
                                document.DayOfMonth = byte.Parse(docDateArray[2]);
                                document.DocumentDate = docDate.Value;
                                document.GalleryId = model.GalleryId.Value;
                                document.SellerId = model.SellerId.Value;

                                model.OldDocumentNo = document.DocumentNo;
                                document.DocumentNo = model.DocumentNo;

                                _unitOfWork.DocumentRepository.Update(document);

                                var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                                if (updateResult.IsSuccess)
                                {
                                    transaction.Complete();
                                    return CommandResult<EditDocumentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                                }
                                else
                                    return CommandResult<EditDocumentViewModel>.Failure(updateResult.Message, model);
                            }
                            else
                                return CommandResult<EditDocumentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Document), model);
                        }
                        else
                            return CommandResult<EditDocumentViewModel>.Failure(UserMessages.OnlyCanEditActiveDocument, model);
                    }
                    else
                        return CommandResult<EditDocumentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Document), model);
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    await _logManager.RaiseLogAsync(ex, cancellationToken);
                    return CommandResult<EditDocumentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
                }
            }

        }

        private List<Installment> CalculateInstallment(byte installmentsCont, string documentDate, long installmentAmount)
        {
            List<Installment> installments = new List<Installment>();
            var installmentsDate = CalculateInstallmentDate(installmentsCont, documentDate);
            byte installmentNumber = 1;
            if (installmentsDate.IsSuccess)
            {
                installments = installmentsDate.Data.Select(x => new Installment
                {
                    Date = x.GorgianDate,
                    Amount = installmentAmount,
                    Number = installmentNumber++,
                    IsPaid = false,

                }).ToList();
            }

            return installments;
        }

        public CommandResult<IQueryable<DocumentModel>> GetDocumentListAsQuerable(SearchDocumentViewModel model)
        {
            try
            {
                var result = _unitOfWork.DocumentRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Include(x => x.Collaterals).ThenInclude(x => x.CollateralType)
                        .Include(c => c.Customer).ThenInclude(x => x.User)
                        .Include(i => i.Installments).ThenInclude(x => x.Payments)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(model.SettleDate))
                    {
                        DateTime? settleDate = model.SettleDate.ParsePersianToGorgian();
                        if (settleDate.HasValue)
                            query = query.Where(i => i.SettleDate.Value.Date == settleDate.Value);
                    }

                    if (!string.IsNullOrEmpty(model.Name))
                        query = query.Where(x => x.Customer.User.FullName.Contains(model.Name));

                    if (model.CollateralTypeId.HasValue)
                        query = query.Where(x => x.Collaterals.Any(y => y.CollateralTypeId == model.CollateralTypeId.Value));

                    if (!string.IsNullOrEmpty(model.CollateralDescription))
                        query = query.Where(x => x.Collaterals.Any(y => y.Description.Contains(model.CollateralDescription)));


                    if (!string.IsNullOrEmpty(model.Name))
                        query = query.Where(x => x.Customer.User.FullName.Contains(model.Name));

                    if (model.DocumentNumber.HasValue)
                        query = query.Where(x => x.DocumentNo == model.DocumentNumber.Value);

                    if (model.DocumentDay.HasValue)
                        query = query.Where(x => x.DayOfMonth == model.DocumentDay.Value);

                    if (model.DocumentStatus.HasValue)
                        query = query.Where(x => x.Status == model.DocumentStatus.Value);

                    if (model.GalleryId.HasValue)
                        query = query.Where(x => x.GalleryId == model.GalleryId.Value);

                    if (!string.IsNullOrEmpty(model.NotPayedInstallmentFromDate))
                    {
                        DateTime? notPayedInstallmentFromDate = model.NotPayedInstallmentFromDate.ParsePersianToGorgian();
                        if (notPayedInstallmentFromDate.HasValue)
                            query = query.Where(i => i.Installments.Any(x => x.Date >= notPayedInstallmentFromDate.Value & !x.IsPaid));
                    }

                    if (!string.IsNullOrEmpty(model.NotPayedInstallmentToDate))
                    {
                        DateTime? notPayedInstallmentToDate = model.NotPayedInstallmentToDate.ParsePersianToGorgian();

                        if (notPayedInstallmentToDate.HasValue)
                            query = query.Where(i => i.Installments.Any(x => x.Date <= notPayedInstallmentToDate.Value & !x.IsPaid));
                    }



                    if (!string.IsNullOrEmpty(model.InstallmentDate))
                    {
                        DateTime? iDate = DateTimeTools.ParsePersianToGorgianDateTime(model.InstallmentDate);
                        query = query.Where(i => i.Installments.Any(x => x.Date == iDate));
                    }



                    if (!string.IsNullOrEmpty(model.DocumentDate))
                    {
                        DateTime? docDate = model.DocumentDate.ParsePersianToGorgian();
                        if (docDate.HasValue)
                        {
                            query = query.Where(x => x.DocumentDate == docDate);
                        }
                    }

                    if (model.OverdueInstallmentsCount.HasValue)
                    {
                        query = query.Where(x => x.Installments.Count(i => !i.IsPaid & i.Date.Date < DateTime.Now.Date) == model.OverdueInstallmentsCount.Value);
                    }



                    IQueryable<DocumentModel> finallQuery = query.OrderBy(x => x.DocumentNo).OrderByDescending(x => x.DocumentDate).Select(x => new DocumentModel
                    {
                        Id = x.Id,
                        FullName = x.Customer.User.FullName,
                        AdminDescription = x.AdminDescription,
                        Collaterals = x.Collaterals.Select(c => new CollateralInfoModel { Type = c.CollateralType.Title, Description = c.Description }),
                        DocumentDate = x.DocumentDate,
                        DocumentNo = x.DocumentNo,
                        InstallmentAmount = x.InstallmentAmount,
                        Gallery = x.Gallery.Name,
                        IsDeletable = x.Status == DocumentStatus.Active,
                        RemainInstallmentCount = x.Installments.Count(i => !i.IsPaid),
                        RemainDueDateInstallmentCount = x.Installments.Count(i => !i.IsPaid & i.Date.Date < DateTime.Now.Date),
                        Status = x.Status.GetDisplayName(),
                        DocStatus = x.Status,
                        SettleDate = x.SettleDate
                    }).AsQueryable();

                    return CommandResult<IQueryable<DocumentModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<DocumentModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<DocumentModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        /// <summary>
        /// محاسبه جمع کل مانده در لیست اسناد
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<CommandResult<long>> GetDocumentSumOfAmount(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                long sumOfAmount = 0;
                var loanSettingResult = _settingService.GetSettingAsync<LoanSettingsViewModel>(SettingType.LoanSetting).Result.Data;

                decimal penaltyFactor = 0;
                if (loanSettingResult is not null)
                    decimal.TryParse(loanSettingResult.PenaltyFactor, out penaltyFactor);

                var installments = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.DocumentId == documentId)
                    .Include(x => x.Payments)
                    .Select(x => new Installment
                    {
                        Amount = x.Amount,
                        DelayDays = x.DelayDays,
                        Payments = x.Payments.Select(x => new Payment
                        {
                            Amount = x.Amount
                        }).ToList()
                    }).ToListAsync(cancellationToken);

                if (installments is not null)
                {
                    long sumOfInstallmentsAmount = 0;
                    long sumOfPaymentsAmount = 0;
                    int delayDays = 0;
                    foreach (var installment in installments)
                    {
                        foreach (var payment in installment.Payments)
                            sumOfPaymentsAmount += payment.Amount;

                        sumOfInstallmentsAmount += installment.Amount;
                        delayDays += installment.DelayDays.HasValue ? installment.DelayDays.Value : 0;
                    }
                    sumOfAmount = Convert.ToInt64(sumOfInstallmentsAmount + (delayDays * installments.First().Amount * penaltyFactor) - sumOfPaymentsAmount);
                }

                return CommandResult<long>.Success(OperationResultMessage.AnErrorHasOccurredInTheSoftware, sumOfAmount);

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }


        public List<SelectListItem> GetDocumentStatusSelectListItems(int selectedItem = 0)
        {
            List<DocumentStatus> list = EnumHelper<DocumentStatus>.GetAsList();
            return list.Select(item => new SelectListItem
            {
                Selected = ((int)item == selectedItem),
                Text = item.GetDisplayName(),
                Value = ((int)item).ToString()
            }).ToList();
        }

        public async Task<CommandResult<List<SelectListItem>>> GetCollateralsTypeSelectListItemsAsync(int selectedItem = 0, CancellationToken cancellationToken = default)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                var collateralsTypesResult = await _unitOfWork.CollateralTypeRepository.GetAllAsync(cancellationToken);
                if (collateralsTypesResult.IsSuccess)
                {
                    list = collateralsTypesResult.Data.Select(item => new SelectListItem
                    {
                        Selected = item.Id == selectedItem,
                        Text = item.Title,
                        Value = item.Id.ToString()
                    }).ToList();
                }
                return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }

        public async Task<CommandResult<DocumentDetailModel>> GetDocumentDetailAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                DocumentDetailModel? result = _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                     .Where(x => x.Id == id)
                     .Include(x => x.Customer).ThenInclude(x => x.User)
                     //.Include(x => x.Customer).ThenInclude(x => x.EssentialTels)
                     .Include(i => i.Installments)//.ThenInclude(x => x.Payments)
                                                  //.Include(x => x.Collaterals).ThenInclude(x => x.CollateralType)
                     .Include(x => x.Gallery)
                     .Include(x => x.Seller).ThenInclude(x => x.User)
                     .Select(x => new DocumentDetailModel
                     {
                         Id = x.Id,
                         FullName = x.Customer.User.FullName,
                         NationalCode = x.Customer.NationalCode,
                         DocumentDate = x.DocumentDate,
                         Mobile = x.Customer.User.Mobile,
                         EssentialTel = x.Customer.EssentialTels.OrderBy(x => x.OrderNo).Select(x => x.Tel).FirstOrDefault(),
                         DocumentNumber = x.DocumentNo,
                         PrePaymentAmount = x.PrepaymentAmount,
                         RemainAmount = x.RemainAmount,
                         GalleryName = x.Gallery.Name,
                         SellerName = x.Seller.User.FullName,
                         AdminDescription = x.AdminDescription,
                         InstallmentAmount = x.InstallmentAmount,
                         InstallmentCount = x.InstallmentCount,
                         DocumentStatus = x.Status,
                     }).FirstOrDefault();
                if (result is null)
                    return CommandResult<DocumentDetailModel>.Failure(string.Format(UserMessages.NotFound, Captions.Document), null);



                return CommandResult<DocumentDetailModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, result);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<DocumentDetailModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }


        /// <summary>
        /// محاسبه دیرکرد وام
        /// </summary>
        /// <param name="delayDays"></param>
        /// <param name="installmentAmount"></param>
        /// <returns></returns>
        private async Task<long> GetDelayDaysAmountAsync(int delayDays, long installmentAmount, bool isRoundDelayAmount)
        {
            //دیرکرد برابر است با ضریب دیرکرد * مبلغ قسط * مجموع روزهای دیرکرد 
            long result = 0;
            var loanSettingResult = await _settingService.GetSettingAsync<LoanSettingsViewModel>(SettingType.LoanSetting);


            if (loanSettingResult.IsSuccess)
            {
                if (decimal.TryParse(loanSettingResult.Data.PenaltyFactor, out decimal penaltyFactor))
                {
                    // result = (long)(installmentAmount * (penaltyFactor / 100) * delayDays);
                    result = (long)(installmentAmount * penaltyFactor * delayDays);
                }
            }

            if (result > 0 & isRoundDelayAmount)
            {
                result = NumberTools.RoundUpNumber(result.ToString(), 4);
            }
            return result;
        }

        public async Task<CommandResult<string>> GetDocumentDescriptionAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                var documentResult = await _unitOfWork.DocumentRepository.GetByIdAsync(documentId, cancellationToken);
                if (documentResult.IsSuccess)
                {
                    if (documentResult.Data.Status != DocumentStatus.Active)
                        return CommandResult<string>.Failure(UserMessages.OnlyCanEditAdminDescriptionInActiveDocument, string.Empty);
                    return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, documentResult.Data.AdminDescription);
                }
                else
                {
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.Document), string.Empty);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<EditDocumentDescriptionViewModel>> UpdateDocumentAdminDescriptionAsync(EditDocumentDescriptionViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var documentResult = await _unitOfWork.DocumentRepository.GetByIdAsync(model.Id, cancellationToken);
                if (!documentResult.IsSuccess)
                    return CommandResult<EditDocumentDescriptionViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Document), model);

                if (documentResult.Data.Status != DocumentStatus.Active)
                {
                    return CommandResult<EditDocumentDescriptionViewModel>.Failure(UserMessages.OnlyCanEditAdminDescriptionInActiveDocument, model);
                }

                documentResult.Data.AdminDescription = model.Description;
                model.DocumentNo = documentResult.Data.DocumentNo;

                _unitOfWork.DocumentRepository.Update(documentResult.Data);
                var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (updateResult.IsSuccess)
                    return CommandResult<EditDocumentDescriptionViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                else
                    return CommandResult<EditDocumentDescriptionViewModel>.Failure(updateResult.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EditDocumentDescriptionViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<InstallmentPymentsDetailModel>> GetInstallmentPaymentsAsync(long installmentId, CancellationToken cancellationToken)
        {
            try
            {
                InstallmentPymentsDetailModel? model = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == installmentId)
                    .OrderByDescending(x => x.Date)
                    .Include(x => x.Document)
                    .Include(x => x.Payments)
                    .Select(x => new InstallmentPymentsDetailModel
                    {
                        InstallmentId = x.Id,
                        DocumentNumber = x.Document.DocumentNo,
                        InstallmentAmount = x.Amount,
                        InstallmentCount = x.Document.InstallmentCount,
                        InstallmentDate = x.Date,
                        InstallmentNumber = x.Number,
                        Payments = x.Payments.Select(p => new PaymentModel
                        {
                            PaymentId = p.Id,
                            Amount = p.Amount,
                            InstallmentId = p.InstallmentId,
                            RegisterDate = p.RegisterDate,
                            ImageName = p.ImageName,
                            ImageUrl = string.Empty,//to do =>create image link
                            PaymentDate = p.Date
                        }),
                    }).FirstOrDefaultAsync();
                if (model is not null)
                {
                    model.SumOfRemainAmount = GetSumOfRemainAmountToInstallment(installmentId, 0, cancellationToken).Result.Data;
                    return CommandResult<InstallmentPymentsDetailModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<InstallmentPymentsDetailModel>.Failure(string.Format(UserMessages.NotFound, Captions.Installment), null);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<InstallmentPymentsDetailModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<CreateOrEditCollateralViewModel>> GetCollateralForEditAsync(long documentId, long collateralId, CancellationToken cancellationToken)
        {
            try
            {
                CreateOrEditCollateralViewModel? model = await _unitOfWork.CollateralRepository.GetAllAsIQueryable().Data
                    .Where(x => x.DocumentId == documentId & x.Id == collateralId)
                    .Select(x => new CreateOrEditCollateralViewModel
                    {
                        CollateralTypeId = x.CollateralTypeId,
                        CollateralId = x.Id,
                        DocumentId = x.DocumentId,
                        Description = x.Description,
                        ImageUrl = $"{ImageAddress.CollateralImageUrl}{x.ImageName}",
                        ImageName = x.ImageName
                    }).FirstOrDefaultAsync();
                if (model is not null)
                {
                    var collateralsTypes = await GetCollateralTypesAsync(model.CollateralTypeId.Value, cancellationToken);
                    model.CollateralTypes = collateralsTypes.Data;
                    return CommandResult<CreateOrEditCollateralViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Collateral), null);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditCollateralViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<List<CollateralModel>> GetCollaterals(long documentId)
        {
            try
            {
                List<CollateralModel> collaterals = _unitOfWork.CollateralRepository.GetAllAsIQueryable().Data
                    .Include(x => x.CollateralType)
                    .Where(x => x.DocumentId == documentId).Select(x => new CollateralModel
                    {
                        Id = x.Id,
                        DocumentId = x.DocumentId,
                        Description = x.Description,
                        Type = x.CollateralType.Title,
                        ImageName = x.ImageName
                    }).ToList();
                return CommandResult<List<CollateralModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, collaterals);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<CollateralModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<CreateOrEditCollateralViewModel>> UpdateCollateralAsync(CreateOrEditCollateralViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                //ضمانت سند تنها در حالتی ممکن است که سند فعال باشد
                var documentResult = await _unitOfWork.DocumentRepository.GetByIdAsync(model.DocumentId, cancellationToken);
                if (documentResult.IsSuccess)
                {
                    if (documentResult.Data.Status != DocumentStatus.Active)
                    {
                        return CommandResult<CreateOrEditCollateralViewModel>.Failure(UserMessages.OnlyCanEditOrAddCollateralForActiveDocument, model);
                    }
                }
                else
                {
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.Document), model);
                }

                var colateralTypeResult = await _unitOfWork.CollateralTypeRepository.GetByIdAsync(model.CollateralTypeId.Value, cancellationToken);
                if (!colateralTypeResult.IsSuccess)
                {
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.CollateralType), model);
                }

                var colateral = await _unitOfWork.CollateralRepository.GetByIdAsync(model.CollateralId, cancellationToken);
                if (!colateral.IsSuccess)
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(colateral.Message, model);

                colateral.Data.Description = model.Description;
                colateral.Data.CollateralTypeId = colateralTypeResult.Data.Id;
                if (model.ImageFile is not null)
                {
                    //update image
                    string newFileName = Guid.NewGuid().ToString();
                    var updateFileResult = await _fileService.UpdateFileAsync(model.ImageFile, newFileName, colateral.Data.ImageName, _filePathAddress.CollateralsDocs);
                    colateral.Data.ImageName = $"{newFileName}{Path.GetExtension(model.ImageFile.FileName)}";
                }
                else
                {
                    if (model.IsDeleteImage)
                        if (_fileService.DeleteFile(colateral.Data.ImageName, _filePathAddress.CollateralsDocs))
                            colateral.Data.ImageName = null;
                        else
                            return CommandResult<CreateOrEditCollateralViewModel>.Failure(UserMessages.ErrorInDeleteFile, model);
                }
                _unitOfWork.CollateralRepository.Update(colateral.Data);
                var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (updateResult.IsSuccess)
                {
                    model.DocumentNo = documentResult.Data.DocumentNo;
                    model.CollateralTypeTitle = colateralTypeResult.Data.Title;

                    return CommandResult<CreateOrEditCollateralViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(updateResult.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditCollateralViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<CreateOrEditCollateralViewModel>> AddCollateralAsync(CreateOrEditCollateralViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                //ضمانت سند تنها در حالتی ممکن است که سند فعال باشد
                var documentResult = await _unitOfWork.DocumentRepository.GetByIdAsync(model.DocumentId, cancellationToken);
                if (documentResult.IsSuccess)
                {
                    if (documentResult.Data.Status != DocumentStatus.Active)
                    {
                        return CommandResult<CreateOrEditCollateralViewModel>.Failure(UserMessages.OnlyCanEditOrAddCollateralForActiveDocument, model);
                    }
                }
                else
                {
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.Document), model);
                }
                var colateralTypeResult = await _unitOfWork.CollateralTypeRepository.GetByIdAsync(model.CollateralTypeId.Value, cancellationToken);
                if (!colateralTypeResult.IsSuccess)
                {
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.CollateralType), model);
                }

                var item = new Collateral
                {
                    CollateralTypeId = model.CollateralTypeId.Value,
                    Description = model.Description,
                    DocumentId = model.DocumentId,
                    Id = model.CollateralId,
                };
                if (model.ImageFile is not null)
                {
                    //update image
                    string newFileName = Guid.NewGuid().ToString();
                    var updateFileResult = await _fileService.UploadFileAsync(model.ImageFile, _filePathAddress.CollateralsDocs, newFileName);
                    item.ImageName = $"{newFileName}{Path.GetExtension(model.ImageFile.FileName)}";
                }

                await _unitOfWork.CollateralRepository.InsertAsync(item, cancellationToken);
                var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (result.IsSuccess)
                {
                    model.DocumentNo = documentResult.Data.DocumentNo;
                    model.CollateralTypeTitle = colateralTypeResult.Data.Title;

                    return CommandResult<CreateOrEditCollateralViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                }
                else
                    return CommandResult<CreateOrEditCollateralViewModel>.Failure(result.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditCollateralViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<DocumentStatus>> GetDocumentStatusByIdAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                DocumentStatus? documentStatus = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                     .Where(x => x.Id == documentId)
                     .Select(x => x.Status)
                     .SingleOrDefaultAsync();
                if (documentStatus is null)
                {
                    return CommandResult<DocumentStatus>.Failure(string.Format(UserMessages.NotFound, Captions.Document), 0);
                }
                else
                {
                    return CommandResult<DocumentStatus>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, documentStatus.Value);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<DocumentStatus>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<string>> RemoveCollateralAsync(long id, long documentId, CancellationToken cancellationToken)
        {
            try
            {
                int count = _unitOfWork.CollateralRepository.GetAllAsIQueryable().Data
                    .Count(x => x.DocumentId == documentId);
                if (count > 1)
                {
                    //only can remove if count of Collateral > 1
                    var itemResult = await _unitOfWork.CollateralRepository.GetByIdAsync(id, cancellationToken);
                    if (itemResult.IsSuccess)
                    {
                        //حذف ضمانت سند تنها در حالتی ممکن است که سند فعال باشد
                        CommandResult<DocumentStatus> documentStatusResult = await GetDocumentStatusByIdAsync(documentId, cancellationToken);
                        if (documentStatusResult.IsSuccess)
                        {
                            if (documentStatusResult.Data != DocumentStatus.Active)
                            {
                                return CommandResult<string>.Failure(UserMessages.OnlyCanDeleteCollateralForActiveDocument, string.Empty);
                            }
                        }
                        else
                        {
                            return CommandResult<string>.Failure(String.Format(UserMessages.NotFound, Captions.Document), string.Empty);
                        }

                        //remove Collateral
                        _unitOfWork.CollateralRepository.Delete(itemResult.Data);
                        var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                        if (removeResult.IsSuccess)
                        {
                            //remove image
                            _fileService.DeleteFile(itemResult.Data.ImageName, _filePathAddress.CollateralsDocs);

                            var colateralTypeTypeTitleResult = await _unitOfWork.CollateralTypeRepository.GetByIdAsync(itemResult.Data.CollateralTypeId, cancellationToken);

                            return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, colateralTypeTypeTitleResult.Data.Title);
                        }
                        else
                            return CommandResult<string>.Failure(removeResult.Message, string.Empty);
                    }
                    else
                        return CommandResult<string>.Failure(itemResult.Message, string.Empty);

                }
                else if (count == 1)
                    return CommandResult<string>.Failure(UserMessages.AtLeastOneCollateralIsRequiredForTheDocument, string.Empty);
                else
                    return CommandResult<string>.Failure(UserMessages.ThisDocumentHaventACollateral, string.Empty);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<CreateOrEditPaymentViewModel>> GetPaymentForEditAsync(long paymentId, long instllmentId, CancellationToken cancellationToken)
        {
            try
            {

                CreateOrEditPaymentViewModel? payment = await _unitOfWork.PaymentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == paymentId)
                    .Include(x => x.Installment).ThenInclude(x => x.Document)
                    .Select(x => new CreateOrEditPaymentViewModel
                    {
                        PaymentId = x.Id,
                        InstallmentId = x.InstallmentId,
                        PersianPaymentDate = x.Date.GeorgianToPersian(ShowMode.OnlyDate),
                        DelayDay = x.Installment.DelayDays,
                        Description = x.Installment.Description,
                        DocumentNumber = x.Installment.Document.DocumentNo,
                        InstallmentAmount = x.Installment.Amount,
                        InstallmentCount = x.Installment.Document.InstallmentCount,
                        InstallmentDate = x.Installment.Date,
                        InstallmentNumber = x.Installment.Number,
                        IsPayInstallment = x.Installment.IsPaid,
                        PaymentAmount = x.Amount.ToString("N0"),
                        PaymentType = x.Installment.PaymentType,
                        ImageUrl = x.ImageName.IsEmptyOrNull() ? string.Empty : $"{ImageAddress.PaymentImageUrl}{x.ImageName}"
                    }).FirstOrDefaultAsync();


                if (payment is null)
                    return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.PaymentDetail), null);
                //payment.PaymentTypes = GetPamentTypesSelectListItem(payment.PaymentType);
                var paymentsResult = await GetPaymentsByInstallmentIdAsync(instllmentId, cancellationToken);
                if (paymentsResult.IsSuccess)
                {
                    payment.Payments = paymentsResult.Data;
                }


                var result = await CalculationInstallmentDelayInfoAsync(payment.InstallmentId, payment.PaymentId, payment.PersianPaymentDate, cancellationToken);
                if (result.IsSuccess)
                    payment.DelayDay = result.Data.CurrentInstallmentDelayDay;

                //long paymentAmount = 0;
                //if (!string.IsNullOrEmpty(payment.PaymentAmount))
                //{
                //    long.TryParse(payment.PaymentAmount.Replace(",", ""),out paymentAmount);
                //}
                //payment.DelayDay ??= 0;
                //var paymentMessageResult = await GeneratePaymentDescriptionWithMessageAsync(payment.InstallmentId, paymentId, paymentAmount, payment.DelayDay.Value, cancellationToken);
                //if (paymentMessageResult.IsSuccess)
                //    payment.CustomerMessageContent = paymentMessageResult.Data.Message;

                payment.SumOfRemainAmount = GetSumOfRemainAmountToInstallment(instllmentId, 0, cancellationToken).Result.Data;

                return CommandResult<CreateOrEditPaymentViewModel>.Success(OperationResultMessage.OperationIsFailureInUploadFile, payment);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditPaymentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<List<PaymentModel>>> GetPaymentsByInstallmentIdAsync(long instllmentId, CancellationToken cancellationToken)
        {
            try
            {
                List<PaymentModel>? payments = await _unitOfWork.PaymentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.InstallmentId == instllmentId)
                    //.Include(x=>x.pa)
                    .Select(x => new PaymentModel
                    {
                        PaymentId = x.Id,
                        InstallmentId = x.InstallmentId,
                        Amount = x.Amount,
                        ImageName = x.ImageName,
                        ImageUrl = $"{ImageAddress.PaymentImageUrl}{x.ImageName}",
                        PaymentDate = x.Date,
                        RegisterDate = x.RegisterDate
                    }).ToListAsync(cancellationToken);

                return CommandResult<List<PaymentModel>>.Success(OperationResultMessage.OperationIsFailureInUploadFile, payments);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<PaymentModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public List<SelectListItem> GetPamentTypesSelectListItem(PaymentType? selectedItem)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            var list = EnumHelper<PaymentType>.GetAsList();
            if (list is not null)
            {
                return list.Select(x => new SelectListItem
                {
                    Selected = (selectedItem == null ? false : x == selectedItem),
                    Text = x.GetDisplayName(),
                    Value = ((byte)x).ToString()
                }).ToList();
            }
            return selectListItems;
        }

        public async Task<CommandResult<CreateOrEditPaymentViewModel>> CreateOrEditPaymentAsync(CreateOrEditPaymentViewModel model, CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //برای پرداخت باید سند فعال باشد
                    // CommandResult<DocumentStatus> docStatusResult = await _unitOfWork.DocumentRepository.GetDocumentStatusByInstallmentIdAsync(model.InstallmentId, cancellationToken);
                    var document = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Id == model.InstallmentId)
                        .Include(x => x.Document)
                        .Select(x => x.Document)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (document == null)
                    {
                        return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Document), model);
                    }
                    else
                    {


                        long paymentamount = 0;
                        if (!long.TryParse(model.PaymentAmount.Replace(",", "").ToString(), out paymentamount))
                        {
                            //invalid amount
                            return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.PaymentAmount), model);
                        }
                        if (document.Status != DocumentStatus.Active)
                        {
                            return CommandResult<CreateOrEditPaymentViewModel>.Failure(UserMessages.ItIsNotPossibleToPerformThisOperation, model);
                        }
                        var installmentIsPayResult = await IsPayInstallmentAsync(model.InstallmentId, cancellationToken);
                        if (installmentIsPayResult.Data)
                        {
                            return CommandResult<CreateOrEditPaymentViewModel>.Failure(UserMessages.TheInstallmentIsPayed, model);
                        }



                        var installmentResult = await _unitOfWork.InstallmentRepository.GetByIdAsync(model.InstallmentId, cancellationToken);

                        string? customerName = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                                .Where(x => x.Id == document.Id)
                                .Include(x => x.Customer).ThenInclude(x => x.User)
                                .Select(x => x.Customer.User.FullName)
                                .FirstOrDefaultAsync(cancellationToken);

                        DateTime? paymentDate = model.PersianPaymentDate.ParsePersianToGorgian();
                        if (paymentDate.HasValue)
                        {
                            //invalid paymentDate
                            if (paymentDate.Value.Date > DateTime.Now.Date)
                                return CommandResult<CreateOrEditPaymentViewModel>.Failure(UserMessages.PaymentDateCanNotMorThanTodayDate, model);

                        }
                        else
                        {
                            //invalid paymentDate
                            return CommandResult<CreateOrEditPaymentViewModel>.Failure(String.Format(ValidationMessages.Invalid, Captions.PaymentDate), model);
                        }

                        if (model.IsPayInstallment)
                        {
                            if (model.DelayDay.HasValue)
                            {
                                if (model.DelayDay < 0)
                                    return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Delay), model);
                            }
                            else
                                return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Delay), model);

                            //if (model.PaymentType == null)
                            //	return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.PaymentType), model);
                            if (string.IsNullOrEmpty(model.CustomerMessageContent))
                                return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.CustomerMessageContent), model);

                            #region set paymentType status
                            var accountStatusResult = await GetSumOfRemainAmountToInstallment(model.InstallmentId, model.PaymentId, cancellationToken);
                            //تعیین وضعیت پرداخت قسط
                            if (accountStatusResult.Data - paymentamount > 0)
                                model.PaymentType = PaymentType.OverPayment;
                            else if (accountStatusResult.Data - paymentamount < 0)
                                model.PaymentType = PaymentType.DeficitPayment;
                            else
                                model.PaymentType = PaymentType.FullPayment;
                            #endregion

                            //برای پرداخت باید اولین قسط پرداخت نشده باشد
                            var paymantableResult = await IsPayableInstallmentAsync(model.InstallmentId, cancellationToken);
                            if (!paymantableResult.Data)
                                return CommandResult<CreateOrEditPaymentViewModel>.Failure(paymantableResult.Message, model);

                            //چک کردن تاریخ پرداخت برای قسط فقط در حالتی که پرداخت برای تسویه قسط میباشد
                            var paymentDateResultCheack = await IsCheackPaymentDateAsync(model.InstallmentId, model.PaymentId, paymentDate.Value, cancellationToken);
                            if (paymentDateResultCheack.IsSuccess)
                            {
                                if (!paymentDateResultCheack.Data)
                                    return CommandResult<CreateOrEditPaymentViewModel>.Failure(paymentDateResultCheack.Message, model);
                            }
                            else
                                return CommandResult<CreateOrEditPaymentViewModel>.Failure(paymentDateResultCheack.Message, model);


                            if (installmentResult.IsSuccess)
                            {
                                installmentResult.Data.DelayDays = model.DelayDay;
                                installmentResult.Data.Description = model.Description;
                                installmentResult.Data.PaymentType = model.PaymentType;
                                installmentResult.Data.IsPaid = model.IsPayInstallment;

                                //update installment
                                _unitOfWork.InstallmentRepository.Update(installmentResult.Data);

                                #region log
                                //log
                                var logParams = new Dictionary<string, string>();
                                var logModel = model.LogUserActivity;

                                if (model.PaymentId <= 0)
                                {
                                    logParams.Add("0", installmentResult.Data.Number.ToString());
                                    logParams.Add("1", document.DocumentNo.ToString());
                                    logParams.Add("2", customerName);
                                    logParams.Add("3", model.PaymentAmount);

                                    logModel.ActivityType = AdminActivityType.Insert;
                                    logModel.DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Insert_InstallmentChangedToPaid;
                                }
                                else
                                {
                                    logParams.Add("0", model.PaymentAmount);
                                    logParams.Add("1", installmentResult.Data.Number.ToString());
                                    logParams.Add("2", document.DocumentNo.ToString());
                                    logParams.Add("3", customerName);

                                    logModel.ActivityType = AdminActivityType.Edit;
                                    logModel.DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit_InstallmentChangedToPaid;
                                }
                                logModel.Parameters = logParams;
                                _userService.LogUserActivityAsync(logModel, cancellationToken, false);
                                #endregion

                            }
                            else
                            {
                                //installment not founded
                                return CommandResult<CreateOrEditPaymentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Installment), model);
                            }
                        }





                        string? imageName = null;

                        if (model.PaymentId == 0)
                        {
                            //add
                            if (model.ImageFile is not null)
                            {
                                //upload image
                                imageName = imageName = Guid.NewGuid().ToString();
                                bool isSuccesUlpoad = await _fileService.UploadFileAsync(model.ImageFile, _filePathAddress.PaymentImages, imageName);
                                if (isSuccesUlpoad)
                                    imageName = $"{imageName}{Path.GetExtension(model.ImageFile.FileName)}";
                                else
                                    return CommandResult<CreateOrEditPaymentViewModel>.Failure(OperationResultMessage.OperationIsFailureInUploadFile, model);
                            }

                            Payment newPayment = new()
                            {
                                InstallmentId = model.InstallmentId,
                                Amount = paymentamount,
                                Date = paymentDate.Value,
                                ImageName = imageName,
                                RegisterDate = DateTime.Now,
                            };
                            await _unitOfWork.PaymentRepository.InsertAsync(newPayment, cancellationToken);

                            //log for add
                            #region log activity
                            var logParams = new Dictionary<string, string>();
                            logParams.Add("0", installmentResult.Data.Number.ToString());
                            logParams.Add("1", document.DocumentNo.ToString());
                            logParams.Add("2", customerName);
                            logParams.Add("3", model.PaymentAmount);
                            var logAddPayment = model.LogUserActivity;

                            logAddPayment.ActivityType = AdminActivityType.Insert;
                            logAddPayment.Parameters = logParams;
                            logAddPayment.DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Insert_PaymentOfInstallment;

                            _userService.LogUserActivityAsync(logAddPayment, cancellationToken, false);
                            #endregion
                        }
                        else
                        {
                            //edit
                            var paymentResult = await _unitOfWork.PaymentRepository.GetAllAsIQueryable()
                                .Data.FirstOrDefaultAsync(x => x.Id == model.PaymentId & x.InstallmentId == model.InstallmentId, cancellationToken);
                            if (paymentResult is null)
                            {
                                //payment is not founded
                                return CommandResult<CreateOrEditPaymentViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.PaymentDetail), model);
                            }
                            if (model.ImageFile is not null)
                            {
                                //update image
                                imageName = Guid.NewGuid().ToString();
                                bool isSuccessUpadteImage = await _fileService.UpdateFileAsync(model.ImageFile, imageName, paymentResult.ImageName, _filePathAddress.PaymentImages);
                                if (isSuccessUpadteImage)
                                    paymentResult.ImageName = $"{imageName}{Path.GetExtension(model.ImageFile.FileName)}";
                                else
                                    return CommandResult<CreateOrEditPaymentViewModel>.Failure(OperationResultMessage.OperationIsFailureInUploadFile, model);

                            }
                            else
                            {
                                if (model.IsDeletePaymentImage)
                                    if (_fileService.DeleteFile(paymentResult.ImageName, _filePathAddress.PaymentImages))
                                        paymentResult.ImageName = null;
                                    else
                                        return CommandResult<CreateOrEditPaymentViewModel>.Failure(UserMessages.ErrorInDeleteFile, model);
                            }
                            paymentResult.Amount = paymentamount;
                            paymentResult.Date = paymentDate.Value;


                            _unitOfWork.PaymentRepository.Update(paymentResult);

                            //log for add
                            #region log activity
                            var logParams = new Dictionary<string, string>();
                            logParams.Add("0", model.PaymentAmount);
                            logParams.Add("1", installmentResult.Data.Number.ToString());
                            logParams.Add("2", document.DocumentNo.ToString());
                            logParams.Add("3", customerName);
                            var logEditPayment = model.LogUserActivity;

                            logEditPayment.ActivityType = AdminActivityType.Insert;
                            logEditPayment.Parameters = logParams;
                            logEditPayment.DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit_PaymentOfInstallment;
                            _userService.LogUserActivityAsync(logEditPayment, cancellationToken, false);
                            #endregion
                        }

                        var saveChangeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveChangeResult.IsSuccess)
                        {

                            var paymentsResult = await GetPaymentsByInstallmentIdAsync(model.InstallmentId, cancellationToken);
                            if (paymentsResult.IsSuccess)
                                model.Payments = paymentsResult.Data;

                            if (model.IsPayInstallment)
                            {
                                #region send message and sms for customer
                                var message = new CustomerMessage
                                {
                                    CustomerId = document.CustomerId,
                                    DocumentId = document.Id,
                                    installmentId = model.InstallmentId,
                                    Message = model.CustomerMessageContent,
                                    Date = DateTime.Now,
                                };
                                switch (model.PaymentType.Value)
                                {
                                    case PaymentType.FullPayment:
                                        {
                                            message.Type = CustomerMessageType.FullPayment;
                                            message.Title = Captions.FullPayment;
                                            break;
                                        }
                                    case PaymentType.OverPayment:
                                        {
                                            message.Type = CustomerMessageType.OverPayment;
                                            message.Title = Captions.OverPayment;
                                            break;
                                        }
                                    case PaymentType.DeficitPayment:
                                        {
                                            message.Type = CustomerMessageType.DeficitPayment;
                                            message.Title = Captions.DeficitPayment;
                                            break;
                                        }
                                }

                                await _unitOfWork.CustomerMessageRepository.InsertAsync(message, cancellationToken);
                                var sendMessageResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                                #endregion
                            }

                            transaction.Complete();
                            return CommandResult<CreateOrEditPaymentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        }
                        else
                            return CommandResult<CreateOrEditPaymentViewModel>.Failure(saveChangeResult.Message, model);
                    }

                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    await _logManager.RaiseLogAsync(ex, cancellationToken);

                    return CommandResult<CreateOrEditPaymentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
                }
            }

        }

        public async Task<CommandResult<InstallmentDetailModel>> GetInstallmentDetailAsync(long installmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                InstallmentDetailModel? model = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == installmentId)
                    .Include(x => x.Document)
                    .Select(x => new InstallmentDetailModel
                    {
                        InstallmentId = x.Id,
                        DocumentNumber = x.Document.DocumentNo,
                        InstallmentAmount = x.Amount,
                        InstallmentCount = x.Document.InstallmentCount,
                        InstallmentDate = x.Date,
                        InstallmentNumber = x.Number,
                        IsPayInstallment = x.IsPaid
                    })
                    .FirstOrDefaultAsync(cancellationToken);
                if (model is null)
                    return CommandResult<InstallmentDetailModel>.Failure(string.Format(UserMessages.NotFound, Captions.Installment), null);
                else
                    return CommandResult<InstallmentDetailModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<InstallmentDetailModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<List<InstallmentModel>>> GetInstallmentsAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                var installmentList = new List<InstallmentModel>();
                var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId)
                    .Include(x => x.Installments).ThenInclude(x => x.Payments)
                    .SingleOrDefaultAsync(cancellationToken);
                if (document != null)
                {
                    installmentList = document.Installments.Select(x => new InstallmentModel
                    {
                        Id = x.Id,
                        Date = x.Date,
                        DelayDays = x.DelayDays,
                        Description = x.Description,
                        IsPaid = x.IsPaid,
                        PaymentAmount = x.Payments.Any() ? x.Payments.Sum(x => x.Amount) : 0,
                        PaymentDate = x.Payments.Any() ? x.Payments.OrderBy(x => x.Date).Last().Date : null,
                        DocumentStatus = document.Status
                    }).ToList();
                }

                return CommandResult<List<InstallmentModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, installmentList);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<List<InstallmentModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<List<CollateralInfoModel>>> GetCollateralInfoAsync(long documentId, CancellationToken cancellationToken)
        {
            var collateralList = new List<CollateralInfoModel>();
            try
            {
                var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId)
                    .Include(x => x.Collaterals).ThenInclude(x => x.CollateralType)
                    .SingleOrDefaultAsync(cancellationToken);
                if (document != null)
                {
                    collateralList = document.Collaterals.Select(x => new CollateralInfoModel
                    {
                        Type = x.CollateralType.Title,
                        Description = x.Description
                    }).ToList();
                }

                return CommandResult<List<CollateralInfoModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, collateralList);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<List<CollateralInfoModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, collateralList);
            }
        }

        public async Task<CommandResult<InstallmentInfo>> GetInstallmentsInfoAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {

                InstallmentInfo installmentInfo = new();
                var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId)
                    .Include(x => x.Installments).ThenInclude(x => x.Payments).FirstOrDefaultAsync(cancellationToken);

                if (document is not null)
                {

                    var loanSetting = await _settingService.GetSettingAsync<LoanSettingsViewModel>(Domain.Enums.SettingType.LoanSetting, cancellationToken);
                    if (!loanSetting.IsSuccess)
                        return CommandResult<InstallmentInfo>.Failure(UserMessages.LoanSettingsNotRegisterd, installmentInfo);

                    installmentInfo.DocumentStatus = document.Status;
                    installmentInfo.SettleDate = document.SettleDate;
                    installmentInfo.DeliveryDate = document.DeliveryDate;
                    installmentInfo.DiscountAmount = document.DiscountAmount;
                    installmentInfo.ReturnedAmount = document.ReturnedAmount.HasValue ? document.ReturnedAmount.Value : 0;

                    //int delayDays = 0;
                    if (document.Installments.Any())
                    {
                        installmentInfo.TotalDelayDay = document.Installments.Where(x => x.DelayDays != null).Sum(x => x.DelayDays.Value);
                        installmentInfo.TodayTotalDelayDay = installmentInfo.TotalDelayDay;

                        foreach (var item in document.Installments)
                        {
                            if (!item.IsPaid & item.Date.Date < DateTime.Now.Date & document.Status == DocumentStatus.Active)
                                installmentInfo.TodayTotalDelayDay += (int)Math.Abs((DateTime.Now.Date - item.Date.Date).TotalDays);

                            if (item.Payments.Any())
                                installmentInfo.TotalPayedAmount += item.Payments.Sum(x => x.Amount);
                        }
                        if (decimal.TryParse(loanSetting.Data.PenaltyFactor, out decimal penaltyFactor))
                        {
                            //installmentInfo.DelayAmount = (long)Math.Ceiling(installmentInfo.TotalDelayDay * penaltyFactor * document.InstallmentAmount);
                            installmentInfo.DelayAmount = await GetDelayDaysAmountAsync(installmentInfo.TotalDelayDay, document.InstallmentAmount, true);
                            //installmentInfo.TotalRemainAmount = (document.InstallmentCount * document.InstallmentAmount) + installmentInfo.DelayAmount - installmentInfo.TotalPayedAmount + installmentInfo.ReturnedAmount - installmentInfo.DiscountAmount;
                            installmentInfo.TotalRemainAmount = CalculateSumOfPayAmountInSettleDocument(document.InstallmentCount * document.InstallmentAmount, installmentInfo.DelayAmount, installmentInfo.TotalPayedAmount, installmentInfo.DiscountAmount, installmentInfo.ReturnedAmount);
                            if (document.Status == DocumentStatus.Active)
                            {
                                //installmentInfo.TodayDelayAmount = (long)Math.Ceiling(installmentInfo.TodayTotalDelayDay * penaltyFactor * document.InstallmentAmount);
                                installmentInfo.TodayDelayAmount = await GetDelayDaysAmountAsync(installmentInfo.TodayTotalDelayDay, document.InstallmentAmount, false);
                                //installmentInfo.TodayTotalRemainAmount = (document.InstallmentCount * document.InstallmentAmount) + installmentInfo.TodayDelayAmount - installmentInfo.TotalPayedAmount + installmentInfo.ReturnedAmount - installmentInfo.DiscountAmount;
                                installmentInfo.TodayTotalRemainAmount = CalculateSumOfPayAmountInSettleDocument(document.InstallmentCount * document.InstallmentAmount, installmentInfo.TodayDelayAmount, installmentInfo.TotalPayedAmount, installmentInfo.DiscountAmount, installmentInfo.ReturnedAmount);
                            }
                            else
                            {
                                installmentInfo.TodayDelayAmount = installmentInfo.DelayAmount;
                                installmentInfo.TodayTotalRemainAmount = installmentInfo.TotalRemainAmount;
                            }

                        }
                        if (document.Status == DocumentStatus.Active)
                            installmentInfo.InstantSettlementAmount = CalculationInstantSettlementAmount(document.Installments.Sum(x => x.Amount), NumberTools.RoundUpNumber(installmentInfo.TodayDelayAmount.ToString(), 4), installmentInfo.TotalPayedAmount);
                        else
                            installmentInfo.InstantSettlementAmount = 0;
                        installmentInfo.InstallmentRemainAmount = document.Installments.Sum(x => x.Amount) - installmentInfo.TotalPayedAmount;

                    }
                }

                return CommandResult<InstallmentInfo>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, installmentInfo);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<InstallmentInfo>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }



        /// <summary>
        /// محاسبه مبلغ مانده زمان تسویه-این مبلغ باید زمان تسویه صفر باشد
        /// </summary>
        /// <param name="totalInstallmentsAmount">جمع مبلغ اقساط</param>
        /// <param name="totalDelayAmount">جمع مبلغ دیرکرد</param>
        /// <param name="totalPayedAmount">جمع پرداخت ها</param>
        /// <param name="discountAmount">مبلغ تخفیف</param>
        /// <param name="returnedAmount">مبلغ بازگشت خورده</param>
        /// <returns>مانده کل</returns>
        private long CalculateSumOfPayAmountInSettleDocument(long totalInstallmentsAmount, long totalDelayAmount, long totalPayedAmount, long discountAmount, long returnedAmount)
        {
            return totalInstallmentsAmount + totalDelayAmount - totalPayedAmount - discountAmount + returnedAmount;
        }

        /// <summary>
        /// محاسبه مبلغ تسویه فوری وام
        /// </summary>
        /// <param name="sumOfInstallmentsAmount">مجموع مبلغ اقساط</param>
        /// <param name="todayTotalDelayAmount">مجموع مبلغ دیرکرد تا امروز</param>
        /// <param name="totalPayedAmount">مجموع پرداخت های وام توسط مشتری</param>
        /// <returns>اگر نتیجه منفی باشد، مبلغ تسویه فوری صفر است</returns>
        private long CalculationInstantSettlementAmount(long sumOfInstallmentsAmount, long todayTotalDelayAmount, long totalPayedAmount)
        {
            long result = sumOfInstallmentsAmount + todayTotalDelayAmount - totalPayedAmount;
            return result;
        }

        /// <summary>
        /// محاسبه مبلغ مانده قسط
        /// </summary>
        /// <param name="installmentCount">تعداد اقساط وام</param>
        /// <param name="installmentAmount">مبلغ اقساط</param>
        /// <param name="sumOfConfirmedPaymentedAmount">مجموع مبالغ تایید شده</param>
        /// <param name="installmentDelayAmount">مبلغ دیرکرد</param>
        /// <param name="discountAmount">مبلغ تخفیف</param>
        /// <returns></returns>
        private long CalculationInstallmentRemainAmount(int installmentCount, long installmentAmount, long sumOfConfirmedPaymentedAmount
            , long installmentDelayAmount, long discountAmount, long? returnedAmount)
        {
            returnedAmount ??= 0;

            return (installmentCount * installmentAmount) + installmentDelayAmount - sumOfConfirmedPaymentedAmount - returnedAmount.Value - discountAmount;
        }
        /// <summary>
        /// محاسبه مبلغ دیرکرد
        /// </summary>
        /// <param name="delayDays">مجموع روز های دیرکرد در وام</param>
        /// <param name="penaltyFactor">ضریب دیرکرد</param>
        /// <param name="installmentAmount">مبلغ اقساط</param>
        /// <returns></returns>
        //private long CalculationDelayAmount(int delayDays, decimal penaltyFactor, long installmentAmount)
        //{
        //	//return (long)((penaltyFactor / 100) * delayDays * installmentAmount);
        //	return (long)(penaltyFactor * delayDays * installmentAmount);
        //}

        /// <summary>
        /// محسابه مجموع روزهای دیرکرد اقساط
        /// </summary>
        /// <param name="installments"></param>
        /// <returns></returns>
        //private int CalculationDelayDays(List<Installment> installments)
        //{
        //	int delayDays = 0;
        //	foreach (var item in installments)
        //	{
        //		if (item.DelayDays.HasValue)
        //		{
        //			delayDays += item.DelayDays.Value;
        //		}
        //	}
        //	return delayDays;
        //}

        public async Task<CommandResult<bool>> IsPayInstallmentAsync(long installmentId, CancellationToken cancellationToken)
        {
            try
            {
                bool isPayStatus = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                     .Where(x => x.Id == installmentId)
                     .Select(x => x.IsPaid)
                     .FirstOrDefaultAsync(cancellationToken);
                return CommandResult<bool>.Success(DBOperationMessages.DataFoundedCorrectly, isPayStatus);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<bool>> IsFirstUnPayedInstallmentAsync(long installmentId, CancellationToken cancellationToken)
        {
            try
            {
                var selectedInstallment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Include(x => x.Document)
                    .FirstOrDefaultAsync(x => !x.IsPaid & x.Id == installmentId, cancellationToken);
                if (selectedInstallment is not null)
                {
                    if (selectedInstallment.Document.Status != DocumentStatus.Active)
                        return CommandResult<bool>.Failure(UserMessages.OnlyCanPayForActiveDocument, false);

                    var firstUnpayedInstallment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .OrderBy(x => x.Number)
                    .FirstOrDefaultAsync(x => !x.IsPaid & x.DocumentId == selectedInstallment.DocumentId, cancellationToken);
                    if (firstUnpayedInstallment is null)
                    {
                        return CommandResult<bool>.Failure(UserMessages.UpPaymentInstallmentNotFound, false);
                    }
                    else
                    {
                        if (firstUnpayedInstallment.Id == selectedInstallment.Id & firstUnpayedInstallment.DocumentId == selectedInstallment.DocumentId)
                            return CommandResult<bool>.Success(DBOperationMessages.DataFoundedCorrectly, true);
                        else
                            return CommandResult<bool>.Failure(UserMessages.OnlyCanPayFirstUnPayedInstallment, false);
                    }

                }
                else
                {
                    return CommandResult<bool>.Failure(string.Format(UserMessages.NotFound, Captions.Installment), false);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<bool>> IsPayableInstallmentAsync(long installmentId, CancellationToken cancellationToken)
        {
            try
            {
                var installment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .SingleOrDefaultAsync(x => x.Id == installmentId & !x.IsPaid);

                if (installment is null)
                {
                    return CommandResult<bool>.Success(UserMessages.PayableInstallmentNotFounded, false);
                }
                if (await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data.AnyAsync(x => !x.IsPaid & x.DocumentId == installment.DocumentId & x.Number < installment.Number))
                {
                    return CommandResult<bool>.Success(UserMessages.PossibleCancelPaymentOnlyForLastPaidInstallment, false);
                }
                else
                {
                    return CommandResult<bool>.Success(UserMessages.InstallmentIsPayable, true);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<bool>> IsCheackPaymentDateAsync(long installmentId, long paymentId, DateTime paymentDate, CancellationToken cancellationToken)
        {
            try
            {
                //var allPaymentsOfInstallment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                //    .Where(x => x.Id == installmentId)
                //    .Include(x => x.Payments.Where(x => x.Id != paymentId))
                //    .SelectMany(x => x.Payments).ToListAsync(cancellationToken);

                var allPaymentsOfInstallment = await _unitOfWork.PaymentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.InstallmentId == installmentId & x.Id != paymentId)
                    .ToListAsync(cancellationToken);

                if (allPaymentsOfInstallment is null | !allPaymentsOfInstallment.Any())
                    return CommandResult<bool>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, true);

                var maxPaymentByDate = allPaymentsOfInstallment.Max(x => x.Date);
                if (paymentDate.Date < maxPaymentByDate.Date.Date)
                    return CommandResult<bool>.Success(UserMessages.ItIsNotPossibleToPayForThisDate, false);
                else
                    return CommandResult<bool>.Success(UserMessages.InstallmentIsPayable, true);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<bool>> IsUnPayableInstallmentAsync(long installmentId, CancellationToken cancellationToken)
        {
            try
            {
                var installment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .SingleOrDefaultAsync(x => x.Id == installmentId & x.IsPaid);

                if (installment is null)
                {
                    return CommandResult<bool>.Success(UserMessages.PayableInstallmentNotFounded, false);
                }
                if (await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .AnyAsync(x => x.IsPaid == true & x.DocumentId == installment.DocumentId & x.Number > installment.Number))
                {
                    return CommandResult<bool>.Success(UserMessages.PossibleCancelPaymentOnlyForLastPaidInstallment, false);
                }
                else
                {
                    return CommandResult<bool>.Success(UserMessages.CanUnPayableTheInstallment, true);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }
        public async Task<CommandResult<bool>> RemoveInstallmentPayment(long installmentId, long paymentId, LogUserActivityModel logUserActivity, CancellationToken cancellationToken)
        {
            try
            {
                //برای حذف پرداخت باید سند فعال باشد
                //CommandResult<DocumentStatus> docStatusResult = await _unitOfWork.DocumentRepository.GetDocumentStatusByInstallmentIdAsync(installmentId, cancellationToken);
                var installment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == installmentId)
                    .Include(x => x.Document)
                    .FirstOrDefaultAsync();

                if (installment != null)
                {
                    if (installment.Document == null)
                        return CommandResult<bool>.Failure(string.Format(UserMessages.NotFound, Captions.Document), false);
                    else
                    {
                        if (installment.Document.Status != DocumentStatus.Active)
                        {
                            return CommandResult<bool>.Failure(UserMessages.ItIsNotPossibleToPerformThisOperation, false);
                        }

                        //remove safe payment
                        var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId, cancellationToken);
                        if (payment.IsSuccess)
                        {
                            _unitOfWork.PaymentRepository.Delete(payment.Data);
                            var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                            if (saveResult.IsSuccess)
                            {
                                //remove image
                                if (!payment.Data.ImageName.IsEmptyOrNull())
                                {
                                    _fileService.DeleteFile(payment.Data.ImageName, _filePathAddress.PaymentImages);
                                }

                                #region log

                                var customerNameResult = await GetFullNameByDocumentIdAsync(installment.Document.Id, cancellationToken);
                                var parameters = new Dictionary<string, string>();
                                parameters.Add("0", payment.Data.Amount.ToString("N0"));
                                parameters.Add("1", installment.Number.ToString());
                                parameters.Add("2", installment.Document.DocumentNo.ToString());
                                parameters.Add("3", customerNameResult.Data);

                                logUserActivity.Parameters = parameters;
                                logUserActivity.ActivityType = AdminActivityType.Delete;
                                logUserActivity.DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Delete_PaymentOfInstallment;
                                _userService.LogUserActivityAsync(logUserActivity, cancellationToken, false);
                                #endregion
                                return CommandResult<bool>.Success(UserMessages.DataSavedSuccessfully, true);
                            }
                            else
                                return CommandResult<bool>.Failure(saveResult.Message, false);
                        }
                        else
                        {
                            //payment not founded
                            return CommandResult<bool>.Failure(payment.Message, false);
                        }
                    }
                }
                else
                {
                    return CommandResult<bool>.Failure(string.Format(UserMessages.NotFound, Captions.Installment), false);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<int>> RemoveDocumentAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _unitOfWork.DocumentRepository.GetByIdAsync(documentId, cancellationToken);
                if (document.Data is null)
                    return CommandResult<int>.Failure(string.Format(UserMessages.NotFound, Captions.Document), 0);
                if (document.Data.Status != DocumentStatus.Active)
                {
                    return CommandResult<int>.Failure(UserMessages.OnlyCanDeleteActiveDocument, document.Data.DocumentNo);
                }
                document.Data.Status = DocumentStatus.Deleted;
                document.Data.DeleteDate = DateTime.Now;
                _unitOfWork.DocumentRepository.Update(document.Data);
                var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (removeResult.IsSuccess)
                    return CommandResult<int>.Success(UserMessages.DataDeletedSuccessfully, document.Data.DocumentNo);
                else
                    return CommandResult<int>.Failure(removeResult.Message, document.Data.DocumentNo);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<int>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<int>> SettleDocumentAsync(SettleDocumentViewModel model, CancellationToken cancellationToken)
        {
            try
            {


                var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == model.DocumentId)
                    .Include(x => x.Customer).ThenInclude(x => x.User)
                    .Include(x => x.Installments).ThenInclude(x => x.Payments)
                    .Include(x => x.Gallery)
                    .FirstOrDefaultAsync(cancellationToken);

                if (document is not null)
                {
                    if (string.IsNullOrEmpty(model.CustomerSettleMessageContent))
                        return CommandResult<int>.Failure(string.Format(ValidationMessages.Required, Captions.MessageContent), document.DocumentNo);

                    if (document.Status == DocumentStatus.Active)
                    {
                        //همه اقساط سند باید پرداخت شده باشد
                        if (document.Installments.Any(x => !x.IsPaid))
                            return CommandResult<int>.Failure(UserMessages.OnlyCanSettleDocumentIfAllInstalmentsIsPayed, document.DocumentNo);

                        if (string.IsNullOrEmpty(model.DiscountAmount))
                            model.DiscountAmount = "0";
                        if (string.IsNullOrEmpty(model.ReturnedAmount))
                            model.ReturnedAmount ??= "0";

                        long totalInstallmentsAmount = document.InstallmentAmount * document.InstallmentCount;
                        int totalDelayDays = document.Installments.Where(x => x.DelayDays.HasValue).Sum(x => x.DelayDays.Value);
                        long delayAmount = await GetDelayDaysAmountAsync(totalDelayDays, document.InstallmentAmount, true);
                        long totalPaymentAmount = 0;
                        foreach (var item in document.Installments)
                            totalPaymentAmount += item.Payments.Sum(x => x.Amount);
                        //محاسبه جمع کل مانده وام - درصورتی کخ مانده کل 0 باشد قابل تسویه میباشد
                        long remainAmount = CalculateSumOfPayAmountInSettleDocument(totalInstallmentsAmount, delayAmount, totalPaymentAmount, long.Parse(model.DiscountAmount.Replace(",", "")), long.Parse(model.ReturnedAmount.Replace(",", "")));


                        if (remainAmount != 0)
                        {
                            string remainAmountCaption = string.Empty;
                            if (remainAmount < 0)
                                remainAmountCaption = $"{Math.Abs(remainAmount).FormatMoney()} {Captions.Tooman} {Captions.Creditor}";
                            else if (remainAmount > 0)
                                remainAmountCaption = $"{Math.Abs(remainAmount).FormatMoney()} {Captions.Tooman} {Captions.Debtor}";

                            return CommandResult<int>.Failure(string.Format(UserMessages.OnlyCanSettleDocumentIfTotalRemainAmountIsZiro, remainAmountCaption), document.DocumentNo);
                        }


                        document.DelayAmount = delayAmount;
                        document.DiscountAmount = long.Parse(model.DiscountAmount.Replace(",", ""));
                        document.ReturnedAmount = long.Parse(model.ReturnedAmount.Replace(",", ""));

                        document.DeliveryDate = model.DeliveryDate.ParsePersianToGorgian();
                        document.SettleDate = model.SettleDate.ParsePersianToGorgian();

                        document.Status = DocumentStatus.Settled;
                        document.SettleRegisterDate = DateTime.Now;

                        _unitOfWork.DocumentRepository.Update(document);

                        #region send customer message

                        var message = new CustomerMessage
                        {
                            CustomerId = document.CustomerId,
                            DocumentId = document.Id,
                            installmentId = null,
                            Message = model.CustomerSettleMessageContent,
                            Date = DateTime.Now,
                            Type = CustomerMessageType.SettleDocument,
                            Title = Captions.Settle
                        };
                        await _unitOfWork.CustomerMessageRepository.InsertAsync(message, cancellationToken);
                        #endregion

                        var updateReult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (updateReult.IsSuccess)
                            return CommandResult<int>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, document.DocumentNo);
                        else
                            return CommandResult<int>.Failure(updateReult.Message, document.DocumentNo);
                    }
                    else
                        return CommandResult<int>.Failure(UserMessages.OnlyCanSettleDocumentInActiveMode, document.DocumentNo);
                }

                else
                    return CommandResult<int>.Failure(String.Format(UserMessages.NotFound, Captions.Document), 0);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<SettleDocumentViewModel>> GetDocumentForSettleAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                SettleDocumentViewModel model = new();
                var documentResult = await _unitOfWork.DocumentRepository.GetByIdAsync(documentId, cancellationToken);
                if (documentResult.IsSuccess)
                {
                    if (documentResult.Data.Status == DocumentStatus.Active)
                    {
                        //همه اقساط سند باید پرداخت شده باشد
                        if (_unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data.Any(x => x.DocumentId == documentId & x.IsPaid == false))
                        {
                            return CommandResult<SettleDocumentViewModel>.Failure(UserMessages.OnlyCanSettleDocumentIfAllInstalmentsIsPayed, model);
                        }
                        var installmentInfoResult = await GetInstallmentsInfoAsync(documentId, cancellationToken);

                        model.DocumentNumber = documentResult.Data.DocumentNo;
                        model.DocumentDate = documentResult.Data.DocumentDate;
                        model.InstallmentCount = documentResult.Data.InstallmentCount;
                        model.DocumentId = documentResult.Data.Id;
                        model.DiscountAmount = "0";
                        model.ReturnedAmount = "0";


                        if (installmentInfoResult.IsSuccess)
                        {
                            model.DelayAmount = installmentInfoResult.Data.DelayAmount;
                            model.InstallmentRemainAmount = installmentInfoResult.Data.InstallmentRemainAmount;
                            model.TotalRemainAmount = installmentInfoResult.Data.TotalRemainAmount;//documentResult.Data.RemainAmount;

                            model.TotalDelayDay = installmentInfoResult.Data.TotalDelayDay;

                        }
                        return CommandResult<SettleDocumentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                    }
                    else
                    {
                        return CommandResult<SettleDocumentViewModel>.Failure(UserMessages.OnlyCanSettleDocumentInActiveMode, model);
                    }
                }
                else
                {
                    return CommandResult<SettleDocumentViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.Document), new SettleDocumentViewModel());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);

                return CommandResult<SettleDocumentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new SettleDocumentViewModel());
            }
        }

        public async Task<CommandResult> UnPaymentDocumentInstallment(long installmentId, long documentId, LogUserActivityModel logUserActivity, CancellationToken cancellationToken)
        {
            try
            {
                //چک شود که آخرین قسط پرداخت شده باشد
                var isUnPaymentableInstallmentResult = await IsUnPayableInstallmentAsync(installmentId, cancellationToken);
                if (!isUnPaymentableInstallmentResult.Data)
                {
                    return CommandResult.Failure(isUnPaymentableInstallmentResult.Message);
                }
                var installment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == installmentId)
                    .Include(x => x.Document)
                    .Include(x => x.CustomerMessages)
                    .FirstOrDefaultAsync(cancellationToken);

                if (installment != null)
                {
                    //خالی کردن مجدد فیلدهایی که در هنگام پرداخت پر شده
                    installment.DelayDays = null;
                    installment.Description = null;
                    installment.PaymentType = null;
                    installment.IsPaid = false;

                    _unitOfWork.InstallmentRepository.Update(installment);

                    var customerMessage = installment.CustomerMessages.FirstOrDefault(x => x.Title == Captions.MessageTitle_Payment);
                    if (customerMessage != null)
                    {
                        _unitOfWork.CustomerMessageRepository.Delete(customerMessage);
                    }

                    //------------to to =>unsend message to customer------------------------
                    // _unitOfWork.CustomerPaymentRepository.Delete();

                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                    {
                        #region log
                        var customerNameResult = await GetFullNameByDocumentIdAsync(documentId, cancellationToken);
                        var parameters = new Dictionary<string, string>();
                        parameters.Add("0", installment.Number.ToString());
                        parameters.Add("1", installment.Document.DocumentNo.ToString());
                        parameters.Add("2", customerNameResult.Data);

                        logUserActivity.Parameters = parameters;
                        logUserActivity.ActivityType = AdminActivityType.Delete;
                        logUserActivity.DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit_InstallmentChangedToUnpaid;
                        #endregion

                        return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
                    }
                    else
                    {
                        return CommandResult.Failure(updateResult.Message);
                    }
                }
                else
                {
                    //installment not founded
                    return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.Installment));
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<int>> UnSettleDocumentAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                var documentResult = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId)
                    .Include(x => x.CustomerMessages)
                    .FirstOrDefaultAsync(cancellationToken);

                if (documentResult is not null)
                {
                    //تنها سند هایی قابلیت لغو تسویه دارند که تسویه شده باشند
                    if (documentResult.Status == DocumentStatus.Settled)
                    {
                        documentResult.Status = DocumentStatus.Active;
                        documentResult.DelayAmount = null;
                        documentResult.DeliveryDate = null;
                        documentResult.SettleDate = null;
                        documentResult.SettleRegisterDate = null;
                        documentResult.DiscountAmount = 0;
                        documentResult.ReturnedAmount = 0;

                        var customerMessage = documentResult.CustomerMessages.FirstOrDefault(x => x.Title == Captions.MessageTitle_Settle);
                        if (customerMessage is not null)
                            _unitOfWork.CustomerMessageRepository.Delete(customerMessage);

                        _unitOfWork.DocumentRepository.Update(documentResult);

                        var updteResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (updteResult.IsSuccess)
                        {
                            return CommandResult<int>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, documentResult.DocumentNo);
                        }
                        else
                        {
                            return CommandResult<int>.Failure(updteResult.Message, documentResult.DocumentNo);
                        }
                    }
                    else
                    {
                        return CommandResult<int>.Failure(UserMessages.ForUnSettleDocumentNeedDocumentStatusIsSettled, documentResult.DocumentNo);
                    }
                }
                else
                {
                    return CommandResult<int>.Failure(String.Format(UserMessages.NotFound, Captions.Document), 0);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<EditDocumentViewModel>> GetDocumentForEditAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                var docStatusResult = await GetDocumentStatusByIdAsync(documentId, cancellationToken);
                if (docStatusResult.IsSuccess)
                {
                    if (docStatusResult.Data == DocumentStatus.Active)
                    {
                        EditDocumentViewModel? document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                            .Where(x => x.Id == documentId)
                            .Include(x => x.Customer).ThenInclude(x => x.User)
                            .Select(d => new EditDocumentViewModel
                            {
                                DocumentId = d.Id,
                                DocumentDate = d.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate),
                                DocumentNo = d.DocumentNo,
                                FullName = d.Customer.User.FullName,
                                GalleryId = d.GalleryId,
                                InstallmentAmount = d.InstallmentAmount.ToString("N0"),
                                InstallmentCount = d.InstallmentCount,
                                PrepaymentAmount = d.PrepaymentAmount.ToString("N0"),
                                RemainAmount = d.RemainAmount.ToString("N0"),
                                SellerId = d.SellerId
                            }).FirstOrDefaultAsync();

                        if (document is not null)
                        {

                            // document.Galleries = _galleryService.GetActiveGalleriesAsync(document.GalleryId.Value, cancellationToken).Result.Data;
                            document.Galleries = _galleryService.GetAllGalleriesAsync(document.GalleryId.Value, cancellationToken).Result.Data;
                            document.Sellers = _sellerService.GetSellersOfGalleryAsync(document.GalleryId.Value, document.SellerId.Value, cancellationToken).Result.Data;

                            return CommandResult<EditDocumentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, document);
                        }
                        else
                        {
                            return CommandResult<EditDocumentViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.Document), null);
                        }
                    }
                    else
                    {
                        return CommandResult<EditDocumentViewModel>.Failure(UserMessages.OnlyCanEditActiveDocument, null);
                    }
                }
                else
                {
                    return CommandResult<EditDocumentViewModel>.Failure(String.Format(UserMessages.NotFound, Captions.Document), null);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EditDocumentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<IQueryable<PaymentReceiptsInPendingConfirmationModel>> GetPaymentReceiptsInPendingConfirmationListAsQuerable(PaymentReceiptsInPendingConfirmationSearchViewModel model)
        {
            try
            {
                var result = _unitOfWork.CustomerPaymentRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Where(x => x.ConfirmStatus != ConfirmStatusType.Confirmation)
                        .Include(c => c.Document).ThenInclude(x => x.Customer).ThenInclude(x => x.User)
                        .AsQueryable();
                    if (model.ConfirmStatus.HasValue)
                        query = query.Where(x => x.ConfirmStatus == model.ConfirmStatus.Value);

                    if (model.DocumentNumber.HasValue)
                        query = query.Where(x => x.Document.DocumentNo == model.DocumentNumber.Value);

                    if (model.DocumentDay.HasValue)
                        query = query.Where(x => x.Document.DayOfMonth == model.DocumentDay.Value);

                    if (!string.IsNullOrEmpty(model.FromDate) & !string.IsNullOrEmpty(model.ToDate))
                    {
                        DateTime? fromDate = model.FromDate.ParsePersianToGorgian();
                        DateTime? toDate = model.ToDate.ParsePersianToGorgian();
                        if (toDate.HasValue & fromDate.HasValue)
                            query = query.Where(x => x.RegisterDate.Date >= fromDate.Value.Date & x.RegisterDate.Date <= toDate.Value.Date);
                    }
                    else if (!model.FromDate.IsEmptyOrNull())
                    {
                        DateTime? fromDate = model.FromDate.ParsePersianToGorgian();
                        if (fromDate.HasValue)
                            query = query.Where(x => x.RegisterDate.Date >= fromDate.Value.Date);
                    }
                    else if (!model.ToDate.IsEmptyOrNull())
                    {
                        DateTime? toDate = model.ToDate.ParsePersianToGorgian();
                        if (toDate.HasValue)
                            query = query.Where(x => x.RegisterDate.Date <= toDate.Value.Date);
                    }

                    IQueryable<PaymentReceiptsInPendingConfirmationModel> finallQuery = query.OrderByDescending(x => x.RegisterDate)
                        .Select(x => new PaymentReceiptsInPendingConfirmationModel
                        {
                            Id = x.Id,
                            FullName = x.Document.Customer.User.FullName,
                            DocumentNumber = x.Document.DocumentNo,
                            InstallmentAmount = x.Document.InstallmentAmount,
                            PersianDocumentDate = x.Document.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate),
                            PersianRegisterDate = x.RegisterDate.GeorgianToPersian(ShowMode.OnlyDateAndTime),
                            ImageName = string.IsNullOrEmpty(x.ImageName) ? null : x.ImageName,
                            PayAmount = x.PayAmount,
                            StatusDescription = x.ConfirmStatus.GetDisplayName(),
                            StatusType = x.ConfirmStatus,
                            PersianPayDate = x.PayDate.HasValue ? DateTimeTools.GeorgianToPersian(x.PayDate.Value, ShowMode.OnlyDate) : string.Empty,
                            PersianPayTime = x.PayDate.HasValue ? DateTimeTools.GeorgianToPersian(x.PayDate.Value, ShowMode.OnlyTime) : string.Empty,
                        }).AsQueryable();

                    return CommandResult<IQueryable<PaymentReceiptsInPendingConfirmationModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<PaymentReceiptsInPendingConfirmationModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<PaymentReceiptsInPendingConfirmationModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<ConfirmCustomerPaymentViewModel>> GetCustomerPaymentForConfirmAsync(long customerPayentId, CancellationToken cancellationToken)
        {
            try
            {
                var customerPaymentResult = await _unitOfWork.CustomerPaymentRepository.GetByIdAsync(customerPayentId, cancellationToken);
                if (customerPaymentResult.IsSuccess)
                {
                    var documentResult = await _unitOfWork.DocumentRepository.GetByIdAsync(customerPaymentResult.Data.DocumentId, cancellationToken);
                    if (documentResult.IsSuccess)
                    {
                        //آیا سند رسید پرداخت نشده دارد یا خیر
                        var firstNotPayedInstallment = await _unitOfWork.InstallmentRepository.GetFirstNotPayedAsync(customerPaymentResult.Data.DocumentId, cancellationToken);
                        if (documentResult.Data.Status == DocumentStatus.Active & firstNotPayedInstallment.IsSuccess)
                        {
                            if (customerPaymentResult.Data.ConfirmStatus == ConfirmStatusType.Confirmation)
                                return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.ThePaymentReciptIsConfirmed, null);
                            else if (customerPaymentResult.Data.ConfirmStatus == ConfirmStatusType.Rejection)
                                return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.ThePaymentReciptIsRejected, null);
                            CommandResult<long> sumOfRemainAmountResult = await GetSumOfRemainAmountToInstallment(firstNotPayedInstallment.Data.Id, 0, cancellationToken);

                            ConfirmCustomerPaymentViewModel model = new()
                            {
                                CustomerPaymentInfo = new()
                                {
                                    DocumentNumber = documentResult.Data.DocumentNo,
                                    InstallmentAmount = documentResult.Data.InstallmentAmount,
                                    InstallmentCount = documentResult.Data.InstallmentCount,
                                    InstallmentNumber = firstNotPayedInstallment.Data.Number,
                                    InstallmentDate = firstNotPayedInstallment.Data.Date,
                                    CustomerDescription = customerPaymentResult.Data.Description,
                                    SumOfRemainAmount = sumOfRemainAmountResult.Data,
                                    CustomerPayedTime = customerPaymentResult.Data.PayDate,
                                    CustomerPaymentRedgisterDate = customerPaymentResult.Data.RegisterDate
                                },
                                CustomerPaymentId = customerPaymentResult.Data.Id,
                                ImageName = customerPaymentResult.Data.ImageName,
                                IsPayInstallment = false,
                                InstallmentId = firstNotPayedInstallment.Data.Id,
                            };
                            if (customerPaymentResult.Data.PayAmount.HasValue)
                                model.PaymentAmount = customerPaymentResult.Data.PayAmount.Value.FormatMoney();

                            var paymentsResult = await GetPaymentsByInstallmentIdAsync(firstNotPayedInstallment.Data.Id, cancellationToken);
                            if (paymentsResult.IsSuccess)
                                model.Payments = paymentsResult.Data;
                            // model.PaymentTypes = GetPamentTypesSelectListItem(null);
                            return CommandResult<ConfirmCustomerPaymentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        }
                        else
                        {
                            return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.CanNotAllowToConfirmThisPaymentRecipt, null);
                        }
                    }
                    else
                    {
                        return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.NotFoundDocumentForThisPaymentRecipt, null);
                    }
                }
                else
                {
                    return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.PaymentRecipt), null);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<ConfirmCustomerPaymentViewModel>> ConfirmCustomerPaymentAsync(ConfirmCustomerPaymentViewModel model, LogUserActivityModel logUserActivity, CancellationToken cancellationToken)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var customerPaymentResult = await _unitOfWork.CustomerPaymentRepository.GetByIdAsync(model.CustomerPaymentId, cancellationToken);
                    if (customerPaymentResult.IsSuccess)
                    {
                        if (customerPaymentResult.Data.ConfirmStatus == ConfirmStatusType.Confirmation)
                            return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.ThePaymentReciptIsConfirmed, null);
                        else if (customerPaymentResult.Data.ConfirmStatus == ConfirmStatusType.Rejection)
                            return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.ThePaymentReciptIsRejected, null);

                        var documentResult = await _unitOfWork.DocumentRepository.GetByIdAsync(customerPaymentResult.Data.DocumentId, cancellationToken);
                        if (documentResult.IsSuccess)
                        {
                            //آیا سند رسید پرداخت نشده دارد یا خیر
                            var firstNotPayedInstallment = await _unitOfWork.InstallmentRepository.GetFirstNotPayedAsync(customerPaymentResult.Data.DocumentId, cancellationToken);
                            if (documentResult.Data.Status == DocumentStatus.Active & firstNotPayedInstallment.IsSuccess)
                            {
                                CommandResult<string> customerNameResult = await GetFullNameByDocumentIdAsync(documentResult.Data.Id, cancellationToken);
                                long paymentamount = 0;
                                if (!long.TryParse(model.PaymentAmount.Replace(",", "").ToString(), out paymentamount))//invalid amount
                                    return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.PaymentAmount), model);

                                DateTime? paymentDate = model.PersianPaymentDate.ParsePersianToGorgian();
                                if (paymentDate.HasValue)
                                {
                                    //invalid paymentDate
                                    if (paymentDate.Value.Date > DateTime.Now.Date)
                                        return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.PaymentDateCanNotMorThanTodayDate, model);
                                }
                                else //invalid paymentDate
                                    return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(String.Format(ValidationMessages.Invalid, Captions.PaymentDate), model);

                                if (model.IsPayInstallment)
                                {
                                    if (model.DelayDay.HasValue)
                                    {
                                        if (model.DelayDay < 0)
                                            return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Delay), model);
                                    }
                                    else
                                        return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Delay), model);

                                    // if (model.PaymentType == null)
                                    //     return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.PaymentType), model);

                                    //برای پرداخت باید اولین قسط پرداخت نشده باشد
                                    var paymantableResult = await IsPayableInstallmentAsync(firstNotPayedInstallment.Data.Id, cancellationToken);
                                    if (!paymantableResult.Data)
                                        return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(paymantableResult.Message, model);

                                    //چک کردن تاریخ پرداخت برای قسط فقط در حالتی که پرداخت برای تسویه قسط میباشد
                                    var paymentDateResultCheack = await IsCheackPaymentDateAsync(model.InstallmentId, 0, paymentDate.Value, cancellationToken);
                                    if (paymentDateResultCheack.IsSuccess)
                                    {
                                        if (!paymentDateResultCheack.Data)
                                            return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(paymentDateResultCheack.Message, model);
                                    }
                                    else
                                        return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(paymentDateResultCheack.Message, model);

                                    #region set paymentType status
                                    var accountStatusResult = await GetSumOfRemainAmountToInstallment(model.InstallmentId, 0, cancellationToken);
                                    //تعیین وضعیت پرداخت قسط
                                    if (accountStatusResult.Data - paymentamount > 0)
                                        model.PaymentType = PaymentType.OverPayment;
                                    else if (accountStatusResult.Data - paymentamount < 0)
                                        model.PaymentType = PaymentType.DeficitPayment;
                                    else
                                        model.PaymentType = PaymentType.FullPayment;
                                    #endregion
                                    //update installment
                                    firstNotPayedInstallment.Data.DelayDays = model.DelayDay;
                                    firstNotPayedInstallment.Data.Description = model.Description;
                                    firstNotPayedInstallment.Data.PaymentType = model.PaymentType;
                                    firstNotPayedInstallment.Data.IsPaid = model.IsPayInstallment;
                                    firstNotPayedInstallment.Data.Description = model.Description;
                                    //update installment
                                    _unitOfWork.InstallmentRepository.Update(firstNotPayedInstallment.Data);
                                    //-------------to do =>generate message for payment-------------------------

                                    #region log
                                    var logParams = new Dictionary<string, string>();
                                    logParams.Add("0", customerNameResult.Data.ToString());
                                    logParams.Add("1", firstNotPayedInstallment.Data.Number.ToString());
                                    logParams.Add("2", documentResult.Data.DocumentNo.ToString());
                                    logParams.Add("3", model.PaymentAmount);
                                    var logModel = logUserActivity;
                                    logModel.Parameters = logParams;
                                    logModel.ActivityType = AdminActivityType.Edit;
                                    logModel.DescriptionPattern = AdminActivityLogDescriptions.PaymentPendingConfirmation_Insert_InstallmentPaymented;
                                    _userService.LogUserActivityAsync(logModel, cancellationToken, false);
                                    #endregion
                                }

                                //add payment
                                //clone customer payment image for payment
                                if (!string.IsNullOrEmpty(customerPaymentResult.Data.ImageName))
                                {
                                    string customerImagePath = $"{_filePathAddress.CustomerPaymentImages}/{customerPaymentResult.Data.ImageName}";
                                    string paymentImagePath = $"{_filePathAddress.PaymentImages}/{customerPaymentResult.Data.ImageName}";
                                    //کپی گرفتن از سند پرداخت مشتری در اسناد پرداخت
                                    if (!_fileService.CopyFile(customerImagePath, paymentImagePath))
                                        return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.ErrorInCopyFile, model);
                                }

                                Payment newPayment = new()
                                {
                                    Amount = paymentamount,
                                    Date = paymentDate.Value,
                                    ImageName = customerPaymentResult.Data.ImageName,
                                    RegisterDate = DateTime.Now,
                                    InstallmentId = firstNotPayedInstallment.Data.Id,
                                    CustomerPaymentId = customerPaymentResult.Data.Id,
                                };
                                await _unitOfWork.PaymentRepository.InsertAsync(newPayment, cancellationToken);

                                //confirm customer payment
                                customerPaymentResult.Data.ConfirmDate = DateTime.Now;
                                customerPaymentResult.Data.ConfirmStatus = ConfirmStatusType.Confirmation;
                                _unitOfWork.CustomerPaymentRepository.Update(customerPaymentResult.Data);

                                var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                                if (updateResult.IsSuccess)
                                {
                                    transaction.Complete();
                                    #region log
                                    var logParams = new Dictionary<string, string>();
                                    logParams.Add("0", customerNameResult.Data.ToString());
                                    logParams.Add("1", firstNotPayedInstallment.Data.Number.ToString());
                                    logParams.Add("2", documentResult.Data.DocumentNo.ToString());
                                    logParams.Add("3", model.PaymentAmount);

                                    var logModel = logUserActivity;
                                    logModel.Parameters = logParams;
                                    logModel.ActivityType = AdminActivityType.Insert;
                                    logModel.DescriptionPattern = AdminActivityLogDescriptions.PaymentPendingConfirmation_Insert;
                                    await _userService.LogUserActivityAsync(logModel, cancellationToken, false);
                                    #endregion

                                    var paymentsResult = await GetPaymentsByInstallmentIdAsync(firstNotPayedInstallment.Data.Id, cancellationToken);
                                    if (paymentsResult.IsSuccess)
                                        model.Payments = paymentsResult.Data;
                                    return CommandResult<ConfirmCustomerPaymentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                                }
                                else
                                    return CommandResult<ConfirmCustomerPaymentViewModel>.Success(updateResult.Message, model);
                            }
                            else
                                return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.CanNotAllowToConfirmThisPaymentRecipt, new());
                        }
                        else
                            return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(UserMessages.NotFoundDocumentForThisPaymentRecipt, new());
                    }
                    else
                        return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.PaymentRecipt), new());
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    await _logManager.RaiseLogAsync(ex, cancellationToken);
                    return CommandResult<ConfirmCustomerPaymentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
                }
            }

        }

        public async Task<CommandResult> RemoveCustomerPaymentAsync(long customerPaymentId, LogUserActivityModel logModel, CancellationToken cancellationToken)
        {
            try
            {
                var itemResult = await _unitOfWork.CustomerPaymentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == customerPaymentId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (itemResult is not null)
                {
                    if (itemResult.ConfirmStatus == ConfirmStatusType.Confirmation)
                        return CommandResult.Failure(UserMessages.ThePaymentReciptIsConfirmedAndCantRemoveIt);
                    else
                    {
                        var documentInfo = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                            .Where(x => x.Id == itemResult.DocumentId)
                            .Include(x => x.Customer).ThenInclude(x => x.User)
                            .Select(x => new
                            {
                                DocumentNumber = x.DocumentNo,
                                CustomerName = x.Customer.User.FullName
                            }).FirstOrDefaultAsync(cancellationToken);
                        if (documentInfo is null)
                            return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.Document));

                        _unitOfWork.CustomerPaymentRepository.Delete(itemResult);
                        var deleteResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (deleteResult.IsSuccess)
                        {
                            #region log
                            var logParams = new Dictionary<string, string>();
                            logParams.Add("0", documentInfo.CustomerName);
                            logParams.Add("1", documentInfo.DocumentNumber.ToString());
                            logModel.Parameters = logParams;
                            logModel.ActivityType = AdminActivityType.Delete;
                            logModel.DescriptionPattern = AdminActivityLogDescriptions.PaymentPendingConfirmation_Delete;
                            await _userService.LogUserActivityAsync(logModel, cancellationToken, false);
                            #endregion

                            return CommandResult.Success(UserMessages.DataDeletedSuccessfully);
                        }
                        else
                            return CommandResult.Failure(deleteResult.Message);
                    }
                }
                else
                    return CommandResult.Failure(String.Format(UserMessages.NotFound, Captions.PaymentRecipt));
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public CommandResult<IQueryable<ConfirmedPaymentReport>> GetPaymentListAsQuerable(PaymentReportSearchViewModel model)
        {
            try
            {
                var result = _unitOfWork.PaymentRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Include(x => x.Installment)
                        .ThenInclude(c => c.Document)
                        .ThenInclude(x => x.Customer)
                        .ThenInclude(x => x.User)
                        .AsQueryable();

                    if (model.DocumentNumber.HasValue)
                        query = query.Where(x => x.Installment.Document.DocumentNo == model.DocumentNumber.Value);

                    if (!string.IsNullOrEmpty(model.FromDatePayment) & !string.IsNullOrEmpty(model.ToDatePayment))
                    {
                        DateTime? fromDate = model.FromDatePayment.ParsePersianToGorgian();
                        DateTime? toDate = model.ToDatePayment.ParsePersianToGorgian();
                        if (toDate.HasValue & fromDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date >= fromDate.Value.Date & x.Date.Date <= toDate.Value.Date);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.FromDatePayment))
                    {
                        DateTime? fromDate = model.FromDatePayment.ParsePersianToGorgian();
                        if (fromDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date >= fromDate.Value.Date);
                        }
                    }
                    else if (!string.IsNullOrEmpty(model.ToDatePayment))
                    {
                        DateTime? toDate = model.ToDatePayment.ParsePersianToGorgian();
                        if (toDate.HasValue)
                        {
                            query = query.Where(x => x.Date.Date <= toDate.Value.Date);
                        }
                    }

                    IQueryable<ConfirmedPaymentReport> finallQuery = query.OrderByDescending(x => x.Date)
                        .Select(x => new ConfirmedPaymentReport
                        {
                            Id = x.Id,
                            FullName = x.Installment.Document.Customer.User.FullName,
                            DocumentNumber = x.Installment.Document.DocumentNo,
                            InstallmentAmount = x.Installment.Amount,
                            InstallmentNumber = x.Installment.Number,
                            PaymentAmount = x.Amount,
                            PersianInstallmentDate = x.Installment.Date.GeorgianToPersian(ShowMode.OnlyDate),
                            PersianPaymentDate = x.Date.GeorgianToPersian(ShowMode.OnlyDate),
                            ImageName = x.ImageName ?? string.Empty,
                        }).AsQueryable();

                    return CommandResult<IQueryable<ConfirmedPaymentReport>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<ConfirmedPaymentReport>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ConfirmedPaymentReport>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<string>> GetFullNameByIdAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                var customerName = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == customerId)
                    .Include(x => x.User)
                    .Select(x => x.User.FullName)
                    .FirstOrDefaultAsync(cancellationToken);
                if (string.IsNullOrEmpty(customerName))
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.Customer), string.Empty);
                else
                    return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<string>> GetFullNameByDocumentIdAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                var customerName = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId)
                    .Include(x => x.Customer).ThenInclude(x => x.User)
                    .Select(x => x.Customer.User.FullName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (string.IsNullOrEmpty(customerName))
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.Customer), string.Empty);
                else
                    return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<int>> GetDocumentNumberByIdAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                int? documentNumber = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId)
                    .Select(x => x.DocumentNo)
                    .FirstOrDefaultAsync(cancellationToken);
                if (documentNumber.HasValue)
                    return CommandResult<int>.Failure(string.Format(UserMessages.NotFound, Captions.Document), 0);
                else
                    return CommandResult<int>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, documentNumber.Value);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<string>> GetFullNameByEditInformationRequestIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var customerName = await _unitOfWork.EditInformationRequestRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == id)
                    .Include(x => x.Customer).ThenInclude(x => x.User)
                    .Select(x => x.Customer.User.FullName)
                    .FirstOrDefaultAsync(cancellationToken);
                if (string.IsNullOrEmpty(customerName))
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.Customer), string.Empty);
                else
                    return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<long>> InstallmentAmountSumAsync(SearchDocumentViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = _unitOfWork.DocumentRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Include(x => x.Collaterals).ThenInclude(x => x.CollateralType)
                        .Include(c => c.Customer).ThenInclude(x => x.User)
                        .Include(i => i.Installments)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(model.Name))
                        query = query.Where(x => x.Customer.User.FullName.Contains(model.Name));

                    if (model.CollateralTypeId.HasValue)
                        query = query.Where(x => x.Collaterals.Any(y => y.CollateralTypeId == model.CollateralTypeId.Value));

                    if (!string.IsNullOrEmpty(model.CollateralDescription))
                        query = query.Where(x => x.Collaterals.Any(y => y.Description.Contains(model.CollateralDescription)));


                    if (!string.IsNullOrEmpty(model.Name))
                        query = query.Where(x => x.Customer.User.FullName.Contains(model.Name));

                    if (model.DocumentNumber.HasValue)
                        query = query.Where(x => x.DocumentNo == model.DocumentNumber.Value);

                    if (model.DocumentDay.HasValue)
                        query = query.Where(x => x.DayOfMonth == model.DocumentDay.Value);

                    if (model.DocumentStatus.HasValue)
                        query = query.Where(x => x.Status == model.DocumentStatus.Value);

                    if (model.GalleryId.HasValue)
                        query = query.Where(x => x.GalleryId == model.GalleryId.Value);

                    if (string.IsNullOrEmpty(model.InstallmentDate))
                    {
                        if (!string.IsNullOrEmpty(model.NotPayedInstallmentFromDate) | !string.IsNullOrEmpty(model.NotPayedInstallmentToDate))
                        {
                            DateTime? notPayedInstallmentFromDate = model.NotPayedInstallmentFromDate.ParsePersianToGorgian();
                            DateTime? notPayedInstallmentToDate = model.NotPayedInstallmentToDate.ParsePersianToGorgian();

                            if (notPayedInstallmentFromDate.HasValue & notPayedInstallmentToDate.HasValue)
                                query = query.Where(i => i.Installments.Any(x => x.Date >= notPayedInstallmentFromDate.Value & x.Date <= notPayedInstallmentToDate.Value & !x.IsPaid));
                            else if (notPayedInstallmentFromDate.HasValue)
                                query = query.Where(i => i.Installments.Any(x => x.Date >= notPayedInstallmentFromDate.Value & !x.IsPaid));
                            else if (notPayedInstallmentToDate.HasValue)
                                query = query.Where(i => i.Installments.Any(x => x.Date <= notPayedInstallmentToDate.Value & !x.IsPaid));
                        }
                    }
                    else
                    {
                        DateTime? iDate = DateTimeTools.ParsePersianToGorgianDateTime(model.InstallmentDate);
                        query = query.Where(i => i.Installments.Any(x => x.Date == iDate));
                    }



                    if (!string.IsNullOrEmpty(model.DocumentDate))
                    {
                        DateTime? docDate = model.DocumentDate.ParsePersianToGorgian();
                        if (docDate.HasValue)
                        {
                            query = query.Where(x => x.DocumentDate == docDate);
                        }
                    }
                    var sumAmount = await query.OrderBy(x => x.DocumentNo)
                         .SumAsync(x => x.InstallmentAmount, cancellationToken);
                    return CommandResult<long>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, sumAmount);
                }
                else
                {
                    return CommandResult<long>.Failure(result.Message, 0);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<long>> RemainAmountSumAsync(SearchDocumentViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = _unitOfWork.DocumentRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data
                        .Include(x => x.Collaterals).ThenInclude(x => x.CollateralType)
                        .Include(c => c.Customer).ThenInclude(x => x.User)
                        .Include(i => i.Installments).ThenInclude(x => x.Payments)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(model.Name))
                        query = query.Where(x => x.Customer.User.FullName.Contains(model.Name));

                    if (model.CollateralTypeId.HasValue)
                        query = query.Where(x => x.Collaterals.Any(y => y.CollateralTypeId == model.CollateralTypeId.Value));

                    if (!string.IsNullOrEmpty(model.CollateralDescription))
                        query = query.Where(x => x.Collaterals.Any(y => y.Description.Contains(model.CollateralDescription)));


                    if (!string.IsNullOrEmpty(model.Name))
                        query = query.Where(x => x.Customer.User.FullName.Contains(model.Name));

                    if (model.DocumentNumber.HasValue)
                        query = query.Where(x => x.DocumentNo == model.DocumentNumber.Value);

                    if (model.DocumentDay.HasValue)
                        query = query.Where(x => x.DayOfMonth == model.DocumentDay.Value);

                    if (model.DocumentStatus.HasValue)
                        query = query.Where(x => x.Status == model.DocumentStatus.Value);

                    if (model.GalleryId.HasValue)
                        query = query.Where(x => x.GalleryId == model.GalleryId.Value);

                    if (string.IsNullOrEmpty(model.InstallmentDate))
                    {
                        if (!string.IsNullOrEmpty(model.NotPayedInstallmentFromDate) | !string.IsNullOrEmpty(model.NotPayedInstallmentToDate))
                        {
                            DateTime? notPayedInstallmentFromDate = model.NotPayedInstallmentFromDate.ParsePersianToGorgian();

                            DateTime? notPayedInstallmentToDate = model.NotPayedInstallmentToDate.ParsePersianToGorgian();

                            if (notPayedInstallmentFromDate.HasValue & notPayedInstallmentToDate.HasValue)
                                query = query.Where(i => i.Installments.Any(x => x.Date >= notPayedInstallmentFromDate.Value & x.Date <= notPayedInstallmentToDate.Value & !x.IsPaid));
                            else if (notPayedInstallmentFromDate.HasValue)
                                query = query.Where(i => i.Installments.Any(x => x.Date >= notPayedInstallmentFromDate.Value & !x.IsPaid));
                            else if (notPayedInstallmentToDate.HasValue)
                                query = query.Where(i => i.Installments.Any(x => x.Date <= notPayedInstallmentToDate.Value & !x.IsPaid));
                        }
                    }
                    else
                    {
                        DateTime? iDate = DateTimeTools.ParsePersianToGorgianDateTime(model.InstallmentDate);
                        query = query.Where(i => i.Installments.Any(x => x.Date == iDate));
                    }



                    if (!string.IsNullOrEmpty(model.DocumentDate))
                    {
                        DateTime? docDate = model.DocumentDate.ParsePersianToGorgian();
                        if (docDate.HasValue)
                        {
                            query = query.Where(x => x.DocumentDate == docDate);
                        }
                    }

                    List<Document> documents = await query
                        .Where(x => x.Status == DocumentStatus.Active)
                        .Select(x => new Document
                        {
                            Id = x.Id,
                            InstallmentAmount = x.InstallmentAmount,
                            InstallmentCount = x.InstallmentCount,
                            DiscountAmount = x.DiscountAmount,
                            Installments = x.Installments,
                            DelayAmount = x.DelayAmount,
                        }).ToListAsync(cancellationToken);


                    long sumOfAmount = 0;
                    var loanSettingResult = _settingService.GetSettingAsync<LoanSettingsViewModel>(SettingType.LoanSetting).Result.Data;

                    decimal penaltyFactor = 0;
                    if (loanSettingResult is not null)
                        decimal.TryParse(loanSettingResult.PenaltyFactor, out penaltyFactor);

                    long sumOfInstallmentsAmount = 0;
                    long sumOfPaymentsAmount = 0;
                    int delayDays = 0;

                    if (documents.Any())
                    {
                        foreach (var doc in documents)
                        {
                            foreach (var installment in doc.Installments)
                            {
                                foreach (var payment in installment.Payments)
                                    sumOfPaymentsAmount += payment.Amount;

                                sumOfInstallmentsAmount += installment.Amount;
                                delayDays += installment.DelayDays.HasValue ? installment.DelayDays.Value : 0;
                            }
                            sumOfAmount += Convert.ToInt64(sumOfInstallmentsAmount + (delayDays * doc.Installments.First().Amount * penaltyFactor) - sumOfPaymentsAmount);

                            delayDays = 0;
                            sumOfPaymentsAmount = 0;
                            sumOfInstallmentsAmount = 0;
                        }
                    }


                    //long sumOfRemainAmount = 0;
                    //if (documents.Any())
                    //{
                    //	foreach (var doc in documents)
                    //	{
                    //		long sumOfInstallmentRemainAmount = 0;
                    //		if (doc.Installments.Any())
                    //		{
                    //			foreach (var installment in doc.Installments)
                    //			{
                    //				if (installment.Payments.Any())
                    //					sumOfInstallmentRemainAmount += installment.Amount - installment.Payments.Sum(x => x.Amount);
                    //				else
                    //					sumOfInstallmentRemainAmount += installment.Amount;
                    //			}
                    //		}
                    //		sumOfRemainAmount += sumOfInstallmentRemainAmount;
                    //	}
                    //}

                    return CommandResult<long>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, sumOfAmount);
                }
                else
                {
                    return CommandResult<long>.Failure(result.Message, 0);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<long>> GetDocumentIdByNumberAsync(int documentNo, CancellationToken cancellationToken)
        {
            try
            {
                var documentId = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.DocumentNo == documentNo & x.Status == DocumentStatus.Active)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (documentId <= 0)
                {
                    return CommandResult<long>.Success(string.Format(UserMessages.NotFound, Captions.Document), 0);
                }
                else
                {
                    return CommandResult<long>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, documentId);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<List<InstallmentPaymentReminderModel>>> GetAllInstallmentsForReminderAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var list = new List<InstallmentPaymentReminderModel>();
            try
            {
                list = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                   .Where(x => !x.IsPaid & x.Date.Date == date.Date & x.Document.Status == DocumentStatus.Active)
                   .Include(x => x.Document).ThenInclude(x => x.Customer).ThenInclude(x => x.User)
                   .Select(x => new InstallmentPaymentReminderModel
                   {
                       CustomerId = x.Document.CustomerId,
                       DocumentId = x.DocumentId,
                       InstallmentId = x.Id,
                       Gender = x.Document.Customer.Gender,
                       FullName = x.Document.Customer.User.FullName,
                       DocumentDate = x.Document.DocumentDate,
                       DocumentNumber = x.Document.DocumentNo,
                       InstallmentAmount = x.Amount,
                       InstallmentCount = x.Document.InstallmentCount,
                       InstallmentDay = x.Document.DayOfMonth,
                       InstallmentNumber = x.Number,
                       TotalDelayDays = (int)Math.Abs((x.Date.Date - DateTime.Now.Date).TotalDays),
                   }).ToListAsync(cancellationToken);

                //get totalDelayDays
                //foreach (var item in list)
                //{
                //    var installments = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                //        .Where(x => x.DocumentId == item.DocumentId & x.Number <= item.InstallmentNumber)
                //        .Select(x => new
                //        {
                //            x.Id,
                //            x.DelayDays,
                //            x.Date
                //        }).ToListAsync(cancellationToken);

                //    int currentInstallmentDelayDay = (int)Math.Abs((installments.FirstOrDefault(x => x.Id == item.InstallmentId).Date.Date - DateTime.Now.Date).TotalDays);
                //    item.TotalDelayDays = installments.Where(x => x.Id != item.InstallmentId & x.DelayDays != null).Sum(x => x.DelayDays.Value) + currentInstallmentDelayDay;
                //}
                return CommandResult<List<InstallmentPaymentReminderModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<InstallmentPaymentReminderModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }

        public async Task<CommandResult<List<BirthdayMessageModel>>> GetCustomerInfoForBirthdayMessage(CancellationToken cancellationToken = default)
        {
            var list = new List<BirthdayMessageModel>();
            try
            {
                string currentDate = DateTime.Now.GeorgianToPersian(ShowMode.OnlyDate);
                int year = int.Parse(currentDate.Split('/')[0]);
                byte month = byte.Parse(currentDate.Split('/')[1]);
                byte day = byte.Parse(currentDate.Split('/')[2]);
                list = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                   .Where(x => x.BirthDateDay == day & x.BirthDateMonth == month)
                   .Include(x => x.User)
                   .Select(x => new BirthdayMessageModel
                   {
                       CustomerId = x.Id,
                       Day = x.BirthDateDay.ToString(),
                       DayName = day.ToString().ToPersianCharacterString(),
                       FullName = x.User.FullName,
                       Gender = x.Gender.HasValue ? x.Gender.Value == GenderType.Men ? Captions.Mr : Captions.Delay : string.Empty,
                       Month = x.BirthDateMonth.ToString(),
                       MonthName = x.BirthDateMonth.ToString().ToPersianCharacterString(),
                       Year = x.BirthDateYear.ToString()
                   }).ToListAsync(cancellationToken);

                return CommandResult<List<BirthdayMessageModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<BirthdayMessageModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }

        #region customer 
        public async Task<CommandResult> SendMessageToCustomerAsync(CreateCustomerMessageModel model, bool saveData = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var newCustomerMessage = new CustomerMessage
                {
                    CustomerId = model.CustomerId,
                    Date = DateTime.Now,
                    DocumentId = model.DocumentId,
                    installmentId = model.installmentId,
                    Title = model.Title,
                    Type = model.MessageType
                };
                newCustomerMessage.Message = await GenerateMessageAsync(model.MessageSettingType, model.Parameters);
                if (!string.IsNullOrEmpty(newCustomerMessage.Message))
                {
                    var addResult = await _unitOfWork.CustomerMessageRepository.InsertAsync(newCustomerMessage, cancellationToken);
                    if (saveData)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult.Success(addResult.Message);
                        else
                            return CommandResult.Failure(addResult.Message);
                    }

                    if (addResult.IsSuccess)
                        return CommandResult.Success(addResult.Message);
                    else
                        return CommandResult.Failure(addResult.Message);
                }
                else
                {
                    return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }
        public async Task<string> GenerateMessageAsync(SettingType messageType, Dictionary<string, string> parameters)
        {
            string messageContent = string.Empty;

            if (messageType == SettingType.MessageType_DocumentRegistration |
                messageType == SettingType.MessageType_ReminderOfTheDayBeforeTheInstallmentDate |
                messageType == SettingType.MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate |
                messageType == SettingType.MessageType_SettleDocument |
                messageType == SettingType.MessageType_DeficitPayment |
                messageType == SettingType.MessageType_Payment |
                messageType == SettingType.MessageType_HappyBirthday |
                messageType == SettingType.MessageType_OverPayment)
            {
                var settingResult = await _settingService.GetSettingAsync<string>(messageType);
                if (settingResult.IsSuccess)
                {
                    if (!settingResult.Data.IsEmptyOrNull())
                        messageContent = settingResult.Data;
                }
            }
            foreach (var param in parameters)
            {
                string index = "{" + param.Key + "}";
                messageContent = messageContent.Replace(index, param.Value);
            }
            return messageContent;
        }

        public async Task<CommandResult<CustomerProfileModel>> GetCustomerProfileInfoAsync(string userName, CancellationToken cancellationToken = default)
        {
            try
            {
                var userInfo = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserName == userName & x.UserType == UserType.Customer)
                    .Include(x => x.Customer).ThenInclude(x => x.ProfileImages)
                    .Select(x => new CustomerProfileModel
                    {
                        FullName = x.FullName,
                        Mobile = x.Mobile,
                        Address = x.Customer.Address,
                        CityId = x.Customer.CityId,
                        FatherName = x.Customer.FatherName,
                        NationalCode = x.Customer.NationalCode,
                        PostalCode = x.Customer.PostalCode,
                        SanaCode = x.Customer.SanaCode,
                        JobTitle = x.Customer.JobTitle,
                        BirthDate = x.Customer.BirthDate.HasValue ? x.Customer.BirthDate.Value.GeorgianToPersian(ShowMode.OnlyDate) : null,
                        ProfileImage = x.Customer.ProfileImages.Any() != null ? x.Customer.ProfileImages.OrderByDescending(x => x.RegisterDate).FirstOrDefault().ImageName : null
                    }).FirstOrDefaultAsync(cancellationToken);
                if (userInfo is null)
                {
                    return CommandResult<CustomerProfileModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.Customer), new CustomerProfileModel());
                }
                var provinceResult = await _unitOfWork.CityRepository.GetProvinceAsync(userInfo.CityId, cancellationToken);
                if (provinceResult.IsSuccess)
                    userInfo.ProvinceId = provinceResult.Data.Id;

                return CommandResult<CustomerProfileModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, userInfo);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerProfileModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new CustomerProfileModel());
            }
        }

        public async Task<CommandResult<List<DocumentInfoModel>>> GetDocumentsInfoAsync(string userName, CancellationToken cancellationToken)
        {
            List<DocumentInfoModel> list = new List<DocumentInfoModel>();
            try
            {
                var documents = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserName == userName & x.UserType == UserType.Customer)
                    .Include(x => x.Customer).ThenInclude(x => x.Documents.Where(x => x.Status != DocumentStatus.Deleted)).ThenInclude(x => x.Collaterals)
                    .Include(x => x.Customer).ThenInclude(x => x.Documents.Where(x => x.Status != DocumentStatus.Deleted)).ThenInclude(x => x.CustomerPayments)
                    .Include(x => x.Customer).ThenInclude(x => x.Documents.Where(x => x.Status != DocumentStatus.Deleted)).ThenInclude(x => x.Installments)
                    .SelectMany(x => x.Customer.Documents.Where(x => x.Status != DocumentStatus.Deleted).ToList()).ToListAsync(cancellationToken);
                if (documents.Any())
                {
                    foreach (var document in documents.OrderBy(x => x.Status == DocumentStatus.Active).OrderByDescending(x => x.DocumentDate))
                    {
                        var model = new DocumentInfoModel
                        {
                            Id = document.Id,
                            Number = document.DocumentNo,
                            PersianDate = document.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate),
                            Status = document.Status,
                            HasCollateralImageUrl = document.Collaterals.Any() ? document.Collaterals.Any(x => x.ImageName != null) : false
                        };


                        if (document.Status == DocumentStatus.Active & document.Installments.Any(x => !x.IsPaid & x.Date.Date < DateTime.Now.Date))
                        {
                            #region سند تسویه نشده با قسط پرداخت نشده عقب افتاده
                            model.DocumentPaymentState = DocumentPaymentStatus.UnpaidOverdueInstallment;
                            #endregion
                        }
                        else if (document.Status == DocumentStatus.Active & document.CustomerPayments.Any(x => x.ConfirmStatus == ConfirmStatusType.Pending))
                        {
                            #region سند تسویه نشده با پرداخت در انتظار تایید
                            model.DocumentPaymentState = DocumentPaymentStatus.DocumentWithPaymentPending;
                            #endregion
                        }
                        else if (document.Status == DocumentStatus.Active & !document.Installments.Any(x => !x.IsPaid & x.Date.Date < DateTime.Now.Date))
                        {
                            #region سند تسویه نشده بدون قسط عقب افتاده
                            model.DocumentPaymentState = DocumentPaymentStatus.UnsettledDocumentWithOutOverdueInstallments;
                            #endregion
                        }
                        list.Add(model);
                    }
                }
                return CommandResult<List<DocumentInfoModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<DocumentInfoModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }
        //public async Task<CommandResult> HaveDocumentsAsync(string userName, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var haveDoc = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
        //            .Where(x => x.UserName == userName & x.UserType == UserType.Customer)
        //            .Include(x => x.Customer).ThenInclude(x => x.Documents.Where(x => x.Status != DocumentStatus.Deleted))
        //            .SelectMany(x => x.Customer.Documents.ToList()).AnyAsync(cancellationToken);
        //        if (haveDoc)
        //            return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
        //        return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.Documents));
        //    }
        //    catch (Exception ex)
        //    {
        //        await _logManager.RaiseLogAsync(ex, cancellationToken);
        //        return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
        //    }
        //}

        public async Task<CommandResult<CustomerDocumentDetailModel>> GetCustomerDocumentDetailAsync(string userName, long documentId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserName == userName & x.UserType == UserType.Customer & x.IsActive)
                    .Include(x => x.Customer).ThenInclude(x => x.EssentialTels)
                    .FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                    return CommandResult<CustomerDocumentDetailModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Customer), new CustomerDocumentDetailModel());


                Document? document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.CustomerId == user.Customer.Id & x.Id == documentId)
                    .Include(x => x.Collaterals).ThenInclude(x => x.CollateralType)
                    .Include(x => x.Installments).ThenInclude(x => x.Payments)
                    .Include(x => x.Installments).ThenInclude(x => x.CustomerMessages)
                    .Include(x => x.Gallery)
                    .Include(x => x.Seller).ThenInclude(x => x.User)
                    .Include(x => x.CustomerPayments)
                    .Include(x => x.CustomerMessages)
                    .FirstOrDefaultAsync(cancellationToken);
                if (document is null)
                    return CommandResult<CustomerDocumentDetailModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Document), new CustomerDocumentDetailModel());


                CustomerDocumentDetailModel customerDocument = new()
                {
                    FullName = user.FullName,
                    NationalCode = user.Customer.NationalCode,
                    SaleDate = document.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate),
                    Mobile = user.Mobile,
                    EssentialTell = user.Customer.EssentialTels.OrderBy(x => x.OrderNo).FirstOrDefault()?.Tel,
                    DocumentNo = document.DocumentNo,
                    InvoiceAmount = document.PrepaymentAmount + document.RemainAmount,
                    PrePaymentAmount = document.PrepaymentAmount,
                    RemainAmount = document.RemainAmount,
                    InstallmentCount = document.InstallmentCount,
                    InstallmentAmount = document.InstallmentAmount,
                    CollateralInfo = document.Collaterals.Select(x => $"{x.CollateralType.Title} - {x.Description}").ToList(),
                    Gallery = document.Gallery.Name,
                    DocumentDate = document.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate),
                    Seller = document.Seller.User.FullName,
                    PersianDeliveryDate = (document.DeliveryDate.HasValue ? document.DeliveryDate.Value.GeorgianToPersian(ShowMode.OnlyDate) : string.Empty),
                    PersianSettleDate = (document.SettleDate.HasValue ? document.SettleDate.Value.GeorgianToPersian(ShowMode.OnlyDate) : string.Empty),
                    DocumentLastCustomerMessage = document.CustomerMessages.Any() ? document.CustomerMessages.OrderByDescending(x => x.Date).FirstOrDefault().Message : string.Empty,
                };

                bool checkHasPendingPayment = true;
                if (document.Installments is not null)
                {
                    if (string.IsNullOrEmpty(customerDocument.DocumentLastCustomerMessage))
                    {
                        var lastInstallmentWithMessage = document.Installments
                            .Where(x => x.CustomerMessages.Any())
                            .OrderByDescending(x => x.Number)
                            .FirstOrDefault();

                        if (lastInstallmentWithMessage is not null)
                        {
                            var lastCustomerMessage = lastInstallmentWithMessage.CustomerMessages
                                .OrderByDescending(x => x.Date)
                                .FirstOrDefault();
                            if (lastCustomerMessage is not null)
                                customerDocument.DocumentLastCustomerMessage = lastCustomerMessage.Message;
                        }

                    }
                    foreach (var installment in document.Installments)
                    {
                        var item = new CustomerDocumentInstallmentModel();
                        item.InstallmentId = installment.Id;
                        item.Delay = installment.DelayDays;
                        item.CustomerMessage = (installment.CustomerMessages.Any() ? installment.CustomerMessages.FirstOrDefault().Message : string.Empty);
                        item.PaymentAmount = installment.Payments.Sum(x => x.Amount);
                        item.PersianInstallmentDate = installment.Date.GeorgianToPersian(ShowMode.OnlyDate);
                        item.PersianPaymentDate = installment.Payments.OrderByDescending(x => x.Date).FirstOrDefault()?.Date.GeorgianToPersian(ShowMode.OnlyDate);

                        //اقساط عقب افتاده به رنگ قرمز نمایش داده می شوند -
                        if (!installment.IsPaid & installment.Date.Date < DateTime.Now.Date)
                            item.InstallmentStateClass = "bg-red text-white";

                        // اگر رسید پرداخت ثبت شده در انتظار تایید دارد اولین قسط پرداخت نشده به رنگ زرد نمایش داده شود - 
                        if (!installment.IsPaid & checkHasPendingPayment & document.CustomerPayments.Any(x => x.ConfirmStatus == ConfirmStatusType.Pending))
                        {
                            item.InstallmentStateClass = "bg-yellow text-dark";
                            checkHasPendingPayment = false;
                        }

                        customerDocument.Installments.Add(item);
                    }

                    foreach (var installment in document.Installments)
                    {
                        customerDocument.PaymentAmount += installment.Payments.Sum(x => x.Amount);
                    }
                }

                return CommandResult<CustomerDocumentDetailModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerDocument);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerDocumentDetailModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new CustomerDocumentDetailModel());
            }
        }

        public async Task<CommandResult<List<CustomerPaymentModel>>> GetCustomerPaymentsAsync(long documentId, CancellationToken cancellationToken)
        {
            List<CustomerPaymentModel> list = new List<CustomerPaymentModel>();
            try
            {
                var payments = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                   .Where(x => x.Id == documentId & x.Status == DocumentStatus.Active)
                   .Include(x => x.CustomerPayments)
                   .SelectMany(x => x.CustomerPayments).ToListAsync(cancellationToken);

                foreach (var item in payments.OrderByDescending(x => x.RegisterDate))
                {
                    CustomerPaymentModel payment = new();
                    payment.ImageName = item.ImageName;
                    payment.Description = string.IsNullOrEmpty(item.Description) ? string.Empty : item.Description;
                    payment.PersianDate = item.RegisterDate.GeorgianToPersian(ShowMode.OnlyDateAndTime);
                    payment.PaymentAmount = item.PayAmount;
                    payment.PaymentDate = item.PayDate.HasValue ? item.PayDate.Value.GeorgianToPersian(ShowMode.OnlyDateAndTime) : null;
                    payment.ConfirmType = item.ConfirmStatus;
                    payment.AdminDescription = item.AdminDescription;
                    list.Add(payment);
                }
                if (list is not null)
                    return CommandResult<List<CustomerPaymentModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
                return CommandResult<List<CustomerPaymentModel>>.Failure(string.Format(ValidationMessages.Invalid, Captions.Document), new List<CustomerPaymentModel>());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<CustomerPaymentModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }

        public async Task<CommandResult<DocumentInfoForCustomerPayment>> GetDocumentInfoForCustomerPaymentAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                DocumentInfoForCustomerPayment? document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId & x.Status == DocumentStatus.Active)
                    .Select(x => new DocumentInfoForCustomerPayment
                    {
                        DocumentId = x.Id,
                        DocumentNo = x.DocumentNo,
                        InstallmentAmount = x.InstallmentAmount,
                        PersianDocumentDate = x.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate)
                    }).FirstOrDefaultAsync(cancellationToken);

                if (document is not null)
                    return CommandResult<DocumentInfoForCustomerPayment>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, document);
                return CommandResult<DocumentInfoForCustomerPayment>.Failure(string.Format(ValidationMessages.Invalid, Captions.Document), new DocumentInfoForCustomerPayment());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<DocumentInfoForCustomerPayment>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new DocumentInfoForCustomerPayment());
            }
        }

        public async Task<CommandResult> CreateCustomerPaymentAsync(CustomerPaymentViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                bool isValidForCustomerPayment = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .AnyAsync(x => x.Id == model.DocumentId & x.Status == DocumentStatus.Active, cancellationToken);
                if (isValidForCustomerPayment)
                {
                    string imageName = Guid.NewGuid().ToString();
                    CustomerPayment customerPayment = new()
                    {
                        DocumentId = model.DocumentId,
                        RegisterDate = DateTime.Now,
                        Description = model.Description,
                        ConfirmStatus = ConfirmStatusType.Pending,
                    };
                    //fill pay date and time
                    DateTime? payDateTime = null;
                    if (!string.IsNullOrEmpty(model.PayDate) & !string.IsNullOrEmpty(model.PayTime))
                    {
                        model.PayDate = model.PayDate.ToEnglishNumberByCultureInfo();
                        model.PayTime = model.PayTime.ToEnglishNumberByCultureInfo();

                        var payGorgianDate = DateTimeTools.ParsePersianToGorgian(model.PayDate);
                        if (payGorgianDate is null)
                            return CommandResult.Failure(string.Format(ValidationMessages.Invalid, Captions.PayDate));



                        payDateTime = new DateTime(payGorgianDate.Value.Year, payGorgianDate.Value.Month, payGorgianDate.Value.Day, int.Parse(model.PayTime.Split(':')[0]), int.Parse(model.PayTime.Split(':')[1]), 0);
                        if (payDateTime > DateTime.Now)
                            return CommandResult.Failure(UserMessages.PayDateTimeCanNotMoreThanDateTimeNow);
                        customerPayment.PayDate = payDateTime;
                    }

                    //fill pay amount
                    if (!string.IsNullOrEmpty(model.PayAmount))
                    {
                        if (long.TryParse(model.PayAmount.Replace(",", ""), out long payAmount))
                        {
                            if (payAmount <= 1000)
                                return CommandResult.Failure(string.Format(ValidationMessages.MoreThan, Captions.Price, $"1,000 {Captions.Tooman}"));
                            customerPayment.PayAmount = payAmount;
                        }
                        else
                            return CommandResult.Failure(string.Format(ValidationMessages.Invalid, Captions.Price));
                    }



                    //save image
                    if (model.PaymentReceiptImage is not null)
                        if (!await _fileService.UploadFileAsync(model.PaymentReceiptImage, _filePathAddress.CustomerPaymentImages, imageName))
                            return CommandResult.Failure(UserMessages.ErrorInUploadFile);
                        else
                            customerPayment.ImageName = $"{imageName}{Path.GetExtension(model.PaymentReceiptImage.FileName)}";


                    await _unitOfWork.CustomerPaymentRepository.InsertAsync(customerPayment, cancellationToken);
                    var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (saveResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataSavedSuccessfully);
                    else
                        return CommandResult.Failure(saveResult.Message);
                }
                else
                    return CommandResult.Failure(string.Format(ValidationMessages.Invalid, Captions.Document));
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<Pagination<CustomerMessageModel>>> GetMessagesAsync(string userName, int page, CancellationToken cancellationToken)
        {
            try
            {
                var customerId = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                   .Where(x => x.UserName == userName & x.UserType == UserType.Customer & x.IsActive)
                   .Include(x => x.Customer)
                   .Select(x => x.Customer.Id)
                   .FirstOrDefaultAsync(cancellationToken);


                var iquerableMessagesResult = _unitOfWork.CustomerMessageRepository.GetAllAsIQueryable();
                if (iquerableMessagesResult.IsSuccess)
                {
                    var customerMessagesCount = await iquerableMessagesResult.Data
                   .Where(x => x.CustomerId == customerId & (
                   x.Type == CustomerMessageType.Custom |
                   x.Type == CustomerMessageType.RegisterDocument |
                   x.Type == CustomerMessageType.Reminder7Days |
                   x.Type == CustomerMessageType.PaymentReminder |
                   x.Type == CustomerMessageType.HappyBirthday))
                   .OrderByDescending(x => x.Date)
                   .CountAsync(cancellationToken);

                    //var d = iquerableMessagesResult.Data.Where(x => x.CustomerId == customerId).ToList();
                    Pagination<CustomerMessageModel> pager = new Pagination<CustomerMessageModel>(customerMessagesCount, page, 10);

                    pager.Data = await iquerableMessagesResult.Data
                        .Where(x => x.CustomerId == customerId & (
                        x.Type == CustomerMessageType.Custom |
                   x.Type == CustomerMessageType.RegisterDocument |
                   x.Type == CustomerMessageType.Reminder7Days |
                   x.Type == CustomerMessageType.PaymentReminder |
                   x.Type == CustomerMessageType.HappyBirthday))
                        .OrderByDescending(x => x.Date)
                        .Skip((page - 1) * pager.PageSize).Take(pager.PageSize)
                        .Select(x => new CustomerMessageModel
                        {
                            Content = x.Message,
                            PersianDate = x.Date.GeorgianToPersian(ShowMode.OnlyDate),
                            Title = x.Title
                        }).ToListAsync(cancellationToken);
                    return CommandResult<Pagination<CustomerMessageModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, pager);
                }
                else
                {
                    return CommandResult<Pagination<CustomerMessageModel>>.Failure(iquerableMessagesResult.Message, null);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<Pagination<CustomerMessageModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<CustomerProfileViewModel>> EditProfileAsync(CustomerProfileViewModel model, string userName, CancellationToken cancellationToken)
        {
            try
            {
                DateTime? birthDate = null;
                if (!string.IsNullOrEmpty(model.BirthDate))
                {
                    birthDate = DateTimeTools.ParsePersianToGorgian(model.BirthDate.ToEnglishNumbers());
                    if (birthDate is null)
                        return CommandResult<CustomerProfileViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.BirthDate), model);
                    if (birthDate >= DateTime.Now)
                        return CommandResult<CustomerProfileViewModel>.Failure(string.Format(ValidationMessages.NotMoreThan, Captions.BirthDate, Captions.Today), model);
                }

                var customerUser = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserName == userName & x.UserType == UserType.Customer & x.IsActive)
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(cancellationToken);
                if (customerUser == null)
                    return CommandResult<CustomerProfileViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Customer), model);
                if (model.CityId.HasValue)
                {
                    var cityResult = await _unitOfWork.CityRepository.GetCityAsync(model.CityId.Value, cancellationToken);
                    if (cityResult.IsSuccess)
                        customerUser.Customer.CityId = model.CityId;
                }
                else
                {
                    customerUser.Customer.CityId = null;
                }
                customerUser.Customer.Address = model.Address;
                customerUser.Customer.SanaCode = string.IsNullOrEmpty(model.SanaCode) ? null : model.SanaCode.ToEnglishNumbers();
                customerUser.Customer.PostalCode = string.IsNullOrEmpty(model.PostalCode) ? null : model.PostalCode.ToEnglishNumbers();
                customerUser.Customer.JobTitle = model.JobTitle;
                customerUser.Customer.BirthDate = birthDate;

                customerUser.Customer.BirthDateYear = model.BirthDate is null ? null : int.Parse(model.BirthDate.Split('/')[0].ToEnglishNumbers());
                customerUser.Customer.BirthDateMonth = model.BirthDate is null ? null : byte.Parse(model.BirthDate.Split('/')[1].ToEnglishNumbers());
                customerUser.Customer.BirthDateDay = model.BirthDate is null ? null : byte.Parse(model.BirthDate.Split('/')[2].ToEnglishNumbers());

                if (model.ProfileImage is not null)
                {
                    //save profile image
                    string imageName = Guid.NewGuid().ToString();
                    if (!await _fileService.UploadFileAsync(model.ProfileImage, _filePathAddress.CustomerProfileImage, imageName))
                        return CommandResult<CustomerProfileViewModel>.Failure(UserMessages.ErrorInUpdateProfileImage, model);
                    else
                        imageName = $"{imageName}{Path.GetExtension(model.ProfileImage.FileName)}";

                    ProfileImage profileImage = new()
                    {
                        CustomerId = customerUser.Customer.Id,
                        ImageName = imageName,
                        RegisterDate = DateTime.Now
                    };
                    await _unitOfWork.ProfileImageRepository.InsertAsync(profileImage, cancellationToken);
                }

                _unitOfWork.CustomerRepository.Update(customerUser.Customer);
                var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (updateResult.IsSuccess)
                {
                    model.ProfileImage = null;
                    return CommandResult<CustomerProfileViewModel>.Success(updateResult.Message, model);
                }
                else
                    return CommandResult<CustomerProfileViewModel>.Failure(updateResult.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CustomerProfileViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<EditInformationRequestViewModel>> EditInformationRequestAsync(EditInformationRequestViewModel model, string userName, CancellationToken cancellationToken)
        {
            try
            {
                var customerUser = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserName == userName & x.UserType == UserType.Customer & x.IsActive)
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(cancellationToken);
                if (customerUser == null)
                    return CommandResult<EditInformationRequestViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Customer), model);
                EditInformationRequest editInfoRequest = new()
                {
                    CustomerId = customerUser.Customer.Id,
                    IsActive = true,
                    Description = model.Description,
                    RegisterDate = DateTime.Now,
                };

                if (model.Image is not null)
                {
                    //save image
                    string imageName = Guid.NewGuid().ToString();
                    if (!await _fileService.UploadFileAsync(model.Image, _filePathAddress.EditInformationRequest, imageName))
                        return CommandResult<EditInformationRequestViewModel>.Failure(UserMessages.ErrorInUploadFile, model);
                    else
                        editInfoRequest.ImageName = $"{imageName}{Path.GetExtension(model.Image.FileName)}";
                }
                await _unitOfWork.EditInformationRequestRepository.InsertAsync(editInfoRequest, cancellationToken);
                var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (result.IsSuccess)
                    return CommandResult<EditInformationRequestViewModel>.Success(UserMessages.YourEditInformationRequestSuccessfulySended, model);
                else
                    return CommandResult<EditInformationRequestViewModel>.Failure(result.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<EditInformationRequestViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult> HaveDocumentAsync(string userName, CancellationToken cancellationToken = default)
        {
            try
            {
                bool haveDocument = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .AnyAsync(x => x.UserName == userName & x.UserType == UserType.Customer & x.Customer.Documents.Any(x => x.Status != DocumentStatus.Deleted), cancellationToken);
                if (haveDocument)
                    return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
                return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.Document));
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }


        #endregion

        public async Task<CommandResult<CalculationInstallmentDelayModel>> CalculationInstallmentDelayInfoAsync(long installmentId, long selectedPaymentId, string paymentDate, CancellationToken cancellationToken)
        {
            CalculationInstallmentDelayModel model = new();

            try
            {

                var installment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == installmentId)
                    .Include(x => x.Payments)
                    .FirstOrDefaultAsync(cancellationToken);
                if (installment is not null)
                {
                    var allDelayDaysBeforThisInstallment = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Id == installment.DocumentId)
                        .Include(x => x.Installments.Where(x => x.IsPaid))
                        .Select(x => x.Installments.Where(x => x.DelayDays.HasValue).Sum(x => x.DelayDays.Value))
                        .FirstOrDefaultAsync(cancellationToken);
                    model.CurrentInstallmentDelayDay = allDelayDaysBeforThisInstallment;
                    model.TotalDelayDayToCurrentInstallment = allDelayDaysBeforThisInstallment;
                    DateTime? installmentPaymentDate = DateTimeTools.ParsePersianToGorgian(paymentDate);
                    if (installmentPaymentDate is null)
                        return CommandResult<CalculationInstallmentDelayModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.PaymentDate), model);
                    if (installmentPaymentDate.Value.Date >= installment.Date.Date & installmentPaymentDate.Value.Date <= DateTime.Now.Date)
                    {
                        List<DateTime> allPaymentDate = installment.Payments.Where(x => x.Id != selectedPaymentId).Select(x => x.Date).ToList();
                        allPaymentDate.Add(installmentPaymentDate.Value);
                        var payDateForCalcDelayDay = allPaymentDate.Max(x => x);
                        model.TotalDelayDayToCurrentInstallment += (int)Math.Abs(Math.Floor((payDateForCalcDelayDay.Date - installment.Date.Date).TotalDays));

                        model.CurrentInstallmentDelayDay = (int)Math.Abs(Math.Floor((installmentPaymentDate.Value.Date - installment.Date.Date).TotalDays));
                        model.DelayAmount = 0;
                    }
                    if (installmentPaymentDate.Value.Date < installment.Date.Date)
                        model.CurrentInstallmentDelayDay = 0;

                    return CommandResult<CalculationInstallmentDelayModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                {
                    return CommandResult<CalculationInstallmentDelayModel>.Failure(string.Format(UserMessages.NotFound, Captions.Installment), model);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CalculationInstallmentDelayModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<string>> GetCollateralImageAsync(long documentId, string userName, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserName == userName)
                    .Include(x => x.Customer).ThenInclude(x => x.Documents)
                    .ThenInclude(x => x.Collaterals)
                    .Select(x => new
                    {
                        Collateral = x.Customer.Documents.FirstOrDefault(x => x.Id == documentId).Collaterals.FirstOrDefault(x => x.ImageName != null)
                    }).FirstOrDefaultAsync(cancellationToken);

                string imageName = string.Empty;
                if (item is not null)
                    if (item.Collateral is not null)
                        imageName = item.Collateral.ImageName ?? string.Empty;
                return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, imageName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<string>> GeneratePaymentDescriptionAsync(long installmentId, long paymentId, long newAmount, CancellationToken cancellationToken)
        {
            string description = string.Empty;
            try
            {
                var installment = await _unitOfWork.InstallmentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == installmentId & !x.IsPaid)
                    .Include(x => x.Payments)
                    .FirstOrDefaultAsync(cancellationToken);
                if (installment is not null)
                {
                    long sumOfPayments = installment.Payments.Where(x => x.Id != paymentId).Sum(x => x.Amount);
                    sumOfPayments += newAmount;
                    if (sumOfPayments == installment.Amount)
                    {
                        //پرداخت کامل
                        description = string.Empty; //Captions.FullPayment;

                    }
                    else if (sumOfPayments > installment.Amount)
                    {
                        //اضافه پرداخت
                        long overPaymentValue = Math.Abs(sumOfPayments - installment.Amount);
                        //description = $"{Captions.Price} {overPaymentValue.ToString("N0")} {Captions.Tooman} {Captions.OverPayment}";
                        description = $"{Captions.Price} {overPaymentValue.ToString("N0")} {Captions.Tooman}";

                    }
                    else
                    {
                        //کسری پرداخت
                        long overPaymentValue = Math.Abs(sumOfPayments - installment.Amount);
                        //description = $"{Captions.Price} {overPaymentValue.ToString("N0")} {Captions.Tooman} {Captions.DeficitPayment}";
                        description = $"{Captions.Price} {overPaymentValue.ToString("N0")} {Captions.Tooman}";
                    }
                }

                return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, description);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, description);
            }
        }

        public async Task<CommandResult<PaymentDescriptionWithMessageModel>> GeneratePaymentDescriptionWithMessageAsync(long installmentId, long paymentId, long newAmount, int delayDay, CancellationToken cancellationToken)
        {
            PaymentDescriptionWithMessageModel model = new();
            try
            {
                var accountStatusResult = await GetSumOfRemainAmountToInstallment(installmentId, paymentId, cancellationToken);
                var currentInstallmentResult = await _unitOfWork.InstallmentRepository.GetByIdAsync(installmentId, cancellationToken);
                if (!currentInstallmentResult.IsSuccess)
                    return CommandResult<PaymentDescriptionWithMessageModel>.Failure(currentInstallmentResult.Message, model);

                // CommandResult<CalculationInstallmentDelayModel> result = await CalculationInstallmentDelayInfoAsync(installmentId, paymentId, paymentDate, cancellationToken);
                //CommandResult<CalculationInstallmentDelayModel> result = await CalculationInstallmentDelayInfoAsync(installmentId, paymentId, paymentDate, cancellationToken);

                var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == currentInstallmentResult.Data.DocumentId)
                    .Include(x => x.Customer).ThenInclude(x => x.User)
                    .Include(x => x.Installments)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Gender = (x.Customer.Gender.HasValue ? (x.Customer.Gender.Value == GenderType.Men ? Captions.Mr : Captions.Lady) : string.Empty),
                        CustomerName = x.Customer.User.FullName,
                        DocumentNo = x.DocumentNo,
                        DocumentDate = x.DocumentDate,
                        InstallmentCount = x.InstallmentCount,
                        InstallmentAmount = x.InstallmentAmount,
                        TotalDelayDays = x.Installments.Where(i => i.DelayDays != null & i.Id != installmentId).Sum(s => s.DelayDays.Value)
                    }).FirstOrDefaultAsync(cancellationToken);

                string installmentDay = currentInstallmentResult.Data.Date.GeorgianToPersian(ShowMode.OnlyDate).Split('/')[2];

                Dictionary<string, string> messageParameters = new Dictionary<string, string>();
                messageParameters.Add("0", document.Gender);//جنسیت
                messageParameters.Add("1", document.CustomerName);//نام و نام خانوادگی
                messageParameters.Add("2", document.DocumentNo.ToString());//شماره سند
                messageParameters.Add("3", document.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate));//تاریخ سند
                messageParameters.Add("4", document.InstallmentCount.ToString());// تعداد کل اقساط به عدد
                messageParameters.Add("5", document.InstallmentCount.ToString().ToPersianAlphabetNumber());// تعداد کل اقساط به حروف
                messageParameters.Add("6", document.InstallmentAmount.ToString("N0"));//مبلغ قسط
                messageParameters.Add("7", installmentDay.ToArray()[0] == '0' ? installmentDay.ToArray()[1].ToString() : installmentDay);//روز قسط به عدد
                messageParameters.Add("8", (installmentDay.ToArray()[0] == '0' ? installmentDay.ToArray()[1].ToString() : installmentDay).ToPersianAlphabetNumber());//روز قسط به حروف
                messageParameters.Add("9", currentInstallmentResult.Data.Number.ToString());//شماره قسط به عدد
                messageParameters.Add("10", currentInstallmentResult.Data.Number.ToString().ToPersianAlphabetNumber3());//شماره قسط به حروف


                //string delayDays = result.IsSuccess ? result.Data.TotalDelayDayToCurrentInstallment.ToString() : "0";
                if (delayDay < 0)
                    delayDay = 0;

                delayDay += document.TotalDelayDays;

                messageParameters.Add("11", delayDay.ToString());//تعداد روز دیرکرد کل

                model.Description = "مستند: \n";
                if (accountStatusResult.Data - newAmount > 0)
                {
                    //کسری در پرداخت
                    messageParameters.Add("12", Math.Abs(accountStatusResult.Data - newAmount).ToString("N0"));
                    model.Description += Math.Abs(accountStatusResult.Data - newAmount).ToString("N0") + $" {Captions.Tooman} {Captions.DeficitInPayment}";
                    model.Message = await GenerateMessageAsync(SettingType.MessageType_DeficitPayment, messageParameters);

                }
                else if (accountStatusResult.Data - newAmount < 0)
                {
                    //اضافه پرداخت
                    messageParameters.Add("12", Math.Abs(accountStatusResult.Data - newAmount).ToString("N0"));
                    model.Description += Math.Abs(accountStatusResult.Data - newAmount).ToString("N0") + $" {Captions.Tooman} {Captions.OverPayment}";
                    model.Message = await GenerateMessageAsync(SettingType.MessageType_OverPayment, messageParameters);
                }
                else
                {
                    //پرداخت کامل
                    model.Description += Captions.FullPayment;
                    model.Message = await GenerateMessageAsync(SettingType.MessageType_Payment, messageParameters);
                }

                return CommandResult<PaymentDescriptionWithMessageModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<PaymentDescriptionWithMessageModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<RejectionCustomerPaymentViewModel>> RejectCustomerPaymentAsync(RejectionCustomerPaymentViewModel model, LogUserActivityModel logModel, CancellationToken cancellationToken)
        {
            try
            {
                var customerPaymentResult = await _unitOfWork.CustomerPaymentRepository.GetByIdAsync(model.CustomerPaymentId, cancellationToken);
                if (customerPaymentResult.IsSuccess)
                {
                    if (customerPaymentResult.Data.ConfirmStatus == ConfirmStatusType.Rejection)
                        return CommandResult<RejectionCustomerPaymentViewModel>.Failure(UserMessages.ThePaymentReciptIsRejected, model);
                    else if (customerPaymentResult.Data.ConfirmStatus == ConfirmStatusType.Confirmation)
                        return CommandResult<RejectionCustomerPaymentViewModel>.Failure(UserMessages.ThePaymentReciptIsConfirmed, model);

                    customerPaymentResult.Data.AdminDescription = model.Description;
                    customerPaymentResult.Data.ConfirmStatus = ConfirmStatusType.Rejection;
                    _unitOfWork.CustomerPaymentRepository.Update(customerPaymentResult.Data);

                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                    {
                        CommandResult<string> customerNameResult = await GetFullNameByDocumentIdAsync(customerPaymentResult.Data.DocumentId, cancellationToken);
                        var documentNumberResult = await GetDocumentNumberByIdAsync(customerPaymentResult.Data.DocumentId, cancellationToken);
                        #region log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", customerNameResult.Data.ToString());
                        logParams.Add("1", documentNumberResult.Data.ToString());
                        customerPaymentResult.Data.AdminDescription ??= string.Empty;
                        logParams.Add("2", customerPaymentResult.Data.AdminDescription);

                        logModel.Parameters = logParams;
                        logModel.ActivityType = AdminActivityType.Insert;
                        logModel.DescriptionPattern = AdminActivityLogDescriptions.PaymentPendingRejection_Insert;
                        await _userService.LogUserActivityAsync(logModel, cancellationToken, true);
                        #endregion

                        return CommandResult<RejectionCustomerPaymentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                    }
                    else
                    {
                        return CommandResult<RejectionCustomerPaymentViewModel>.Failure(updateResult.Message, model);
                    }

                }
                else
                    return CommandResult<RejectionCustomerPaymentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.PaymentRecipt), new());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<RejectionCustomerPaymentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<RejectionCustomerPaymentViewModel>> GetCustomerPaymentForRejectAsync(long customerPayentId, CancellationToken cancellationToken)
        {
            try
            {
                var customerPayment = await _unitOfWork.CustomerPaymentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == customerPayentId)
                    .Include(x => x.Document)
                    .Select(x => new RejectionCustomerPaymentViewModel
                    {
                        CustomerPaymentId = x.Id,
                        CustomerDescription = x.Description,
                        Description = x.AdminDescription,
                        DocumentNumber = x.Document.DocumentNo,
                        InstallmentAmount = x.Document.InstallmentAmount,
                        RegisterPaymentDateTime = DateTimeTools.GeorgianToPersian(x.RegisterDate, ShowMode.OnlyDateAndTime),
                        StatusType = x.ConfirmStatus
                    }).FirstOrDefaultAsync(cancellationToken);

                if (customerPayment is not null)
                    return CommandResult<RejectionCustomerPaymentViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerPayment);
                else
                    return CommandResult<RejectionCustomerPaymentViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.PaymentRecipt), null);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<RejectionCustomerPaymentViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<List<ProfileImagesModel>>> GetProfileImagesAsync(int customerId, CancellationToken cancellationToken)
        {
            try
            {
                List<ProfileImagesModel> customerProfileImages = await _unitOfWork.ProfileImageRepository.GetAllAsIQueryable().Data
                    .Where(x => x.CustomerId == customerId)
                    .Select(x => new ProfileImagesModel
                    {
                        CustomerId = x.CustomerId,
                        Id = x.Id,
                        ImageName = x.ImageName,
                        RegisterDate = x.RegisterDate
                    }).ToListAsync(cancellationToken);
                if (customerProfileImages is not null)
                    return CommandResult<List<ProfileImagesModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerProfileImages);
                else
                    return CommandResult<List<ProfileImagesModel>>.Failure(string.Format(UserMessages.NotFound, Captions.ProfileImages), customerProfileImages);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<ProfileImagesModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<ProfileImagesModel>> GetProfileImageByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var profileImage = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Include(x => x.ProfileImages)
                    .Select(x => x.ProfileImages.OrderByDescending(x => x.RegisterDate).FirstOrDefault())
                    .FirstOrDefaultAsync(cancellationToken);

                if (profileImage is not null)
                {
                    ProfileImagesModel imagesModel = new ProfileImagesModel
                    {
                        Id = profileImage.Id,
                        RegisterDate = profileImage.RegisterDate,
                        CustomerId = profileImage.CustomerId,
                        ImageName = profileImage.ImageName
                    };
                    return CommandResult<ProfileImagesModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, imagesModel);
                }
                else
                    return CommandResult<ProfileImagesModel>.Failure(string.Format(UserMessages.NotFound, Captions.ProfileImages), null);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProfileImagesModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        /// <summary>
        /// مبلغ مانده کل تا قسط فعلی
        /// </summary>
        /// <param name="installmentId">ای دی قسط فعلی</param>
        /// <param name="cancellationToken"></param>
        /// <returns>مبلغ مانده کل با محاسبه دیرکرد</returns>
        public async Task<CommandResult<long>> GetSumOfRemainAmountToInstallment(long installmentId, long paymentId, CancellationToken cancellationToken = default)
        {
            try
            {
                long sumOfAmount = 0;
                var loanSettingResult = _settingService.GetSettingAsync<LoanSettingsViewModel>(SettingType.LoanSetting).Result.Data;

                decimal penaltyFactor = 0;
                if (loanSettingResult is not null)
                    decimal.TryParse(loanSettingResult.PenaltyFactor, out penaltyFactor);

                var currentInstallment = await _unitOfWork.InstallmentRepository.GetByIdAsync(installmentId, cancellationToken);

                if (currentInstallment.IsSuccess)
                {
                    var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Id == currentInstallment.Data.DocumentId)
                        .Include(x => x.Installments).ThenInclude(x => x.Payments)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (document is not null)
                        if (document.Installments.Any())
                        {
                            long sumOfInstallmentsAmount = 0;
                            long sumOfPaymentsAmount = 0;
                            int delayDays = 0;
                            foreach (var installment in document.Installments.Where(x => x.Date <= currentInstallment.Data.Date))
                            {
                                foreach (var payment in installment.Payments.Where(x => x.Id != paymentId))
                                    sumOfPaymentsAmount += payment.Amount;

                                sumOfInstallmentsAmount += installment.Amount;
                                delayDays += installment.DelayDays.HasValue ? installment.DelayDays.Value : 0;
                            }
                            if (document.InstallmentCount == document.Installments.Where(x => x.Date <= currentInstallment.Data.Date).Count())
                            {
                                //قسط اخر و محاسبه دیرکرد لحاظ میشود
                                sumOfAmount = Convert.ToInt64(sumOfInstallmentsAmount + (delayDays * document.InstallmentAmount * penaltyFactor) - sumOfPaymentsAmount);
                            }
                            else
                            {
                                sumOfAmount = Convert.ToInt64(sumOfInstallmentsAmount - sumOfPaymentsAmount);
                            }
                        }

                    return CommandResult<long>.Success(OperationResultMessage.AnErrorHasOccurredInTheSoftware, sumOfAmount);
                }
                else
                    return CommandResult<long>.Failure(currentInstallment.Message, 0);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<List<SendMessageModel>>> GetCustomerListForSendMessageAsync(SendMessageViewModel model, CancellationToken cancellationToken)
        {
            List<SendMessageModel> list = new();
            try
            {
                var query = _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                     .Include(x => x.Documents).ThenInclude(x => x.Collaterals).ThenInclude(x => x.CollateralType)
                     .Include(x => x.Documents).ThenInclude(x => x.Gallery)
                     .Include(x => x.User).AsQueryable();
                if (!string.IsNullOrEmpty(model.FullName))
                    query = query.Where(x => x.User.FullName.Contains(model.FullName));

                if (model.GenderType is not null)
                    query = query.Where(x => x.Gender == model.GenderType);

                if (model.NationalityType is not null)
                    query = query.Where(x => x.Nationality == model.NationalityType);

                if (model.UserStatus is not null)
                    query = query.Where(x => x.User.IsActive == model.UserStatus);

                var fromRegisterDate = model.FromRegisterDate.ParsePersianToGorgian();
                if (fromRegisterDate.HasValue)
                    query = query.Where(x => x.User.RegisterDate.Date >= fromRegisterDate.Value.Date);

                var toRegisterDate = model.ToRegisterDate.ParsePersianToGorgian();
                if (fromRegisterDate.HasValue)
                    query = query.Where(x => x.User.RegisterDate.Date <= toRegisterDate.Value.Date);

                if (model.FromBirthDateDay.HasValue)
                    query = query.Where(x => x.BirthDateDay >= model.FromBirthDateDay.Value);

                if (model.ToBirthDateDay.HasValue)
                    query = query.Where(x => x.BirthDateDay <= model.ToBirthDateDay.Value);

                if (model.FromBirthDateMonth.HasValue)
                    query = query.Where(x => x.BirthDateMonth >= model.FromBirthDateMonth.Value);
                if (model.ToBirthDateMonth.HasValue)
                    query = query.Where(x => x.BirthDateMonth <= model.ToBirthDateMonth.Value);

                if (model.FromBirthDateYear.HasValue)
                    query = query.Where(x => x.BirthDateYear >= model.FromBirthDateYear.Value);
                if (model.ToBirthDateYear.HasValue)
                    query = query.Where(x => x.BirthDateYear <= model.ToBirthDateYear.Value);

                if (model.DocumentStatus.HasValue)
                    query = query.Where(x => x.Documents.Any(x => x.Status == model.DocumentStatus.Value));

                if (model.CollateralTypeId.HasValue)
                    query = query.Where(x => x.Documents.Any(d => d.Collaterals.Any(c => c.CollateralTypeId == model.CollateralTypeId.Value)));

                if (model.GalleryId.HasValue)
                    query = query.Where(x => x.Documents.Any(d => d.GalleryId == model.GalleryId.Value));


                var documentFromDate = model.DocumentFromDate.ParsePersianToGorgian();
                if (documentFromDate.HasValue)
                    query = query.Where(x => x.Documents.Any(d => d.DocumentDate.Date >= documentFromDate.Value.Date));

                var documentToDate = model.DocumentToDate.ParsePersianToGorgian();
                if (documentToDate.HasValue)
                    query = query.Where(x => x.Documents.Any(d => d.DocumentDate.Date <= documentToDate.Value.Date));

                if (long.TryParse(model.FromInstallmentAmount, out long fromAmount))
                    query = query.Where(x => x.Documents.Any(d => d.InstallmentAmount >= fromAmount));

                if (long.TryParse(model.ToInstallmentAmount, out long toAmount))
                    query = query.Where(x => x.Documents.Any(d => d.InstallmentAmount <= toAmount));

                var unpaidInstallmentFromDate = model.UnpaidInstallmentFromDate.ParsePersianToGorgian();
                if (unpaidInstallmentFromDate.HasValue)
                    query = query.Where(x => x.Documents.Any(d => d.Installments.Any(i => !i.IsPaid & i.Date.Date >= unpaidInstallmentFromDate.Value.Date)));

                var unpaidInstallmentToDate = model.UnpaidInstallmentToDate.ParsePersianToGorgian();
                if (unpaidInstallmentToDate.HasValue)
                    query = query.Where(x => x.Documents.Any(d => d.Installments.Any(i => !i.IsPaid & i.Date.Date <= unpaidInstallmentToDate.Value.Date)));

                list = query.Select(x => new SendMessageModel
                {
                    CustomerName = x.User.FullName,
                    CustomerId = x.Id,
                    Mobile = x.User.Mobile,
                    NationalCode = x.NationalCode,
                    Nationality = x.Nationality.HasValue ? x.Nationality.Value.GetDisplayName() : string.Empty,
                    RegisterDate = x.User.RegisterDate.GeorgianToPersian(ShowMode.OnlyDate),
                }).ToList();

                foreach (var item in list)
                {
                    var accountStatusResult = await GetCustomerAccountStatus(item.CustomerId, cancellationToken);
                    item.AccountStatus = accountStatusResult.Data;
                }
                if (model.AccountStatusType.HasValue)
                {
                    switch (model.AccountStatusType.Value)
                    {
                        case AccountStatusType.GoodPay:
                            list = list.Where(x => x.AccountStatus == AccountStatusType.GoodPay.GetDisplayName()).ToList();
                            break;
                        case AccountStatusType.DeadBeat:
                            list = list.Where(x => x.AccountStatus == AccountStatusType.DeadBeat.GetDisplayName()).ToList();
                            break;
                        default:
                            break;
                    }
                }
                return CommandResult<List<SendMessageModel>>.Success(string.Empty, list);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SendMessageModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, list);
            }
        }

        public async Task<CommandResult> SendMessageToCustomersAsync(SendMessageContentViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Title))
                    return CommandResult.Failure(string.Format(ValidationMessages.Required, Captions.Title));
                if (string.IsNullOrEmpty(model.MessageContent))
                    return CommandResult.Failure(string.Format(ValidationMessages.Required, Captions.MessageContent));

                if (model.CustomerIds is null)
                    return CommandResult.Failure(UserMessages.NotSelectedRecored);

                var customerIds = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => model.CustomerIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync(cancellationToken);
                if (customerIds is null)
                    return CommandResult.Failure(UserMessages.NotSelectedValidCustomer);
                else
                {
                    foreach (var customerId in customerIds)
                    {

                        await _unitOfWork.CustomerMessageRepository.InsertAsync(new()
                        {
                            CustomerId = customerId,
                            Date = DateTime.Now,
                            Message = model.MessageContent,
                            Title = model.Title,
                            Type = CustomerMessageType.Custom
                        }, cancellationToken);
                    }
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult.Success(UserMessages.MessageIsSuccessfulySended);
                    else
                        return CommandResult.Failure(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<int?>> GetCustomerIdByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var customerId = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (customerId > 0)
                    return CommandResult<int?>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, customerId);
                else
                    return CommandResult<int?>.Failure(string.Format(UserMessages.NotFound, Captions.Customer), null);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int?>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<InstantSettlementModel>> GetInstantSettlementInfoAsync(long documentId, CancellationToken cancellationToken)
        {
            try
            {
                long todayDelayAmount = 0;
                long instantSettlementAmount = 0;
                long delayAmount = 0;
                int todayTotalDelayDay = 0;
                int totalDelayDay = 0;
                long totalPayedAmount = 0;
                var loanSetting = await _settingService.GetSettingAsync<LoanSettingsViewModel>(Domain.Enums.SettingType.LoanSetting, cancellationToken);
                if (!loanSetting.IsSuccess)
                    return CommandResult<InstantSettlementModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);

                var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId & x.Status == DocumentStatus.Active)
                    .Include(x => x.Installments).ThenInclude(x => x.Payments)
                    .FirstOrDefaultAsync(cancellationToken);
                if (document is null)
                    return CommandResult<InstantSettlementModel>.Failure(UserMessages.OnlyCanInstantSettlementActiveDocument, null);

                foreach (var item in document.Installments)
                {
                    if (!item.IsPaid & item.Date.Date < DateTime.Now.Date & document.Status == DocumentStatus.Active)
                        todayTotalDelayDay += (int)Math.Abs((DateTime.Now.Date - item.Date.Date).TotalDays);

                    if (item.Payments.Any())
                        totalPayedAmount += item.Payments.Sum(x => x.Amount);
                    if (item.DelayDays.HasValue)
                        totalDelayDay += item.DelayDays.Value;
                }

                //delayAmount = (long)Math.Ceiling(totalDelayDay * penaltyFactor * document.InstallmentAmount);
                delayAmount = await GetDelayDaysAmountAsync(totalDelayDay, document.InstallmentAmount, true);
                if (document.Status == DocumentStatus.Active)
                {
                    //todayDelayAmount = (long)Math.Ceiling(todayTotalDelayDay * penaltyFactor * document.InstallmentAmount);
                    todayDelayAmount = await GetDelayDaysAmountAsync(todayTotalDelayDay, document.InstallmentAmount, true);
                }
                else
                    todayDelayAmount = delayAmount;

                //if (document.Status == DocumentStatus.Active)
                instantSettlementAmount = CalculationInstantSettlementAmount(document.Installments.Sum(x => x.Amount), todayDelayAmount, totalPayedAmount);
                InstantSettlementModel model = new()
                {
                    InstantSettlementAmount = NumberTools.RoundUpNumber(instantSettlementAmount.ToString(), 4),
                    DocumentId = document.Id
                };

                return CommandResult<InstantSettlementModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<InstantSettlementModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }

        }

        public async Task<CommandResult<SettleDocumentMessageModel>> GenerateSettleDocumentMessageAsync(long documentId, string settleDate, string deliveryDate, long returnedAmount, long discountAmount, CancellationToken cancellationToken)
        {
            SettleDocumentMessageModel model = new();

            try
            {
                var document = await _unitOfWork.DocumentRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == documentId)
                    .Include(x => x.Customer).ThenInclude(x => x.User)
                    .Include(x => x.Installments)
                    .Include(x => x.Gallery)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Gender = (x.Customer.Gender.HasValue ? (x.Customer.Gender.Value == GenderType.Men ? Captions.Mr : Captions.Lady) : string.Empty),
                        CustomerName = x.Customer.User.FullName,
                        DocumentNo = x.DocumentNo,
                        DocumentDate = x.DocumentDate,
                        InstallmentCount = x.InstallmentCount,
                        InstallmentAmount = x.InstallmentAmount,
                        GalleryName = x.Gallery.Name,
                        PrepaymentAmount = x.PrepaymentAmount,
                        TotalDelayDays = x.Installments.Where(i => i.DelayDays != null).Sum(s => s.DelayDays.Value),
                        TotalPaymentAmount = x.Installments.SelectMany(x => x.Payments).Sum(x => x.Amount),
                    }).FirstOrDefaultAsync(cancellationToken);
                if (document is not null)
                {
                    long totalInstallmentsAmount = document.InstallmentCount * document.InstallmentAmount;
                    var installmentInfoResult = await GetInstallmentsInfoAsync(document.Id, cancellationToken);
                    long delayAmount = await GetDelayDaysAmountAsync(document.TotalDelayDays, document.InstallmentAmount, true);

                    //long remainAmount = CalculateSumOfPayAmountInSettleDocument(totalInstallmentsAmount, delayAmount, document.TotalPaymentAmount, discountAmount, returnedAmount);
                    Dictionary<string, string> messageParameters = new Dictionary<string, string>();
                    messageParameters.Add("0", document.Gender);//جنسیت
                    messageParameters.Add("1", document.CustomerName);//نام و نام خانوادگی
                    messageParameters.Add("2", document.DocumentNo.ToString());//شماره سند
                    messageParameters.Add("3", document.DocumentDate.GeorgianToPersian(ShowMode.OnlyDate));//تاریخ سند
                    messageParameters.Add("4", document.InstallmentCount.ToString());//تعداد کل اقساط به عدد
                    messageParameters.Add("5", document.InstallmentCount.ToString().ToPersianAlphabetNumber());// تعداد کل اقساط به حروف
                    messageParameters.Add("6", document.InstallmentAmount.ToString("N0"));//مبلغ قسط
                    messageParameters.Add("7", document.TotalDelayDays.ToString());//تعداد روز دیرکرد کل
                    messageParameters.Add("8", document.GalleryName);//گالری
                    messageParameters.Add("9", document.PrepaymentAmount.ToString("N0"));//پیش پرداخت
                    messageParameters.Add("10", discountAmount.ToString("N0"));//مبلغ تخفیف
                    messageParameters.Add("11", delayAmount.ToString("N0"));//مبلغ دیرکرد

                    var _deliveryDate = deliveryDate.ParsePersianToGorgian();
                    if (!string.IsNullOrEmpty(deliveryDate) & _deliveryDate.HasValue)
                        messageParameters.Add("12", deliveryDate);//تاریخ تحویل

                    var _settleDate = settleDate.ParsePersianToGorgian();
                    if (!string.IsNullOrEmpty(settleDate) & _settleDate.HasValue)
                        messageParameters.Add("13", settleDate);//تاریخ تسویه

                    model.Message = await GenerateMessageAsync(SettingType.MessageType_SettleDocument, messageParameters);
                    return CommandResult<SettleDocumentMessageModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }

                return CommandResult<SettleDocumentMessageModel>.Failure(string.Format(UserMessages.NotFound, Captions.Document), model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<SettleDocumentMessageModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<bool>> HasDocumentAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                bool hasDocument = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                     .Include(x => x.Documents)
                     .AnyAsync(x => x.Documents.Any());
                return CommandResult<bool>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, hasDocument);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }
        public async Task<CommandResult<List<ProductForShowInSiteModel>>> GetAllProductsInFavoritAsync(int userId, CancellationToken cancellationToken)
        {
            string? productName = string.Empty;
            try
            {
                var productIds = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Include(x => x.FavoritProducts).ThenInclude(x => x.Product)
                    .Select(x => x.FavoritProducts.Select(x => x.ProductId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (productIds == null)
                    return CommandResult<List<ProductForShowInSiteModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());

                var productList = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                    .Where(x => productIds.Contains(x.Id))
                   .Include(x => x.ProductGalleries)
                   .Select(x => new ProductForShowInSiteModel
                   {
                       Id = x.Id,
                       ImageName = x.ProductGalleries.Any() ? x.ProductGalleries.Where(x => x.IsThumbnail).FirstOrDefault().FileName : string.Empty,
                       IsImage = x.ProductGalleries.Any() ? x.ProductGalleries.Where(x => x.IsThumbnail).FirstOrDefault().FileType == MediaFileType.Image : false,
                       IsSold = x.Status == ProductStatus.Sold,
                       Title = x.Title,
                   }).ToListAsync(cancellationToken);
                if (productList is null)
                    return CommandResult<List<ProductForShowInSiteModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
                return CommandResult<List<ProductForShowInSiteModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, productList);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<ProductForShowInSiteModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        public async Task<CommandResult> AddProductToFavoritsAsync(long productId, int userId, CancellationToken cancellationToken)
        {
            string? productName = string.Empty;
            try
            {
                var customerId = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (customerId == null | customerId <= 0)
                {
                    return CommandResult.Failure(string.Format(OperationResultMessage.NotFound, Captions.Customer));
                }

                productName = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == productId)
                    .Select(x => x.Title)
                    .FirstOrDefaultAsync(cancellationToken);
                if (string.IsNullOrEmpty(productName))
                    return CommandResult.Failure(string.Format(OperationResultMessage.NotFound, Captions.Product));
                var item = new FavoritProduct
                {
                    ProductId = productId,
                    CustomerId = customerId
                };
                await _unitOfWork.FavoritProductRepository.InsertAsync(item, cancellationToken);
                var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (result.IsSuccess)
                    return CommandResult.Success(string.Format(UserMessages.ProductAddedInCustomerFavorits, productName));
                else
                    return CommandResult.Failure(result.Message);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }
        public async Task<CommandResult> RemoveProductFromFavoritsAsync(long productId, int userId, CancellationToken cancellationToken)
        {
            string? productName = string.Empty;
            try
            {
                var customerFavorit = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Include(x => x.FavoritProducts).ThenInclude(x => x.Product)
                    .Select(x => x.FavoritProducts.FirstOrDefault(x => x.ProductId == productId))
                    .FirstOrDefaultAsync(cancellationToken);
                if (customerFavorit is null)
                    return CommandResult.Failure(string.Format(OperationResultMessage.NotFound, Captions.Product));

                productName = customerFavorit.Product.Title;
                _unitOfWork.FavoritProductRepository.Delete(customerFavorit);
                var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (result.IsSuccess)
                    return CommandResult.Success(string.Format(UserMessages.ProductRemovededInCustomerFavorits, productName));
                else
                    return CommandResult.Failure(result.Message);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<bool>> IsProductInFavoritsAsync(long id, int userId)
        {
            try
            {
                bool isFavorite = await _unitOfWork.CustomerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.UserId == userId)
                    .Include(x => x.FavoritProducts)
                    .AnyAsync(x => x.FavoritProducts.Any(x => x.ProductId == id));
                return CommandResult<bool>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, isFavorite);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }
    }
}

