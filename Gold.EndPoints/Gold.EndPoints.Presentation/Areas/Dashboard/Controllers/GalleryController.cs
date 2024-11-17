using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.GalleryModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GalleryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
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
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;
using System.Threading;
using Gold.SharedKernel.Contract;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Galleries)]
    public class GalleryController : Controller
    {
        private readonly IGalleryService _galleryService;
        private readonly IViewRenderService _renderService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public GalleryController(IGalleryService galleryService, IViewRenderService renderService, IUserService userService, IFileService fileService)
        {
            this._galleryService = galleryService;
            this._renderService = renderService;
            _userService = userService;
            _fileService = fileService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> CreateOrEditGallery(int id, CancellationToken cancellationToken)
        {
            ToastrResult<CreateOrEditGalleryViewModel> toastResult = new();
            CreateOrEditGalleryViewModel model = new();
            string html = string.Empty;

            if (id == 0)
            {
                //add
                model = new() { Id = 0, IsActive = true };
                //html = await _renderService.RenderViewToStringAsync("_CreateOrEditGallery", model, this.ControllerContext);
                toastResult = ToastrResult<CreateOrEditGalleryViewModel>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            }
            else
            {
                //edit
                CommandResult<CreateOrEditGalleryViewModel> result = await _galleryService.GetGalleryInfoForEditAsync(id, cancellationToken);
                if (result.IsSuccess)
                    toastResult = ToastrResult<CreateOrEditGalleryViewModel>.Success(Captions.Success, result.Message, result.Data);
                else
                    toastResult = ToastrResult<CreateOrEditGalleryViewModel>.Error(Captions.Error, result.Message, model);
            }
            return Json(toastResult);
        }


        [HttpPost]
        public async Task<JsonResult> CreateOrEditGallery(CreateOrEditGalleryViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastResult = new();
            if (!ModelState.IsValid)
            {
                toastResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
                return Json(toastResult);
            }

            if (model.Id == 0)
            {
                //add
                CommandResult addResult = await _galleryService.CreateGalleryAsync(model, cancellationToken);
                if (addResult.IsSuccess)
                {
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.Name);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Insert,
                        AdminMenuId = (byte)ManagerPermission.Galleries,
                        DescriptionPattern = AdminActivityLogDescriptions.Gallery_Insert,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);

                    toastResult = ToastrResult.Success(Captions.Success, addResult.Message);
                }
                else
                {
                    toastResult = ToastrResult.Error(Captions.Warning, addResult.Message);
                }
            }
            else
            {
                //edit
                CommandResult editResult = await _galleryService.EditGalleryAsync(model, cancellationToken);
                if (editResult.IsSuccess)
                {
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.Name);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Edit,
                        AdminMenuId = (byte)ManagerPermission.Galleries,
                        DescriptionPattern = AdminActivityLogDescriptions.Gallery_Edit,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    toastResult = ToastrResult.Success(Captions.Success, editResult.Message);
                }
                else
                {
                    toastResult = ToastrResult.Error(Captions.Warning, editResult.Message);
                }
            }
            return Json(toastResult);
        }

        [HttpGet]
        public async Task<JsonResult> RemoveGallery(int id, CancellationToken cancellationToken)
        {

            ToastrResult toastrResult = new ToastrResult<string>();
            CommandResult<string> result = await _galleryService.RemoveGalleryAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.Galleries,
                    DescriptionPattern = AdminActivityLogDescriptions.Gallery_Delete,
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
        public IActionResult GalleryList_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = _galleryService.GetGalleryListAsQuerable();
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
            return Json(true);
        }

        public IActionResult ExportPdf()
        {
            List<GalleryReportModel> reportData = new List<GalleryReportModel>();
            CommandResult<IQueryable<GalleryReportModel>> result = _galleryService.GetGalleryReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.Name).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"GalleryReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            //string model = string.Empty;

            // if (TempData.Peek("InitCustomersReportData") != null)
            //    model = TempData.Get<PaymentReportSearchViewModel>("InitCustomersReportData");

            List<GalleryReportModel> reportData = new List<GalleryReportModel>();
            CommandResult<IQueryable<GalleryReportModel>> result = _galleryService.GetGalleryReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.Name).ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Name);
            reportList.Columns.Add(Captions.Tel);
            reportList.Columns.Add(Captions.Address);
            reportList.Columns.Add(Captions.PurchaseDescription);
            reportList.Columns.Add(Captions.HasInstallmentSale);
            reportList.Columns.Add(Captions.Status);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            ColumnWidth[4] = 50;
            ColumnWidth[5] = 15;
            ColumnWidth[6] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.Name,
                   item.Tel,
                   item.Address,
                   item.PurchaseDescription.RemoveHtmlTags(" "),
                   (item.HasInstallmentSale ? Captions.Active : Captions.DeActive),
                   (item.IsActive ? Captions.Active : Captions.DeActive)
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.Galleries}", true);
            string filename = string.Format("{0}.xlsx", $"GalleryReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion
    }
}
