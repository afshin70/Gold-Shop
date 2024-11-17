using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CityModels;
using Gold.ApplicationService.Contract.DTOs.Models.GalleryModels;
using Gold.ApplicationService.Contract.DTOs.Models.SellerModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GalleryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SellerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Infrastracture.ExternalService;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Gold.ApplicationService.Concrete
{
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;
        private readonly IConfiguration _configuration;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;
        public SellerService(IUnitOfWork unitOfWork, ILogManager logManager, IConfiguration configuration, IFileService fileService,
            IOptions<FilePathAddress> filePathAddressOptions)
        {
            this._unitOfWork = unitOfWork;
            this._logManager = logManager;
            _configuration = configuration;
            this._fileService = fileService;
            this._filePathAddress = filePathAddressOptions.Value;
        }
        public async Task<CommandResult<CreateOrEditSellerViewModel>> CreateSellerAsync(CreateOrEditSellerViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Password))
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Password), model);
                model.GalleryId ??= 0;
                var galleryResult = await _unitOfWork.GalleryRepository.GetByIdAsync(model.GalleryId.Value, cancellationToken);
                if (!galleryResult.IsSuccess)
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(UserMessages.GalleryIsNotValid, model);

                var isDuplicateUserNameResult = await _unitOfWork.UserRepository.GetUserByUsernameAsync(model.UserName, cancellationToken);
                if (isDuplicateUserNameResult.IsSuccess)
                {
                    //exist user by username
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.UserName), model);
                }

                //check duplicate name in gallery
                if (_unitOfWork.SellerRepository.GetAllAsIQueryable().Data.Any(x => x.GalleryId == model.GalleryId & x.User.FullName == model.FullName))
                {
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Name), model);
                }

                string saltKey = Guid.NewGuid().ToString();
                string imageName = Guid.NewGuid().ToString();
                User user = new()
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    PasswordSalt = saltKey,
                    PasswordHash = Encryptor.Encrypt(model.Password, saltKey),
                    IsActive = model.IsActive,
                    Mobile = model.Mobile,
                    IsLocked = false,
                    RegisterDate = DateTime.Now,
                    UserType = UserType.Seller,
                    Seller = new()
                    {
                        GalleryId = model.GalleryId.Value,
                        HasAccessToRegisterLoan = model.HasAccessToRegisterLoan,
                        ImageName = (model.Image is null) ? string.Empty : $"{imageName}{Path.GetExtension(model.Image.FileName)}",
                        ProductRegisterPerHourCount = model.ProductRegisterPerHourCount.Value,
                        HasAccessToRegisterProduct = model.HasAccessToRegisterProduct,
                    },
                    SecurityStamp = Guid.NewGuid(),
                };
                //save image
                if (model.Image is not null)
                {
                    string path = _filePathAddress.SellerProfileImage;// _configuration.GetSection("FilePathAddress:SellerProfileImage").Value;
                                                                      //update file
                    if (!await _fileService.UploadFileAsync(model.Image, path, imageName))
                        return CommandResult<CreateOrEditSellerViewModel>.Failure(UserMessages.ErrorInUploadFile, model);
                }
                var addResult = await _unitOfWork.AddAsync(user, cancellationToken);
                if (addResult.IsSuccess)
                {
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult<CreateOrEditSellerViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                    else
                        return CommandResult<CreateOrEditSellerViewModel>.Failure(result.Message, model);
                }
                else
                {
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(addResult.Message, model);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditSellerViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<CreateOrEditSellerViewModel>> EditSellerInfoAsync(CreateOrEditSellerViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var galleryResult = await _unitOfWork.GalleryRepository.GetByIdAsync(model.GalleryId.Value, cancellationToken);
                if (!galleryResult.IsSuccess)
                {
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(UserMessages.GalleryIsNotFound, model);
                }
                var sellerResult = await _unitOfWork.SellerRepository.GetByIdAsync(model.SellerId, cancellationToken);

                if (!sellerResult.IsSuccess)
                {
                    //seller not found
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(UserMessages.SellerNotFound, model);
                }
                var userResult = await _unitOfWork.UserRepository.GetByIdAsync(sellerResult.Data.UserId, cancellationToken);
                if (!userResult.IsSuccess)
                {
                    //user not found
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(UserMessages.UserNotFound, model);
                }
                //check duplicate userName
                if (model.UserName != userResult.Data.UserName)
                {
                    //if (_unitOfWork.UserRepository.GetAllAsIQueryable().Data.Any(x => x.UserName == model.UserName))
                    //    return CommandResult<CreateOrEditSellerViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.UserName), model);

                    var isDuplicateUserNameResult = await _unitOfWork.UserRepository.GetUserByUsernameAsync(model.UserName, cancellationToken);
                    if (isDuplicateUserNameResult.IsSuccess)
                    {
                        //exist user by username
                        return CommandResult<CreateOrEditSellerViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.UserName), model);
                    }

                }

                //check duplicate name in gallery
                if (model.FullName != userResult.Data.FullName)
                {
                    if (_unitOfWork.SellerRepository.GetAllAsIQueryable().Data.Any(x => x.GalleryId == model.GalleryId & x.User.FullName == model.FullName))
                    {
                        return CommandResult<CreateOrEditSellerViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Name), model);
                    }
                }

                //save or delete image
                string newImageName = Guid.NewGuid().ToString();
                if (model.Image is not null)
                {
                    string path = _filePathAddress.SellerProfileImage;// _configuration.GetSection("FilePathAddress:SellerProfileImage").Value;
                                                                      //update file
                    string lastImageName = string.IsNullOrEmpty(sellerResult.Data.ImageName) ? string.Empty : sellerResult.Data.ImageName;
                    if (!await _fileService.UpdateFileAsync(model.Image, newImageName, lastImageName, path))
                        return CommandResult<CreateOrEditSellerViewModel>.Failure(UserMessages.ErrorInUpdateFile, model);
                    sellerResult.Data.ImageName = $"{newImageName}{Path.GetExtension(model.Image.FileName)}";
                }
                else
                {
                    if (model.IsDeleteImage)
                    {
                        if (_fileService.DeleteFile(sellerResult.Data.ImageName, _filePathAddress.SellerProfileImage))
                        {
                            sellerResult.Data.ImageName = null;
                        }
                        else
                        {
                            return CommandResult<CreateOrEditSellerViewModel>.Failure(UserMessages.ErrorInDeleteFile, model);
                        }
                    }
                }
                //update data
                sellerResult.Data.ProductRegisterPerHourCount = model.ProductRegisterPerHourCount.Value;
                sellerResult.Data.HasAccessToRegisterLoan = model.HasAccessToRegisterLoan;
                sellerResult.Data.GalleryId = model.GalleryId.Value;
                sellerResult.Data.HasAccessToRegisterProduct = model.HasAccessToRegisterProduct;

                //update user
                userResult.Data.Mobile = model.Mobile;
                userResult.Data.UserName = model.UserName;
                userResult.Data.FullName = model.FullName;
                userResult.Data.IsActive = model.IsActive;
                //change user password
                if (!string.IsNullOrEmpty(model.Password))
                {
                    userResult.Data.PasswordHash = Encryptor.Encrypt(model.Password, userResult.Data.PasswordSalt);
                    userResult.Data.LastPasswordChangeDate = DateTime.Now;
                    userResult.Data.SecurityStamp = Guid.NewGuid();
                }

                _unitOfWork.UserRepository.Update(userResult.Data);
                _unitOfWork.SellerRepository.Update(sellerResult.Data);

                var addResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (addResult.IsSuccess)
                    return CommandResult<CreateOrEditSellerViewModel>.Success(UserMessages.DataEditedSuccessfully, model);
                else
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(addResult.Message, model);


            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditSellerViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<GalleryModel>> GetSellerGalleryAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var sellerGallery = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == userId)
                    .Include(x => x.Seller).ThenInclude(x => x.Gallery)
                    .Select(x => new GalleryModel
                    {
                        Id = x.Seller.Gallery.Id,
                        Name = x.Seller.Gallery.Name
                    }).FirstOrDefaultAsync(cancellationToken);
                if (sellerGallery is not null)
                    return CommandResult<GalleryModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, sellerGallery);
                else
                    return CommandResult<GalleryModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.Gallery), new());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<GalleryModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<string>> GetSellerGalleryNameAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var galleryName = await _unitOfWork.SellerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == id)
                    .Include(x => x.Gallery)
                    .Select(x => x.Gallery.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                if (string.IsNullOrEmpty(galleryName))
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.Seller), string.Empty);
                else
                    return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, galleryName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<CreateOrEditSellerViewModel>> GetSellerInfoForEditAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                string path = _filePathAddress.SellerProfileImage;// _configuration.GetSection("FilePathAddress:SellerProfileImage").Value;
                var seller = await _unitOfWork.SellerRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == id)
                    .Include(x => x.User)
                    .Include(x => x.Gallery)
                    .Select(x => new CreateOrEditSellerViewModel
                    {
                        SellerId = x.Id,
                        FullName = x.User.FullName,
                        GalleryId = x.GalleryId,
                        IsActive = x.User.IsActive,
                        ProductRegisterPerHourCount = x.ProductRegisterPerHourCount,
                        UserName = x.User.UserName,
                        HasAccessToRegisterLoan = x.HasAccessToRegisterLoan,
                        HasAccessToRegisterProduct = x.HasAccessToRegisterProduct,
                        ImageNameUrl = $"/Seller/SellerImage/{x.ImageName}",
                        Mobile = x.User.Mobile,
                        Password = string.Empty,
                        ConfirmPassword = string.Empty,
                        ImageName = x.ImageName,
                    }).FirstOrDefaultAsync();

                if (seller is not null)
                {
                    return CommandResult<CreateOrEditSellerViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, seller);
                }
                else
                {
                    return CommandResult<CreateOrEditSellerViewModel>.Failure(OperationResultMessage.OperationIsFailure, null);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditSellerViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<IQueryable<SellerModel>> GetSellerListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.SellerRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Include(x => x.User).Include(x => x.Gallery).Select(x => new SellerModel
                    {
                        Id = x.Id,
                        FullName = x.User.FullName,
                        GalleryId = x.Id,
                        GalleryName = x.Gallery.Name,
                        IsActive = x.User.IsActive,
                        ProductRegisterPerMinCount = x.ProductRegisterPerHourCount,
                        UserName = x.User.UserName,
                        HasAccessToRegisterProduct = x.HasAccessToRegisterProduct,
                        HasAccessToRegisterLoan = x.HasAccessToRegisterLoan
                    });
                    return CommandResult<IQueryable<SellerModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<SellerModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<SellerModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public CommandResult<IQueryable<SellerReportModel>> GetSellerReportListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.SellerRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Include(x => x.User).Include(x => x.Gallery).Select(x => new SellerReportModel
                    {
                        Id = x.Id,
                        FullName = x.User.FullName,
                        GalleryName = x.Gallery.Name,
                        IsActive = x.User.IsActive,
                        ProductRegisterPerMinCount = x.ProductRegisterPerHourCount,
                        UserName = x.User.UserName,
                        Mobile = x.User.Mobile,
                        HasAccessToRegisterLoan = x.HasAccessToRegisterLoan,
                        HasAccessToRegisterProduct = x.HasAccessToRegisterProduct,
                    });
                    return CommandResult<IQueryable<SellerReportModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<SellerReportModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<SellerReportModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetSellersOfGalleryAsync(int galleryId, int selectedId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.SellerRepository.GetAllByGalleryIdAsync(galleryId, cancellationToken);
                if (result.IsSuccess)
                {
                    var list = result.Data.Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.User.FullName,
                        Selected = (selectedId == x.Id)
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(UserMessages.ErrorInLoadSellerList, null);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }
        
        public async Task<CommandResult<List<SelectListItem>>> GetActiveSellersOfGalleryAsync(int galleryId, int selectedId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.SellerRepository.GetAllByGalleryIdAsync(galleryId, cancellationToken);
                if (result.IsSuccess)
                {
                    var list = result.Data.Where(x=>x.HasAccessToRegisterLoan).Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.User.FullName,
                        Selected = (selectedId == x.Id)
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(UserMessages.ErrorInLoadSellerList, null);
                }
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<bool>> HasAllowToRegisterProductAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var allowToRegisterProductState = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == userId)
                    .Include(x => x.Seller)
                    .Select(x => x.Seller.HasAccessToRegisterProduct)
                    .FirstOrDefaultAsync(cancellationToken);
                if (allowToRegisterProductState)
                    return CommandResult<bool>.Success(UserMessages.YouAreAllowedToRegisterProduct, true);
                else
                    return CommandResult<bool>.Failure(UserMessages.YouAreNotAllowedToRegisterProduct, false);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<bool>> IsAllowToRegisterProductAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var seller = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == userId)
                    .Include(x => x.Seller)
                    .Select(x => x.Seller)
                    .FirstOrDefaultAsync(cancellationToken);
                if (seller is null)
                    return CommandResult<bool>.Failure(string.Format(UserMessages.NotFound, Captions.Seller), false);

                #region چک کردن اجازه ثبت محصول
                var sellerProductsInLastHour = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                    .Where(x => x.RegistrarUserId == seller.UserId &
                                x.RegisterDate > DateTime.Now.AddHours(-1) & x.RegisterDate < DateTime.Now)
                    .CountAsync(cancellationToken);

                //چک کردن محدودیت ثبت کالا
                if (sellerProductsInLastHour > seller.ProductRegisterPerHourCount)
                    return CommandResult<bool>.Failure(UserMessages.AllowedLimitForProductRegistrationHasBeenFilled, false);
                else
                    return CommandResult<bool>.Success(UserMessages.AllowedForProductRegistration, true);
                #endregion
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<bool>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, false);
            }
        }

        public async Task<CommandResult<byte>> GetProductRegisterPerHourCountAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var productRegisterPerHourCount = await _unitOfWork.UserRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == userId)
                    .Include(x => x.Seller)
                    .Select(x => x.Seller.ProductRegisterPerHourCount)
                    .FirstOrDefaultAsync(cancellationToken);
                return CommandResult<byte>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, productRegisterPerHourCount);
            }
            catch (Exception ex)
            {
                //log Exception
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<byte>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        public async Task<CommandResult<string>> RemoveSellerAsync(int sellerId, CancellationToken cancellationToken)
        {
            try
            {
                var sellerResult = await _unitOfWork.SellerRepository.GetByIdAsync(sellerId, cancellationToken);
                if (!sellerResult.IsSuccess)
                {
                    //seller not found
                    return CommandResult<string>.Failure(sellerResult.Message, string.Empty);
                }
                var userResult = await _unitOfWork.UserRepository.GetByIdAsync(sellerResult.Data.UserId, cancellationToken);
                if (!userResult.IsSuccess)
                {
                    //user not found
                    return CommandResult<string>.Failure(userResult.Message, string.Empty);
                }
                //remove user and seller
                _unitOfWork.SellerRepository.Delete(sellerResult.Data);
                _unitOfWork.UserRepository.Delete(userResult.Data);

                var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (removeResult.IsSuccess)
                    return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, userResult.Data.FullName);
                else
                    return CommandResult<string>.Failure(removeResult.Message, string.Empty);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }
    }
}
