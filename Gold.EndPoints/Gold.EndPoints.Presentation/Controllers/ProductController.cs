using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels.CategoryModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Entities;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Web;

namespace Gold.EndPoints.Presentation.Controllers
{
    public class ProductController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly ISettingService _settingService;
        private readonly IProductManagementService _productManagement;
        private readonly ICustomerService _customerService;
        private readonly FilePathAddress _filePathAddress;
        private readonly IFileService _fileService;
        public ProductController(IViewRenderService renderService, ISettingService settingService,
            IProductManagementService productManagement, IOptions<FilePathAddress> filePathAddressOptions, IFileService fileService, ICustomerService customerService)
        {
            _renderService = renderService;
            _settingService = settingService;
            _productManagement = productManagement;
            _filePathAddress = filePathAddressOptions.Value;
            _fileService = fileService;
            _customerService = customerService;
        }


        public async Task<IActionResult> All(int[]? categories, SortBy? sortBy, string? term, string? p, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(term))
            {
                term = HttpUtility.HtmlEncode(term);
            }

            if (!categories.Any() & !sortBy.HasValue & string.IsNullOrEmpty(term))
            {
                sortBy = SortBy.Newest;
            }

            var model = new ProductsFilterModel
            {
                Page = 1,
                Categories = categories,
                PageSize = 16,
                SortBy = sortBy,
                Term = term,
            };
            ViewBag.Filter = model;
            var productCategoriesResult = await _productManagement.GetAllCategoriesAsync(cancellationToken);
            if (productCategoriesResult.IsSuccess)
                ViewBag.ProductCategories = productCategoriesResult.Data;
            CommandResult<List<ProductForShowInSiteModel>> productsResult = await _productManagement.GetProductsByFilterAsync(model);
            return View(productsResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> GetProducts(ProductsFilterModel model)
        {
            if (model is null)
                return Json(new List<ProductForShowInSiteModel>());
            if (model.Page <= 0)
                model.Page = 1;
            var productsResult = await _productManagement.GetProductsByFilterAsync(model);

            return Json(productsResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductInfo(long productId, CancellationToken cancellationToken)
        {
            ProductInfoForShowInSiteModel model = new();
            //if (model is null)
            //    return Json(new List<ProductForShowInSiteModel>());
            //if (model.Page <= 0)
            //    model.Page = 1;
            int userId = User.GetUserIdAsInt();
            CommandResult<ProductInfoForShowInSiteModel> productInfoResult = await _productManagement.GetProductInfoAsync(productId, cancellationToken);
            if (productInfoResult.IsSuccess)
            {
                CommandResult<bool> isFavoriteResult = await _customerService.IsProductInFavoritsAsync(productInfoResult.Data.Id, userId);
                if (isFavoriteResult.IsSuccess)
                    productInfoResult.Data.IsBookmarked = isFavoriteResult.Data;
                await _productManagement.IsVisitedAsync(productId, cancellationToken);
            }
            return Json(productInfoResult.Data);
        }



        [HttpPost]
        public async Task<IActionResult> ProductInstallmentsCalculation(InstallmentPurchaseInputDataViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<InstallmentPurchaseModel> toastrResult = new();
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<InstallmentPurchaseModel>(ToastrType.Error, false, UserMessages.FormNotValid, new());
            }
            else
            {
                CommandResult<ProductInfoForShowInSiteModel> productInfoResult = await _productManagement.GetProductInfoAsync(model.ProductId, cancellationToken);
                if (productInfoResult.IsSuccess)
                {
                    var data = new InstallmentPurchaseInputDataModel
                    {
                        ProductId = model.ProductId,
                        InstallmentCount = model.InstallmentCount.Value,
                        PrePayment = long.Parse(model.PrePayment.Replace(",", ""))
                    };
                    if (data.PrePayment < productInfoResult.Data.DefaultPrePayment)
                    {
                        toastrResult = ToastrResult<InstallmentPurchaseModel>
                            .Error(Captions.Error, string.Format(ValidationMessages.MoreThan, Captions.Prepayment, productInfoResult.Data.DefaultPrePayment.ToString("N0")), new());
                    }
                    else
                    {
                        var result = await _productManagement.InstallmentPurchaseAsync(data, cancellationToken);
                        if (result.IsSuccess)
                            toastrResult = ToastrResult<InstallmentPurchaseModel>.Success(Captions.Success, result.Message, result.Data);
                        else
                            toastrResult = ToastrResult<InstallmentPurchaseModel>.Error(Captions.Error, result.Message, result.Data);
                    }
                }
                else
                {
                    toastrResult = ToastrResult<InstallmentPurchaseModel>.Error(Captions.Error, productInfoResult.Message, new());
                }

            }
            return Json(toastrResult);
        }


        [HttpGet]
        [Route("/Product/Image/{fileName}")]
        public async Task<IActionResult> GetProductImage(string? fileName)
        {
            string path = string.Empty;
            byte[]? fileBytes = null;
            string? fileContentType = string.Empty;
            try
            {
                if (_fileService.IsExist(fileName, _filePathAddress.ProductGalleryThumbnail))
                    path = $"{_filePathAddress.ProductGalleryThumbnail}/{fileName}";
                else
                    path = _filePathAddress.ProductDefaultImage;

                fileBytes = await _fileService.GetFileBytesAsync(path);
                fileContentType = _fileService.GetMimeTypeForFileExtension(path);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/Product/SellerImage/{fileName}")]
        public async Task<IActionResult> GetSellerImage(string? fileName)
        {
            string path = string.Empty;
            byte[]? fileBytes = null;
            string? fileContentType = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    path = _filePathAddress.ProductDefaultImage;
                else
                    path = $"{_filePathAddress.SellerProfileImage}/{fileName}";

                fileBytes = await _fileService.GetFileBytesAsync(path);
                fileContentType = _fileService.GetMimeTypeForFileExtension(path);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/Product/MediaGallery/{fileName}")]
        public async Task<IActionResult> GetMediaGallery(string? fileName)
        {
            string path = string.Empty;
            byte[]? fileBytes = null;
            string? fileContentType = string.Empty;
            try
            {
                if (_fileService.IsExist(fileName, _filePathAddress.ProductGallery))
                    path = $"{_filePathAddress.ProductGallery}/{fileName}";
                else
                    path = _filePathAddress.ProductDefaultImage;

                //if (string.IsNullOrEmpty(fileName))
                //    path = _filePathAddress.ProductDefaultImage;
                //else
                //    path = $"{_filePathAddress.ProductGalleryThumbnail}/{fileName}";

                fileBytes = await _fileService.GetFileBytesAsync(path);
                fileContentType = _fileService.GetMimeTypeForFileExtension(path);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

    }
}
