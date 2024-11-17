using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels.CategoryModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.CategoryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.ProdctGalleryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)},{nameof(UserType.Seller)}", ManagerPermission.ProductManagement)]
    public class ProductController : Controller
    {
        private readonly IProductManagementService _productManagement;
        private readonly IUserService _userService;
        private readonly ISellerService _sellerService;
        private readonly ISettingService _settingService;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;
        private readonly IViewRenderService _renderService;
        public ProductController(IProductManagementService productManagement, IUserService userService,
            ISellerService sellerService, ISettingService settingService, IFileService fileService, IOptions<FilePathAddress> filePathAddressOptions, IViewRenderService renderService)
        {
            _productManagement = productManagement;
            _userService = userService;
            _sellerService = sellerService;
            _settingService = settingService;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
            _renderService = renderService;
        }
        private  int? _sellerUserId=> HttpContext.User.GetUserType() == UserType.Seller? HttpContext.User.GetUserIdAsInt():null;

        private async Task<bool> IsSellerUserHasAccessToProductRegistration(CancellationToken cancellationToken = default)
        {
            if (User.GetUserType() == UserType.Seller)
            {
                var userId = User.GetUserIdAsInt();
                var result = await _sellerService.HasAllowToRegisterProductAsync(userId, cancellationToken);
                return result.IsSuccess;
            }
            else
            {
                return true;
            }

        }
        public async Task<IActionResult> Index()
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Forbid();
          
            return View();
        }
        #region Category
        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public async Task<IActionResult> CreateOrEditCategory(int? id, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();

            ToastrResult<CreateOrEditCategoryViewModel> result = new();
            CreateOrEditCategoryViewModel model = new();
            if (id.HasValue)
            {
                CommandResult<CategoryModel> commandResult = await _productManagement.GetCategoryByIdAsync(id.Value, cancellationToken);

                if (commandResult.IsSuccess)
                {
                    model.Id = commandResult.Data.Id;
                    model.Title = commandResult.Data.Title;
                }
                else
                    model = new();
            }
            else
                model = new();
            result = ToastrResult<CreateOrEditCategoryViewModel>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            return Json(result);
        }

        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public async Task<IActionResult> CreateOrEditCategory(CreateOrEditCategoryViewModel model, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();

            ToastrResult result = new();

            if (ModelState.IsValid)
            {
                CommandResult<CreateOrEditCategoryViewModel> commandResult = await _productManagement.CreateOrEditCategoryAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                {
                    if (model.Id == null)
                    {
                        //add log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", commandResult.Data.Title);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.ProductManagement,
                            DescriptionPattern = AdminActivityLogDescriptions.ProductCategory_Insert,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    }
                    else
                    {
                        //edit log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", commandResult.Data.Title);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            AdminMenuId = (byte)ManagerPermission.ProductManagement,
                            DescriptionPattern = AdminActivityLogDescriptions.ProductCategory_Edit,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    }
                    result = ToastrResult.Success(Captions.Success, commandResult.Message);
                }
                else
                    result = ToastrResult.Error(Captions.Error, commandResult.Message);
            }
            else
                result = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            return Json(result);
        }

        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public async Task<IActionResult> RemoveCategory(int id, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            ToastrResult toastrResult = new ToastrResult();
            CommandResult<string> result = await _productManagement.RemoveCategoryAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.ProductManagement,
                    DescriptionPattern = AdminActivityLogDescriptions.ProductCategory_Delete,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);

                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public async Task<IActionResult> CategoryList_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            CommandResult<IQueryable<CategoryModel>> result = _productManagement.GetCategoryAsQuerable();
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
                dataSource = result.Data.ToDataSourceResult(request);
            return Json(dataSource);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public async Task<JsonResult> ChangeCategoryOrder(int id, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            var result = await _productManagement.ChangeCategoryOrderNumberAsync(id, isUp, cancellationToken);
            if (result.IsSuccess)
                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            return Json(toastrResult);
        }

        #region export products data

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public JsonResult InitCategoryReportData()
        {

            //TempData.Set<string>("InitCustomersReportData", string.Empty);
            return Json(true);
        }
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public IActionResult CategoryExportPdf()
        {
            List<CategoryModel> reportData = new List<CategoryModel>();
            CommandResult<IQueryable<CategoryModel>> result = _productManagement.GetCategoryAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            return new ViewAsPdf("ExportPdfCategory", reportData, null)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"SellerReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ProductManagement)]
        public IActionResult CategoryExportExcel()
        {
            List<CategoryModel> reportData = new List<CategoryModel>();
            CommandResult<IQueryable<CategoryModel>> result = _productManagement.GetCategoryAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Title);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.Title
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.Categories}", true);
            string filename = string.Format("{0}.xlsx", $"CategoriesReport " + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion
        #endregion

        #region Product

        [HttpGet]
        public async Task<IActionResult> CreateOrEditProduct(int? id, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            ToastrResult<CreateOrEditProductViewModel> result = new();
            CreateOrEditProductViewModel model = new();
            if (id.HasValue)
            {
                CommandResult<ProductModel> commandResult = await _productManagement.GetProductByIdAsync(id.Value,_sellerUserId, cancellationToken);

                if (commandResult.IsSuccess)
                {
                    model = new()
                    {
                        ProductId = commandResult.Data.Id,
                        CategoryIds = commandResult.Data.CategoryIds,
                        Description = commandResult.Data.Description,
                        GalleryId = commandResult.Data.GalleryId,
                        GalleryProfit = commandResult.Data.GalleryProfit.ToString(),
                        Karat = commandResult.Data.Karat,
                        Status = commandResult.Data.Status,
                        StonPrice = commandResult.Data.StonPrice.ToString("N0"),
                        ProductTitle = commandResult.Data.Title,
                        Wage = commandResult.Data.Wage,
                        Weight = commandResult.Data.Weight.ToString(),
                    };
                }
                else
                {
                    model = new();
                    model.Karat = 18;
                    var loanSettings = await _settingService.GetSettingAsync<LoanSettingsViewModel>(Domain.Enums.SettingType.LoanSetting, cancellationToken);
                    if (loanSettings.IsSuccess)
                    {
                        model.GalleryProfit = string.IsNullOrEmpty(loanSettings.Data.MaxProfitGallery) ? "0" : loanSettings.Data.MaxProfitGallery;
                        // model.Tax = loanSettings.Data.Tax.HasValue ? loanSettings.Data.Tax.Value.ToString() : "0";
                    }
                    result = ToastrResult<CreateOrEditProductViewModel>.Error(Captions.Error, commandResult.Message, model);
                    return Json(result);
                }
            }
            else
                model = new();
            result = ToastrResult<CreateOrEditProductViewModel>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEditProduct(CreateOrEditProductViewModel model, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            ToastrResult result = new();
            if (User.GetUserType()==UserType.Seller)
                ModelState.Remove(nameof(model.GalleryId));

            if (ModelState.IsValid)
            {
                var userId = User.GetUserIdAsInt();
                model.RegistrarUserId = userId;
                model.UserType = User.GetUserType();
                if (User.GetUserType() == UserType.Seller)
                {
                    //چک کردن اجازه ثبت محصول توسط فروشنده
                    var isAllowToRegisterationProductResult = await _sellerService.IsAllowToRegisterProductAsync(userId, cancellationToken);
                    if (isAllowToRegisterationProductResult.IsSuccess)
                    {
                        var sellerGalleryResult = await _sellerService.GetSellerGalleryAsync(userId, cancellationToken);
                        if (sellerGalleryResult.IsSuccess)
                            model.GalleryId = sellerGalleryResult.Data.Id;
                        else
                        {
                            result = ToastrResult.Error(Captions.Error, UserMessages.NotSetGalleryForYou);
                            return Json(result);
                        }
                    }
                    else
                    {
                        result = ToastrResult.Error(Captions.Error, isAllowToRegisterationProductResult.Message);
                        return Json(result);
                    }

                }
                CommandResult<CreateOrEditProductViewModel> commandResult = await _productManagement.CreateOrEditProductAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                {
                    if (model.ProductId == null)
                    {
                        //add log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", commandResult.Data.ProductTitle);
                        logParams.Add("1", commandResult.Data.GalleryName);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.ProductManagement,
                            DescriptionPattern = AdminActivityLogDescriptions.Product_Insert,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    }
                    else
                    {
                        //edit log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", commandResult.Data.ProductTitle);
                        logParams.Add("1", commandResult.Data.GalleryName);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            AdminMenuId = (byte)ManagerPermission.ProductManagement,
                            DescriptionPattern = AdminActivityLogDescriptions.Product_Edit,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    }
                    result = ToastrResult.Success(Captions.Success, commandResult.Message);
                }
                else
                    result = ToastrResult.Error(Captions.Error, commandResult.Message);
            }
            else
                result = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> RemoveProduct(long id, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            ToastrResult toastrResult = new ToastrResult();
            CommandResult<ProductModel> result = await _productManagement.RemoveProductAsync(id,_sellerUserId, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data.Title);
                logParams.Add("1", result.Data.GalleryName);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.ProductManagement,
                    DescriptionPattern = AdminActivityLogDescriptions.Product_Delete,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);

                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            return Json(toastrResult);
        }    
        
        
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(long id, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            ToastrResult toastrResult = new ToastrResult();
            CommandResult<string> result = await _productManagement.ChangeProductStatusAsync(id,_sellerUserId, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    AdminMenuId = (byte)ManagerPermission.ProductManagement,
                    DescriptionPattern = AdminActivityLogDescriptions.Product_Edit,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);

                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            return Json(toastrResult);
        }

        public async Task<IActionResult> ProductList_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
           
            CommandResult<IQueryable<ProductModel>> result = _productManagement.GetProductAsQuerable(_sellerUserId);
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
                dataSource = result.Data.ToDataSourceResult(request);
            return Json(dataSource);
        }

        #region product gallery
        [HttpGet]
        public async Task<IActionResult> CreateProductGallery(long? productId, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CreateProductGalleryViewModel model = new();
            if (productId.HasValue)
            {

                var productInfoResult = await _productManagement.GetProductByIdAsync(productId.Value,_sellerUserId, cancellationToken);

                if (productInfoResult.IsSuccess)
                {
                    var galleryListResult = await _productManagement.GetAllProductGalleryByProductIdAsync(productInfoResult.Data.Id, cancellationToken);

                    model.OwnProductId = productInfoResult.Data.Id;
                    model.ProductName = productInfoResult.Data.Title;
                    model.ProductGalleries = galleryListResult.Data;
                    html = await _renderService.RenderViewToStringAsync("_CreateProductGallery", model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, productInfoResult.Message, html);
                }
                else
                    toastrResult = ToastrResult<string>.Error(Captions.Error, string.Format(UserMessages.NotFound, Captions.Product), html);
            }
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, string.Format(UserMessages.NotFound, Captions.Product), html);
            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductGallery(CreateProductGalleryViewModel model, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;

            if (ModelState.IsValid)
            {
                CommandResult<CreateProductGalleryViewModel> commandResult = await _productManagement.CreateProductGalleryAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                {
                    //add
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", commandResult.Data.ProductName);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Insert,
                        AdminMenuId = (byte)ManagerPermission.ProductManagement,
                        DescriptionPattern = AdminActivityLogDescriptions.ProductGallery_Insert,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);

                    var galleryListResult = await _productManagement.GetAllProductGalleryByProductIdAsync(model.OwnProductId, cancellationToken);
                    commandResult.Data.ProductGalleries = galleryListResult.Data;
                    commandResult.Data.OwnProductId = model.OwnProductId;

                    html = await _renderService.RenderViewToStringAsync("_CreateProductGallery", commandResult.Data, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, commandResult.Message, html);
                }
                else
                    toastrResult = ToastrResult<string>.Error(Captions.Error, commandResult.Message, html);
            }
            else
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveProductGallery(long id, long pid, CancellationToken cancellationToken)
        {
            if (!await IsSellerUserHasAccessToProductRegistration())
                return Unauthorized();
            string html = string.Empty;
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<string> result = await _productManagement.RemoveProductGalleryAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.ProductManagement,
                    DescriptionPattern = AdminActivityLogDescriptions.ProductGallery_Delete,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);

                var galleryListResult = await _productManagement.GetAllProductGalleryByProductIdAsync(pid, cancellationToken);
                html = await _renderService.RenderViewToStringAsync("_ProductGalleryList", galleryListResult.Data, this.ControllerContext);

                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            return Json(toastrResult);
        }

        public async Task<JsonResult> ChangeProductGalleryOrder(long id, long pid, int customerId, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var result = await _productManagement.ChangeGalleryOrderNumberAsync(id, isUp, cancellationToken);
            List<EssentialTelModel> model = new List<EssentialTelModel>();

            string html = string.Empty;

            if (result.IsSuccess)
            {
                var galleryListResult = await _productManagement.GetAllProductGalleryByProductIdAsync(pid, cancellationToken);
                html = await _renderService.RenderViewToStringAsync("_ProductGalleryList", galleryListResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        public async Task<JsonResult> SetProductGalleryThubmnail(long id, long pid, int customerId, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var result = await _productManagement.SetProductGalleryThubmnailAsync(id, cancellationToken);
            List<EssentialTelModel> model = new List<EssentialTelModel>();

            string html = string.Empty;

            if (result.IsSuccess)
            {
                var galleryListResult = await _productManagement.GetAllProductGalleryByProductIdAsync(pid, cancellationToken);
                html = await _renderService.RenderViewToStringAsync("_ProductGalleryList", galleryListResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        [Route("/Product/Gallery/{fileName}")]
        public async Task<IActionResult> GetProductGaleryFile(string? fileName)
        {
            try
            {
                string paymentImagesPath = $"{_filePathAddress.ProductGallery}/{fileName}";
                var fileBytes = await _fileService.GetFileBytesAsync(paymentImagesPath);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(paymentImagesPath);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        #endregion

        #region export products data


        public JsonResult InitProductReportData()
        {

            //TempData.Set<string>("InitCustomersReportData", string.Empty);
            return Json(true);
        }

        public IActionResult ProductExportPdf()
        {
            List<ProductModel> reportData = new List<ProductModel>();
            CommandResult<IQueryable<ProductModel>> result = _productManagement.GetProductAsQuerable(_sellerUserId);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            return new ViewAsPdf("ExportPdfProduct", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"SellerReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ProductExportExcel()
        {
            List<ProductModel> reportData = new List<ProductModel>();
            CommandResult<IQueryable<ProductModel>> result = _productManagement.GetProductAsQuerable(_sellerUserId);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Gallery);
            reportList.Columns.Add(Captions.Title);
            reportList.Columns.Add(Captions.Weight);
            reportList.Columns.Add(Captions.Karat);
            reportList.Columns.Add(Captions.Wage);
            reportList.Columns.Add(Captions.StoneValue);
            reportList.Columns.Add(Captions.GalleryProfit);
            reportList.Columns.Add(Captions.Status);
            reportList.Columns.Add(Captions.RegisterDate);
            reportList.Columns.Add(Captions.Categories);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            ColumnWidth[4] = 50;
            ColumnWidth[5] = 15;
            ColumnWidth[6] = 25;
            ColumnWidth[7] = 25;
            ColumnWidth[8] = 25;
            ColumnWidth[9] = 25;
            ColumnWidth[10] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.GalleryName,
                   item.Title,
                   item.Weight,
                   item.Karat,
                   item.Wage,
                   item.StonPrice.ToString("N0"),
                   item.GalleryProfit,
                   item.StatusTitle,
                   item.RegisterDatePersian,
                   string.Join(" - ", item.CategoryTitles)
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.Products}", true);
            string filename = string.Format("{0}.xlsx", $"ProductsReport " + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion
        #endregion
    }
}
