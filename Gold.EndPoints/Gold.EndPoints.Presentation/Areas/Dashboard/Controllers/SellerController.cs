using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.GalleryModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GalleryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SellerViewModels;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;
using System.Threading;
using Gold.ApplicationService.Contract.DTOs.Models.SellerModels;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Microsoft.Extensions.Options;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Galleries)]
    public class SellerController : Controller
    {
        private readonly ISellerService _sellerService;
        private readonly IViewRenderService _renderService;
        private readonly IGalleryService _galleryService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        private readonly FilePathAddress _filePathAddress;

        public SellerController(ISellerService sellerService, IOptions<FilePathAddress> filePathAddressOptions, IViewRenderService renderService, IGalleryService galleryService, IUserService userService, IFileService fileService)
        {
            this._sellerService = sellerService;
            this._renderService = renderService;
            this._galleryService = galleryService;
            _userService = userService;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
        }

        [HttpGet]
        public async Task<JsonResult> CreateOrEditSeller(int id, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastResult = new();
            CreateOrEditSellerViewModel model = new()
            {
                SellerId = 0,
                IsActive = true,

            };
            string html = string.Empty;
            var activeGalleriesSelectListResult = await _galleryService.GetActiveGalleriesAsync(0, cancellationToken);

            if (id == 0)
            {
                //new
                model.Galleries = activeGalleriesSelectListResult.Data;
                html = await _renderService.RenderViewToStringAsync("_CreateOrEditSeller", model, this.ControllerContext);
                toastResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
            {
                //edit
                CommandResult<CreateOrEditSellerViewModel> result = await _sellerService.GetSellerInfoForEditAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    model = result.Data;
                    var allGalleriesSelectListResult = await _galleryService.GetAllGalleriesAsync(result.Data.GalleryId.Value, cancellationToken);
                    model.Galleries = allGalleriesSelectListResult.Data;
                    html = await _renderService.RenderViewToStringAsync("_CreateOrEditSeller", model, this.ControllerContext);
                    toastResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
                }
                else
                {
                    model.Galleries = activeGalleriesSelectListResult.Data;
                    html = await _renderService.RenderViewToStringAsync("_CreateOrEditSeller", model, this.ControllerContext);
                    toastResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
                }
            }

            return Json(toastResult);
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrEditSeller(CreateOrEditSellerViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            int galleryId = model.GalleryId.HasValue ? model.GalleryId.Value : 0;
            var activeGalleriesSelectListResult = await _galleryService.GetActiveGalleriesAsync(galleryId, cancellationToken);
            if (model.SellerId == 0)
            {
                if (string.IsNullOrEmpty(model.Password))
                    ModelState.AddModelError(nameof(model.Password), string.Format(ValidationMessages.Required, Captions.Password));
                if (string.IsNullOrEmpty(model.ConfirmPassword))
                    ModelState.AddModelError(nameof(model.ConfirmPassword), string.Format(ValidationMessages.Required, Captions.ConfirmPassword));
                model.Galleries = activeGalleriesSelectListResult.Data;
            }
            else
            {
                var galleriesSelectListResult = await _galleryService.GetAllGalleriesAsync(model.GalleryId.Value, cancellationToken);
                model.Galleries = galleriesSelectListResult.Data;
            }

            if (!ModelState.IsValid)
            {
               html = await _renderService.RenderViewToStringAsync("_CreateOrEditSeller", model, this.ControllerContext, false);
                string errorMessage = string.Empty;
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html) ;// ToastrResult<string>.Warning(Captions.Warning, UserMessages.FormNotValid+ errorMessage, html);
            }
            else
            {
                        CommandResult<string> galleryNameResult = await _galleryService.GetGalleryNameAsync(model.GalleryId,cancellationToken);

                if (model.SellerId == 0)
                {
                    //add new
                    var result = await _sellerService.CreateSellerAsync(model, cancellationToken);
                    if (result.IsSuccess)
                    {
                        
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.FullName);
                        logParams.Add("1", galleryNameResult.Data);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.Galleries,
                            DescriptionPattern = AdminActivityLogDescriptions.Gallery_Insert_Seller,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);

                        toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
                    }
                    else
                    {
                       toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
                    }
                }
                else
                {

                    //update
                    var updateResult = await _sellerService.EditSellerInfoAsync(model, cancellationToken);
                    if (updateResult.IsSuccess)
                    {
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.FullName);
                        logParams.Add("1", galleryNameResult.Data);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            AdminMenuId = (byte)ManagerPermission.Galleries,
                            DescriptionPattern = AdminActivityLogDescriptions.Gallery_Edit_Seller,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);

                        toastrResult = ToastrResult<string>.Success(Captions.Success, updateResult.Message, html);
                    }
                    else
                    {
                        toastrResult = ToastrResult<string>.Error(Captions.Error, updateResult.Message, html);
                    }
                }
            }

            return Json(toastrResult);
        }


        [HttpGet]
        public async Task<IActionResult> RemoveSeller(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            CommandResult<string> result = await _sellerService.RemoveSellerAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                CommandResult<string> galleryNameResult=await _sellerService.GetSellerGalleryNameAsync(id, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                logParams.Add("1", galleryNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.Galleries,
                    DescriptionPattern = AdminActivityLogDescriptions.Gallery_Delete_Seller,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }
            return Json(toastrResult);
        }


        public IActionResult SellerList_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = _sellerService.GetSellerListAsQuerable();
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
            {
                dataSource = result.Data.ToDataSourceResult(request);
            }
            return Json(dataSource);
        }

        #region export data


        public JsonResult InitReportData()
        {

            //TempData.Set<string>("InitCustomersReportData", string.Empty);
            return Json(true);
        }

        public IActionResult ExportPdf()
        {
            //string model = string.Empty;

            //if (TempData.Peek("InitCustomersReportData") != null)
            //    model = TempData.Get<string>("InitCustomersReportData");

            List<SellerReportModel> reportData = new List<SellerReportModel>();
            CommandResult<IQueryable<SellerReportModel>> result = _sellerService.GetSellerReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.FullName).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"SellerReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            //string model = string.Empty;

            // if (TempData.Peek("InitCustomersReportData") != null)
            //    model = TempData.Get<PaymentReportSearchViewModel>("InitCustomersReportData");

            List<SellerReportModel> reportData = new List<SellerReportModel>();
            CommandResult<IQueryable<SellerReportModel>> result = _sellerService.GetSellerReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.FullName).ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Name);
            reportList.Columns.Add(Captions.UserName);
            reportList.Columns.Add(Captions.Mobile);
            reportList.Columns.Add(Captions.Gallery);
            reportList.Columns.Add(Captions.ProductRegisterPerMinCount);
            reportList.Columns.Add(Captions.RegisterLoan);
            reportList.Columns.Add(Captions.RegisterProduct);
            reportList.Columns.Add(Captions.Status);

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
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.FullName,
                   item.UserName,
                   item.Mobile,
                   item.GalleryName,
                   item.ProductRegisterPerMinCount,
                   (item.HasAccessToRegisterLoan ? Captions.Active : Captions.DeActive),
                   (item.HasAccessToRegisterProduct ? Captions.Active : Captions.DeActive),
                   (item.IsActive ? Captions.Active : Captions.DeActive)
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.Sellers}", true);
            string filename = string.Format("{0}.xlsx", $"SellerReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion


        [HttpGet]
        [Route("/Seller/SellerImage/{fileName}")]
        public async Task<IActionResult> GetSellerImage(string? fileName)
        {
            string path = string.Empty;
            byte[]? fileBytes = null;
            string? fileContentType = string.Empty;
            try
            {
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
    }
}
