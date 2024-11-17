using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.SettingsModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.Domain.Entities;
using Gold.Domain.Enums;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.Convertor;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GoldPriceViewModels;
using Gold.ApplicationService.Contract.DTOs.Models.GoldPriceModels;
using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.FAQViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Serialization;
using Gold.SharedKernel.Tools;
using System.Threading;

namespace Gold.ApplicationService.Concrete
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly ILogManager _logManager;
        private readonly FilePathAddress _filePathAddress;


        public SettingService(IUnitOfWork unitOfWork, IFileService fileService, IConfiguration configuration, ILogManager logManager, IOptions<FilePathAddress> filePathAddressOptions)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _configuration = configuration;
            _logManager = logManager;
            _filePathAddress = filePathAddressOptions.Value;

        }


        public async Task<CommandResult<TModel>> CreateSettingAsync<TModel>(TModel model, SettingType type, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.SettingRepository.IsDuplicateAsync(type, cancellationToken);
                if (result.IsSuccess)
                    return CommandResult<TModel>.Failure(OperationResultMessage.ExistenceOfDuplicateRecord, model);
                else
                {
                    string? settingValue = model is null ? string.Empty : model.ConvertObjectToJson();
                    var newSetting = new Domain.Entities.Setting
                    {
                        Type = type,
                        Value = settingValue ?? string.Empty
                    };
                    var insertResult = await _unitOfWork.SettingRepository.InsertAsync(newSetting, cancellationToken);
                    if (insertResult.IsSuccess)
                    {
                        var commitChangesResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (commitChangesResult.IsSuccess)
                            return CommandResult<TModel>.Success(UserMessages.DataSavedSuccessfully, model);
                        else
                            return CommandResult<TModel>.Failure(OperationResultMessage.OperationIsFailure, model);
                    }
                    else
                    {
                        return CommandResult<TModel>.Failure(OperationResultMessage.OperationIsFailure, model);
                    }
                }

            }
            catch (Exception)
            {
                return CommandResult<TModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }
        public async Task<CommandResult<TModel>> GetSettingAsync<TModel>(SettingType type, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.SettingRepository.GetSettingByTypeAsync(type, cancellationToken);
                if (result.IsSuccess)
                {
                    var model = result.Data.Value.ConvertJsonToOject<TModel>();
                    if (model is null)
                    {
                        return CommandResult<TModel>.Failure(string.Format(OperationResultMessage.NotFound, nameof(SettingType)), default(TModel));
                    }
                    return CommandResult<TModel>.Success(UserMessages.DataSavedSuccessfully, model);
                }
                else
                {
                    return CommandResult<TModel>.Failure(OperationResultMessage.OperationIsFailure, default(TModel));
                }
            }
            catch (Exception)
            {
                return CommandResult<TModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, default(TModel));
            }
        }
        public async Task<CommandResult<TModel>> UpdateSettingAsync<TModel>(TModel model, SettingType type, CancellationToken cancellationToken = default)
        {
            try
            {

                var result = await _unitOfWork.SettingRepository.GetSettingByTypeAsync(type, cancellationToken);
                if (result.IsSuccess)
                {
                    //setting is exist and must be edit it
                    var jsonValue = model.ConvertObjectToJson();
                    result.Data.Value = string.IsNullOrEmpty(jsonValue) ? string.Empty : jsonValue;
                    _unitOfWork.SettingRepository.Update(result.Data);
                    var updateSettingResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateSettingResult.IsSuccess)
                        return CommandResult<TModel>.Success(UserMessages.DataSavedSuccessfully, model);
                    else
                        return CommandResult<TModel>.Failure(OperationResultMessage.OperationIsFailure, default(TModel));
                }
                else
                {
                    //setting is not exist and need to add new
                    string newSettingValue = model.ConvertObjectToJson();
                    Setting setting = new()
                    {
                        Type = type,
                        Value = string.IsNullOrEmpty(newSettingValue) ? string.Empty : newSettingValue
                    };
                    await _unitOfWork.SettingRepository.InsertAsync(setting, cancellationToken);
                    var addSettingResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (addSettingResult.IsSuccess)
                        return CommandResult<TModel>.Success(UserMessages.DataSavedSuccessfully, model);
                    else
                        return CommandResult<TModel>.Failure(OperationResultMessage.OperationIsFailure, default(TModel));
                }
            }
            catch (Exception)
            {
                return CommandResult<TModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, default(TModel));
            }
        }


        public async Task<CommandResult<SocialNetworkViewModel>> AddSocialNetworkAsync(SocialNetworkViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                //check is duplicate name
                if (_unitOfWork.SocialNetworkRepository.GetAllAsIQueryable().Data.Any(x => x.Title == model.Title))
                {
                    return CommandResult<SocialNetworkViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);
                }
                //save file in disk
                string path = _filePathAddress.SocialNetworkIcon;//_configuration.GetSection("FilePathAddress:SocialNetworIcon").Value;
                if (_fileService.IsExist(model.Icon, path))
                {
                    return CommandResult<SocialNetworkViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Icon), model);
                }
                else
                {
                    if (!await _fileService.UploadFileAsync(model.Icon, path))
                    {
                        return CommandResult<SocialNetworkViewModel>.Failure(OperationResultMessage.OperationIsFailureInUploadFile, model);
                    }
                }
                //save in database
                var item = new SocialNetwork
                {
                    Title = model.Title,
                    Url = model.Url,
                    ImageName = model.Icon.FileName
                };
                await _unitOfWork.SocialNetworkRepository.InsertAsync(item, cancellationToken);
                var addResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (addResult.IsSuccess)
                    return CommandResult<SocialNetworkViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                else
                    return CommandResult<SocialNetworkViewModel>.Failure(OperationResultMessage.OperationIsFailure, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<SocialNetworkViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }
        public CommandResult<IQueryable<SocialNetwork>> GetSocialNetworkAsIQueryable()
        {
            try
            {
                var result = _unitOfWork.SocialNetworkRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                    return CommandResult<IQueryable<SocialNetwork>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, result.Data);
                else
                    return CommandResult<IQueryable<SocialNetwork>>.Failure(OperationResultMessage.OperationIsFailure, null);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<SocialNetwork>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<IQueryable<GoldPrice>> GetGoldPriceAsIQueryable()
        {
            try
            {
                var result = _unitOfWork.GoldPriceRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                    return CommandResult<IQueryable<GoldPrice>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, result.Data);
                else
                    return CommandResult<IQueryable<GoldPrice>>.Failure(OperationResultMessage.OperationIsFailure, null);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<GoldPrice>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<SocialNetworkViewModel>> GetSocialNetworkByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                string path = _filePathAddress.SocialNetworkIcon;
                var result = await _unitOfWork.SocialNetworkRepository.GetByIdAsync(id, cancellationToken);
                SocialNetworkViewModel model = new SocialNetworkViewModel();
                if (result.IsSuccess)
                {
                    model.SocialNetworkId = result.Data.Id;
                    model.Title = result.Data.Title;
                    model.Icon = null;
                    model.IconUrl = $"{path.Replace("wwwroot", "")}/{result.Data.ImageName}";
                    model.Url = result.Data.Url;
                    return CommandResult<SocialNetworkViewModel>.Success(result.Message, model);
                }
                else
                {
                    return CommandResult<SocialNetworkViewModel>.Failure(result.Message, model);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<SocialNetworkViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<string>> RemoveSocialNetworkAsync(int socialNetworkId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.SocialNetworkRepository.GetByIdAsync(socialNetworkId, cancellationToken);

                if (result.IsSuccess)
                {
                    _unitOfWork.SocialNetworkRepository.Delete(result.Data);
                    var deleteResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (deleteResult.IsSuccess)
                    {
                        bool isDeletedFile = true;
                        string path = _filePathAddress.SocialNetworkIcon;// _configuration.GetSection("FilePathAddress:SocialNetworIcon").Value;
                        if (_fileService.IsExist(result.Data.ImageName, path))
                        {
                            isDeletedFile = _fileService.DeleteFile(result.Data.ImageName, path);
                        }
                        if (!isDeletedFile)
                        {
                            return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfullyButFileNotDeleted, result.Data.Title);
                        }
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, result.Data.Title);
                    }
                    else
                    {
                        return CommandResult<string>.Failure(deleteResult.Message, result.Data.Title);
                    }
                }
                else
                {
                    return CommandResult<string>.Failure(result.Message, result.Data.Title);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<string>> RemoveGoldPriceAsync(int goldPriceId, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _unitOfWork.GoldPriceRepository.GetByIdAsync(goldPriceId, cancellationToken);

                if (result.IsSuccess)
                {
                    _unitOfWork.GoldPriceRepository.Delete(result.Data);
                    var deleteResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (deleteResult.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, result.Data.Karat18.ToString("N0"));
                    else
                        return CommandResult<string>.Failure(deleteResult.Message, string.Empty);
                }
                else
                    return CommandResult<string>.Failure(result.Message, string.Empty);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<SocialNetworkViewModel>> UpdateSocialNetworkAsync(SocialNetworkViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var sosiacNetworkResult = await _unitOfWork.SocialNetworkRepository.GetByIdAsync(model.SocialNetworkId, cancellationToken);
                if (!sosiacNetworkResult.IsSuccess)
                    return CommandResult<SocialNetworkViewModel>.Failure(sosiacNetworkResult.Message, model);

                if (_unitOfWork.SocialNetworkRepository.GetAllAsIQueryable().Data.Any(x => x.Title == model.Title & x.Title != sosiacNetworkResult.Data.Title))
                {
                    return CommandResult<SocialNetworkViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);
                }


                if (model.Icon is not null)
                {
                    //check duplicate fileName
                    string path = _filePathAddress.SocialNetworkIcon;
                    if (sosiacNetworkResult.Data.ImageName == model.Icon.FileName)
                    {
                        //update file and rewrite
                        if (!await _fileService.UpdateFileAsync(model.Icon, sosiacNetworkResult.Data.ImageName, path))
                            return CommandResult<SocialNetworkViewModel>.Failure(UserMessages.ErrorInUpdateFile, model);
                    }
                    else
                    {
                        //check is duplicate
                        if (_fileService.IsExist(model.Icon.FileName, sosiacNetworkResult.Data.ImageName, path))
                            return CommandResult<SocialNetworkViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Icon), model);
                        else
                        {
                            if (!await _fileService.UpdateFileAsync(model.Icon, sosiacNetworkResult.Data.ImageName, path))
                                return CommandResult<SocialNetworkViewModel>.Failure(UserMessages.ErrorInUpdateFile, model);
                        }
                    }
                    //change image name
                    sosiacNetworkResult.Data.ImageName = model.Icon.FileName;
                }
                //save in database
                sosiacNetworkResult.Data.Title = model.Title;
                sosiacNetworkResult.Data.Url = model.Url;
                _unitOfWork.SocialNetworkRepository.Update(sosiacNetworkResult.Data);
                var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (updateResult.IsSuccess)
                    return CommandResult<SocialNetworkViewModel>.Success(UserMessages.DataEditedSuccessfully, model);
                else
                    return CommandResult<SocialNetworkViewModel>.Failure(OperationResultMessage.OperationIsFailure, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<SocialNetworkViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }


        public CommandResult<List<SelectListItem>> GetMessageTypesAsSelectListItem()
        {
            try
            {
                var allSettings = Enum.GetValues(typeof(SettingType)).Cast<SettingType>().ToList();
                List<SettingType> smsTypes = allSettings.Where(x =>
                x == SettingType.MessageType_Payment &
                x == SettingType.MessageType_OverPayment &
                x == SettingType.MessageType_DeficitPayment &
                x == SettingType.MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate &
                x == SettingType.MessageType_ReminderOfTheDayBeforeTheInstallmentDate &
                x == SettingType.MessageType_SettleDocument &
                x == SettingType.MessageType_DocumentRegistration).ToList();
                var selectListItems = smsTypes.Select(x => new SelectListItem
                {
                    Text = x.GetDisplayName(),
                    Value = x.ToString()
                }).ToList();
                return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, selectListItems);
            }
            catch (Exception)
            {
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }

        }

        public CommandResult<List<SettingType>> GetMessageTypesAsList()
        {
            try
            {
                var allSettings = Enum.GetValues(typeof(SettingType)).Cast<SettingType>().ToList();
                List<SettingType> messageTypes = allSettings.Where(x =>
                x == SettingType.MessageType_Payment |
                x == SettingType.MessageType_OverPayment |
                x == SettingType.MessageType_DeficitPayment |
                x == SettingType.MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate |
                x == SettingType.MessageType_ReminderOfTheDayBeforeTheInstallmentDate |
                x == SettingType.MessageType_SettleDocument |
                x == SettingType.MessageType_HappyBirthday |
                x == SettingType.MessageType_DocumentRegistration).ToList();
                return CommandResult<List<SettingType>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, messageTypes);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SettingType>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }
        public CommandResult<List<SettingType>> GetSiteContentTypesAsList()
        {
            try
            {
                var allSettings = Enum.GetValues(typeof(SettingType)).Cast<SettingType>().ToList();
                List<SettingType> contentTypes = allSettings.Where(x =>
                x == SettingType.SiteContent_AboutUs |
                x == SettingType.SiteContent_ContactUs |
                x == SettingType.SiteContent_BrancheList |
                x == SettingType.SiteContent_InstallmentPurchaseOfProduct).ToList();
                return CommandResult<List<SettingType>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, contentTypes);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SettingType>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }
        public CommandResult<string> GetMessageContentParameters(SettingType messageType)
        {
            try
            {
                string parameters = string.Empty;
                switch (messageType)
                {
                    case SettingType.MessageType_DocumentRegistration:
                        parameters = $"{Captions.MessageType_DocumentRegistration_ParemetersDesc}";
                        break;
                    case SettingType.MessageType_ReminderOfTheDayBeforeTheInstallmentDate:
                        parameters = $"{Captions.MessageType_ReminderOfTheDayBeforeTheInstallmentDate_ParemetersDesc}";
                        break;
                    case SettingType.MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate:
                        parameters = $"{Captions.MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate_ParemetersDesc}";
                        break;
                    case SettingType.MessageType_DeficitPayment:
                        parameters = $"{Captions.MessageType_DeficitPayment_ParemetersDesc}";
                        break;
                    case SettingType.MessageType_Payment:
                        parameters = $"{Captions.MessageType_Payment_ParemetersDesc}";
                        break;
                    case SettingType.MessageType_OverPayment:
                        parameters = $"{Captions.MessageType_PayWithExtra_ParemetersDesc}";
                        break;
                    case SettingType.MessageType_SettleDocument:
                        parameters = $"{Captions.MessageType_SettleDocument_ParemetersDesc}";
                        break;
                    case SettingType.MessageType_HappyBirthday:
                        parameters = $"{Captions.MessageType_HappyBirthday_ParemetersDesc}";
                        break;
                    default:
                        break;
                }
                return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, parameters);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        /// <summary>
        /// for show in layout
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommandResult<List<SocialNetworkModel>>> GetAllSocialNetworkAsync(CancellationToken cancellationToken = default)
        {
            List<SocialNetworkModel> List = new List<SocialNetworkModel>();
            try
            {
                var socialNetworks = await _unitOfWork.SocialNetworkRepository.GetAllAsIQueryable().Data
                    .ToListAsync(cancellationToken);
                if (socialNetworks is not null)
                {
                    List = socialNetworks.Select(x => new SocialNetworkModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Image = $"{_filePathAddress.SocialNetworkIconUrl}/{x.ImageName}",
                        Url = x.Url
                    }).ToList();
                }
                return CommandResult<List<SocialNetworkModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, List);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<SocialNetworkModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, List);
            }
        }

        public async Task<CommandResult<GoldPriceViewModel>> AddGoldPriceInfoAsync(GoldPriceViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                GoldPrice goldPrice = new() { RegisterDate = DateTime.Now };
                if (string.IsNullOrEmpty(model.Karat18))
                {
                    return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.OneGram18KaratGold), model);
                }
                else
                {
                    if (int.TryParse(model.Karat18.Replace(",", ""), out int value))
                        goldPrice.Karat18 = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.OneGram18KaratGold), model);
                }

                if (!string.IsNullOrEmpty(model.Shekel))
                {
                    if (int.TryParse(model.Shekel.Replace(",", ""), out int value))
                        goldPrice.Shekel = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.Shekel), model);
                }

                if (!string.IsNullOrEmpty(model.GramCoin))
                {
                    if (int.TryParse(model.GramCoin.Replace(",", ""), out int value))
                        goldPrice.GramCoin = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.GramCoin), model);
                }

                if (!string.IsNullOrEmpty(model.GramCoin))
                {
                    if (int.TryParse(model.GramCoin.Replace(",", ""), out int value))
                        goldPrice.GramCoin = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.GramCoin), model);
                }

                if (!string.IsNullOrEmpty(model.Coin))
                {
                    if (int.TryParse(model.Coin.Replace(",", ""), out int value))
                        goldPrice.Coin = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.BankCoin), model);
                }

                if (!string.IsNullOrEmpty(model.OldCoin))
                {
                    if (int.TryParse(model.OldCoin.Replace(",", ""), out int value))
                        goldPrice.OldCoin = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.OldCoin), model);
                }

                if (!string.IsNullOrEmpty(model.HalfCoin))
                {
                    if (int.TryParse(model.HalfCoin.Replace(",", ""), out int value))
                        goldPrice.HalfCoin = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.BankHalfCoin), model);
                }
                if (!string.IsNullOrEmpty(model.QuarterCoin))
                {
                    if (int.TryParse(model.QuarterCoin.Replace(",", ""), out int value))
                        goldPrice.QuarterCoin = value;
                    else
                        return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.BankQuarterCoin), model);
                }
                if (model.UserId.HasValue)
                    goldPrice.UserId = model.UserId.Value;
                else
                    return CommandResult<GoldPriceViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.User), model);

                goldPrice.Anas = model.Anas;

                await _unitOfWork.GoldPriceRepository.InsertAsync(goldPrice, cancellationToken);
                var addResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (addResult.IsSuccess)
                    return CommandResult<GoldPriceViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                else
                    return CommandResult<GoldPriceViewModel>.Failure(OperationResultMessage.OperationIsFailure, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<GoldPriceViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<GoldPriceViewModel?>> GetLastGoldPriceInfoAsync(CancellationToken cancellationToken)
        {
            try
            {
                GoldPriceViewModel? model = await _unitOfWork.GoldPriceRepository.GetAllAsIQueryable().Data
                    .OrderByDescending(x => x.Id)
                    .Select(x => new GoldPriceViewModel
                    {
                        Anas = x.Anas,
                        Coin = x.Coin.HasValue ? x.Coin.Value.ToString("N0") : string.Empty,
                        GramCoin = x.GramCoin.HasValue ? x.GramCoin.Value.ToString("N0") : string.Empty,
                        HalfCoin = x.HalfCoin.HasValue ? x.HalfCoin.Value.ToString("N0") : string.Empty,
                        Karat18 = x.Karat18.ToString("N0"),
                        OldCoin = x.OldCoin.HasValue ? x.OldCoin.Value.ToString("N0") : string.Empty,
                        QuarterCoin = x.QuarterCoin.HasValue ? x.QuarterCoin.Value.ToString("N0") : string.Empty,
                        Shekel = x.Shekel.HasValue ? x.Shekel.Value.ToString("N0") : string.Empty,
                    }).FirstOrDefaultAsync(cancellationToken);
                if (model is not null)
                    return CommandResult<GoldPriceViewModel?>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                else
                    return CommandResult<GoldPriceViewModel?>.Failure(string.Format(OperationResultMessage.NotFound, Captions.GoldPrice), null);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<GoldPriceViewModel?>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<GoldPriceInfoModel>> GetGoldPriceInfoAsync(CancellationToken cancellationToken)
        {
            try
            {
                var model = await _unitOfWork.GoldPriceRepository.GetAllAsIQueryable().Data
                       .OrderByDescending(x => x.RegisterDate)
                       .Select(x => new GoldPriceInfoModel
                       {
                           Anas = x.Anas,
                           Coin = x.Coin,
                           GramCoin = x.GramCoin,
                           HalfCoin = x.HalfCoin,
                           Karat18 = x.Karat18,
                           OldCoin = x.OldCoin,
                           QuarterCoin = x.QuarterCoin,
                           Shekel = x.Shekel,
                           RegisterDate = x.RegisterDate,
                       }).FirstOrDefaultAsync(cancellationToken);
                if (model is not null)
                    return CommandResult<GoldPriceInfoModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                else
                    return CommandResult<GoldPriceInfoModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.GoldPrice), null);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<GoldPriceInfoModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<CalculatedGoldPriceModel>> GetGoldPriceAsync(GoldCalculatorViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = await GetSettingAsync<LoanSettingsViewModel>(SettingType.LoanSetting, cancellationToken);
                if (result.IsSuccess)
                {
                    
                    if (string.IsNullOrEmpty(model.Tax)| string.IsNullOrEmpty(model.StonePrice)| 
                        string.IsNullOrEmpty(model.GramsGoldPrice) | string.IsNullOrEmpty(model.Weight)|
                        string.IsNullOrEmpty(model.Wage)| string.IsNullOrEmpty(model.GalleryProfit))
                        return CommandResult<CalculatedGoldPriceModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());

                    decimal tax, gramsGoldPrice, stonePrice, weight, wage, galleryProfit;

                    if(!decimal.TryParse(model.Tax,out tax)| !decimal.TryParse(model.StonePrice, out stonePrice) |
                       !decimal.TryParse(model.GramsGoldPrice, out gramsGoldPrice) | !decimal.TryParse(model.Weight, out weight)|
                       !decimal.TryParse(model.Wage, out wage) | !decimal.TryParse(model.GalleryProfit, out galleryProfit))
                        return CommandResult<CalculatedGoldPriceModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());

                    decimal goldPrice = CalcGoldPrice(gramsGoldPrice, weight);
                    decimal wageAmount = CalcWageAmount(goldPrice, wage);
                    decimal galleryProfitAmount=CalcGalleryProfitAmount(stonePrice,wageAmount,goldPrice,galleryProfit);
                    decimal taxAmount = CalcTax(wageAmount, galleryProfitAmount, tax);
                    decimal invoceTotalAmount = InvoceTotalAmount(goldPrice, wageAmount, galleryProfitAmount, taxAmount, stonePrice);

                    long roundedInvoceTotalAmount = (long)invoceTotalAmount;
                    long roundedGoldPrice=(long)goldPrice;
                    CalculatedGoldPriceModel priceModel = new()
                    {
                        InvoiceTotalPrice=NumberTools.RoundNumber(roundedInvoceTotalAmount.ToString(),3).ToString("N0"),
                        PureGoldPrice= NumberTools.RoundNumber(roundedGoldPrice.ToString(), 3).ToString("N0")
                    };
                    return CommandResult<CalculatedGoldPriceModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, priceModel);
                }
                else
                {
                    return CommandResult<CalculatedGoldPriceModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CalculatedGoldPriceModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }


        public CommandResult<long>  CalcInvoiceTotalPrice(decimal gramsGoldPrice,decimal weight,decimal wage,decimal stonePrice,decimal galleryProfit,decimal tax)
        {
            try
            {
                decimal goldPrice = CalcGoldPrice(gramsGoldPrice, weight);
                decimal wageAmount = CalcWageAmount(goldPrice, wage);
                decimal galleryProfitAmount = CalcGalleryProfitAmount(stonePrice, wageAmount, goldPrice, galleryProfit);
                decimal taxAmount = CalcTax(wageAmount, galleryProfitAmount, tax);
                decimal invoceTotalAmount = InvoceTotalAmount(goldPrice, wageAmount, galleryProfitAmount, taxAmount, stonePrice);

                long roundedInvoceTotalAmount = (long)invoceTotalAmount;
                //long roundedGoldPrice = (long)goldPrice;
                return CommandResult<long>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, roundedInvoceTotalAmount);
            }
            catch (Exception ex)
            {
                 _logManager.RaiseLog(ex);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
            
        }

        /// <summary>
        /// قیمت طلا
        /// </summary>
        /// <param name="amount">مبلغ</param>
        /// <param name="weight">وزن</param>
        /// <returns></returns>
        public decimal CalcGoldPrice(decimal amount,decimal weight)
        {
            return amount * weight;
        }

        /// <summary>
        /// مبلغ اجرت
        /// </summary>
        /// <param name="goldPrice">قیمت طلا</param>
        /// <param name="wage">اجرت</param>
        /// <returns></returns>
        public decimal CalcWageAmount(decimal goldPrice,decimal wage)
        {
            return goldPrice * wage/100;
        }

        /// <summary>
        /// مبلغ سود گالری
        /// </summary>
        /// <param name="stonePrice">ارزش سنگ</param>
        /// <param name="wageAmount">مبلغ اجرت</param>
        /// <param name="goldPrice">قیمت طلا</param>
        /// <param name="galleryProfit">سود گالری</param>
        /// <returns></returns>
        public decimal CalcGalleryProfitAmount(decimal stonePrice,decimal wageAmount,decimal goldPrice,decimal galleryProfit)
        {
            return (stonePrice+wageAmount+goldPrice)*galleryProfit/100;
        }

        /// <summary>
        /// مالیات بر ارزش افزوده
        /// </summary>
        /// <param name="wageAmount">مبلغ اجرت</param>
        /// <param name="galleryProfitAmount">ملغ سود گالری</param>
        /// <param name="tax">مالیات</param>
        /// <returns></returns>
        public decimal CalcTax(decimal wageAmount,decimal galleryProfitAmount,decimal tax)
        {
            return (wageAmount+galleryProfitAmount)*tax/100;
        }

        /// <summary>
        /// جمع نهایی فاکتور
        /// </summary>
        /// <param name="goldPrice">قیمت طلا</param>
        /// <param name="wageAmount">مبلغ اجرت</param>
        /// <param name="galleryProfitAmount">مبلغ سود گالری</param>
        /// <param name="taxAmount">مبلغ مالیات</param>
        /// <param name="stonePrice">ارزش سنگ</param>
        /// <returns></returns>
        public decimal InvoceTotalAmount(decimal goldPrice,decimal wageAmount,decimal galleryProfitAmount, decimal taxAmount, decimal stonePrice)
        {
            return goldPrice + wageAmount + galleryProfitAmount + taxAmount + stonePrice;
        }
    }
}
