using Gold.ApplicationService.Contract.DTOs.Models.GalleryModels;
using Gold.ApplicationService.Contract.DTOs.Models.SellerModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GalleryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SellerViewModels;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface ISellerService
    {
        Task<CommandResult<CreateOrEditSellerViewModel>> CreateSellerAsync(CreateOrEditSellerViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditSellerViewModel>> EditSellerInfoAsync(CreateOrEditSellerViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<List<SelectListItem>>> GetActiveSellersOfGalleryAsync(int galleryId, int selectedId, CancellationToken cancellationToken);
        Task<CommandResult<byte>> GetProductRegisterPerHourCountAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult<GalleryModel>> GetSellerGalleryAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult<string>> GetSellerGalleryNameAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditSellerViewModel>> GetSellerInfoForEditAsync(int id, CancellationToken cancellationToken);
        CommandResult<IQueryable<SellerModel>> GetSellerListAsQuerable();
        CommandResult<IQueryable<SellerReportModel>> GetSellerReportListAsQuerable();
        Task<CommandResult<List<SelectListItem>>> GetSellersOfGalleryAsync(int galleryId, int selectedId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> HasAllowToRegisterProductAsync(int userId, CancellationToken cancellationToken);
        Task<CommandResult<bool>> IsAllowToRegisterProductAsync(int userId, CancellationToken cancellationToken = default);
        Task<CommandResult<string>> RemoveSellerAsync(int sellerId, CancellationToken cancellationToken);
    }
}