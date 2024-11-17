using Gold.ApplicationService.Contract.DTOs.Models.GalleryModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GalleryViewModels;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IGalleryService
    {
        Task<CommandResult<CreateOrEditGalleryViewModel>> CreateGalleryAsync(CreateOrEditGalleryViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditGalleryViewModel>> EditGalleryAsync(CreateOrEditGalleryViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<List<SelectListItem>>> GetActiveGalleriesAsync(int selectedId=0, CancellationToken cancellationToken = default);
        Task<CommandResult<List<SelectListItem>>> GetAllGalleriesAsync(int selectedId=0, CancellationToken cancellationToken= default);
        Task<CommandResult<CreateOrEditGalleryViewModel>> GetGalleryInfoForEditAsync(int id, CancellationToken cancellationToken);
        CommandResult<IQueryable<GalleryModel>> GetGalleryListAsQuerable();
        Task<CommandResult<string>> GetGalleryNameAsync(int? galleryId, CancellationToken cancellationToken);
        CommandResult<IQueryable<GalleryReportModel>> GetGalleryReportListAsQuerable();
        Task<CommandResult<string>> RemoveGalleryAsync(int id, CancellationToken cancellationToken);
    }
}