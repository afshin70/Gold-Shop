using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.GalleryModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GalleryViewModels;
using Gold.Domain.Entities;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Concrete
{
    public class GalleryService : IGalleryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;

        public GalleryService(IUnitOfWork unitOfWork, ILogManager logManager)
        {
            this._unitOfWork = unitOfWork;
            this._logManager = logManager;
        }

        public async Task<CommandResult<CreateOrEditGalleryViewModel>> CreateGalleryAsync(CreateOrEditGalleryViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var isDuplicateResult = await _unitOfWork.GalleryRepository.IsExistByNameAsync(model.Name, cancellationToken);
                if (isDuplicateResult.IsSuccess)
                {
                    if (isDuplicateResult.Data)
                    {
                        //gallery is exist
                        return CommandResult<CreateOrEditGalleryViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Name), model);
                    }
                    var gallery = new Gallery
                    {
                        Address = model.Address,
                        PurchaseDescription = model.PurchaseDescription,
                        Name = model.Name,
                        Tel = model.Tel,
                        IsActive = model.IsActive,
                        HasInstallmentSale = model.HasInstallmentSale,
                    };
                    await _unitOfWork.GalleryRepository.InsertAsync(gallery, cancellationToken);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<CreateOrEditGalleryViewModel>.Success(UserMessages.DataSavedSuccessfully, model);
                    else
                        return CommandResult<CreateOrEditGalleryViewModel>.Failure(result.Message, model);
                }
                else
                {
                    return CommandResult<CreateOrEditGalleryViewModel>.Failure(isDuplicateResult.Message, model);
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditGalleryViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<CreateOrEditGalleryViewModel>> EditGalleryAsync(CreateOrEditGalleryViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var galleryResult = await _unitOfWork.GalleryRepository.GetByIdAsync(model.Id, cancellationToken);
                if (galleryResult.IsSuccess)
                {
                    if (galleryResult.Data.Name!=model.Name)
                    {
                        var isDuplicatResult = await _unitOfWork.GalleryRepository.IsExistByNameAsync(model.Name, cancellationToken);
                        if (isDuplicatResult.IsSuccess)
                        {
                            if (isDuplicatResult.Data)
                                return CommandResult<CreateOrEditGalleryViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Name), model);
                        }
                        else
                        {
                            return CommandResult<CreateOrEditGalleryViewModel>.Failure(isDuplicatResult.Message, model);
                        }
                    }
                    //gallery is exist
                    galleryResult.Data.Address = model.Address;
                    galleryResult.Data.PurchaseDescription = model.PurchaseDescription;
                    galleryResult.Data.Name = model.Name;
                    galleryResult.Data.Tel = model.Tel;
                    galleryResult.Data.IsActive = model.IsActive;
                    galleryResult.Data.HasInstallmentSale = model.HasInstallmentSale;

                    _unitOfWork.GalleryRepository.Update(galleryResult.Data);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<CreateOrEditGalleryViewModel>.Success(UserMessages.DataEditedSuccessfully, model);
                    else
                        return CommandResult<CreateOrEditGalleryViewModel>.Failure(result.Message, model);
                }
                else
                {
                    //gallery is not exist
                    return CommandResult<CreateOrEditGalleryViewModel>.Failure(galleryResult.Message, model);
                }
                
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditGalleryViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<CreateOrEditGalleryViewModel>> GetGalleryInfoForEditAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var galleryResult = await _unitOfWork.GalleryRepository.GetByIdAsync(id, cancellationToken);
                if (galleryResult.IsSuccess)
                {
                    CreateOrEditGalleryViewModel model = new()
                    {
                        Id = galleryResult.Data.Id,
                        Address = galleryResult.Data.Address,
                        HasInstallmentSale = galleryResult.Data.HasInstallmentSale,
                        IsActive = galleryResult.Data.IsActive,
                        Name = galleryResult.Data.Name,
                        PurchaseDescription = galleryResult.Data.PurchaseDescription,
                        Tel = galleryResult.Data.Tel,
                    };
                    return CommandResult<CreateOrEditGalleryViewModel>.Success(galleryResult.Message, model);
                }
                else
                {
                    return CommandResult<CreateOrEditGalleryViewModel>.Failure(galleryResult.Message, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditGalleryViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public CommandResult<IQueryable<GalleryModel>> GetGalleryListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.GalleryRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new GalleryModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        HasInstallmentSale=x.HasInstallmentSale
                    });
                    return CommandResult<IQueryable<GalleryModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<GalleryModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<GalleryModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }
        public CommandResult<IQueryable<GalleryReportModel>> GetGalleryReportListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.GalleryRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new GalleryReportModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        HasInstallmentSale = x.HasInstallmentSale,
                        Address=x.Address,
                        PurchaseDescription=x.PurchaseDescription,
                        Tel=x.Tel
                    });
                    return CommandResult<IQueryable<GalleryReportModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<GalleryReportModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<GalleryReportModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<string>> RemoveGalleryAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var galleryResult = await _unitOfWork.GalleryRepository.GetByIdAsync(id, cancellationToken);
                if (galleryResult.IsSuccess)
                {
                    //remove user and customer and essentialTels
                    _unitOfWork.GalleryRepository.Delete(galleryResult.Data);

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully,galleryResult.Data.Name);/*string.Format(UserMessages.Gallery0IsDeleted, galleryResult.Data.Name)*/
                    else
                        return CommandResult<string>.Failure(result.Message,galleryResult.Data.Name);
                }
                else
                {
                    return CommandResult<string>.Failure(galleryResult.Message,string.Empty);
                }
                
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware,string.Empty);
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetActiveGalleriesAsync(int selectedId=0,CancellationToken cancellationToken = default)
        {
            try
            {

                CommandResult<List<Gallery>> galleries = await _unitOfWork.GalleryRepository.GetAllAsync(cancellationToken);
                if (galleries.IsSuccess)
                {
                    var selectList = galleries.Data.Where(x => x.IsActive).OrderBy(x=>x.Name).Select(x => new SelectListItem
                    {
                        Selected=(x.Id== selectedId),
                        Text=x.Name,
                        Value=x.Id.ToString()
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, selectList);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(galleries.Message, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetAllGalleriesAsync(int selectedId=0, CancellationToken cancellationToken=default)
        {
            try
            {

                CommandResult<List<Gallery>> galleries = await _unitOfWork.GalleryRepository.GetAllAsync(cancellationToken);
                if (galleries.IsSuccess)
                {
                    var selectList = galleries.Data.OrderBy(x => x.Name).Select(x => new SelectListItem
                    {
                        Selected = (x.Id == selectedId),
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, selectList);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(galleries.Message, new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<string>> GetGalleryNameAsync(int? galleryId, CancellationToken cancellationToken)
        {
            try
            {
                galleryId ??= 0;
				var galleryName=await _unitOfWork.GalleryRepository.GetAllAsIQueryable().Data
                    .Where(x=>x.Id==galleryId.Value)
                    .Select(x=>x.Name)
                    .FirstOrDefaultAsync(cancellationToken);
                if (string.IsNullOrEmpty(galleryName))
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound,Captions.Gallery), string.Empty);
                else
                    return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, galleryName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }
    }
}
