using Gold.ApplicationService.Contract.DTOs.Models.ProductModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels.CategoryModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels.ProductGallertModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.CategoryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.ProdctGalleryViewModels;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gold.ApplicationService.Contract.Abstractions
{
    public interface IProductManagementService
    {
        Task<CommandResult> ChangeCategoryOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken);
        Task<CommandResult> ChangeGalleryOrderNumberAsync(long id, bool isUp, CancellationToken cancellationToken);
        Task<CommandResult<string>> ChangeProductStatusAsync(long id, int? userId = null, CancellationToken cancellationToken = default);
        Task<CommandResult<CreateOrEditCategoryViewModel>> CreateOrEditCategoryAsync(CreateOrEditCategoryViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateOrEditProductViewModel>> CreateOrEditProductAsync(CreateOrEditProductViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<CreateProductGalleryViewModel>> CreateProductGalleryAsync(CreateProductGalleryViewModel model, CancellationToken cancellationToken);
        Task<CommandResult<List<SelectListItem>>> GetAllCategoriesAsync(int selectedId, CancellationToken cancellationToken);
        Task<CommandResult<List<CategoryModel>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
        Task<CommandResult<List<ProductGalleryModel>>> GetAllProductGalleryByProductIdAsync(long productId, CancellationToken cancellationToken);
        CommandResult<IQueryable<CategoryModel>> GetCategoryAsQuerable();
        Task<CommandResult<CategoryModel>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
        CommandResult<IQueryable<ProductModel>> GetProductAsQuerable(int? userId = null);
        Task<CommandResult<ProductModel>> GetProductByIdAsync(long id, int? userId=null, CancellationToken cancellationToken=default);
        Task<CommandResult<ProductGalleryModel>> GetProductGalleryByIdAsync(long id, CancellationToken cancellationToken);
        Task<CommandResult<ProductInfoForShowInSiteModel>> GetProductInfoAsync(long productId, CancellationToken cancellationToken);
        Task<CommandResult<List<ProductForShowInSiteModel>>> GetProductsByFilterAsync(ProductsFilterModel model);
        Task<CommandResult<InstallmentPurchaseModel>> InstallmentPurchaseAsync(InstallmentPurchaseInputDataModel model, CancellationToken cancellationToken);
        Task<CommandResult<string>> IsVisitedAsync(long productId, CancellationToken cancellationToken);
        CommandResult<long> PrepaymentCalculation(long galleryProfitAmount, long taxAmount, long wageAmount, decimal weight, long productAmount);
        Task<CommandResult<string>> RemoveCategoryAsync(int id, CancellationToken cancellationToken);
        Task<CommandResult<ProductModel>> RemoveProductAsync(long id, int? userId=null, CancellationToken cancellationToken=default);
        Task<CommandResult<string>> RemoveProductGalleryAsync(long id, CancellationToken cancellationToken);
        Task<CommandResult> SetProductGalleryThubmnailAsync(long id, CancellationToken cancellationToken);
    }
}
