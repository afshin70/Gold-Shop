using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AccessLevelModels;
using Gold.ApplicationService.Contract.DTOs.Models.ManagerModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ManagerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SellerViewModels;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Data;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Managers)]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly IUserService _userService;
        private readonly IViewRenderService _renderService;
        private readonly IFileService _fileService;

        public ManagerController(IManagerService managerService, IUserService userService, IViewRenderService renderService, IFileService fileService)
        {
            _managerService = managerService;
            _userService = userService;
            _renderService = renderService;
            _fileService = fileService;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var result = await _userService.GetAccessLevelAsSelectListItemAsync(0, cancellationToken);
            ViewBag.AccessLevels = result.Data;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateOrEditManagerUser(int id, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastResult = new();
            string html = string.Empty;

            if (id <= 0)
            {
                //new
                var model = new CreateOrEditManagerUserViewModel
                {
                    ManagerUserId = 0,
                    IsActive = true,
                    AccessLevels = _userService.GetAccessLevelAsSelectListItemAsync(0, cancellationToken).Result.Data
                };
                html = await _renderService.RenderViewToStringAsync("_CreateOrEditManager", model, this.ControllerContext);
                toastResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
            {
                //edit
                CommandResult<CreateOrEditManagerUserViewModel> result = await _managerService.GetManagerUserForEditAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    int accessLevelId = result.Data.AccessLevelId.HasValue ? result.Data.AccessLevelId.Value : 0;
                    result.Data.AccessLevels = _userService.GetAccessLevelAsSelectListItemAsync(accessLevelId, cancellationToken).Result.Data;
                    html = await _renderService.RenderViewToStringAsync("_CreateOrEditManager", result.Data, this.ControllerContext);
                    toastResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
                }
                else
                {
                    toastResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
                }
            }

            return Json(toastResult);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrEditManagerUser(CreateOrEditManagerUserViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastResult = new();
            if (model.ManagerUserId <= 0)
            {
                if (string.IsNullOrEmpty(model.Password))
                    ModelState.AddModelError(nameof(model.Password), string.Format(ValidationMessages.Required, Captions.Password));

                if (string.IsNullOrEmpty(model.ConfirmPassword))
                    ModelState.AddModelError(nameof(model.ConfirmPassword), string.Format(ValidationMessages.Required, Captions.ConfirmPassword));

                if (!model.AccessLevelId.HasValue)
                    ModelState.AddModelError(nameof(model.AccessLevelId), string.Format(ValidationMessages.Required, Captions.AccessLevel));
            }

            if (!ModelState.IsValid)
            {
                toastResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            }
            else
            {
                CommandResult<CreateOrEditManagerUserViewModel> result = await _managerService.AddOrEditManagerUserAsync(model, cancellationToken);
                if (result.IsSuccess)
                {
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.FullName);
                    var logModel = new LogUserActivityModel()
                    {

                        AdminMenuId = (byte)ManagerPermission.Managers,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    if (model.ManagerUserId <= 0)
                    {
                        logModel.ActivityType = AdminActivityType.Insert;
                        logModel.DescriptionPattern = AdminActivityLogDescriptions.Manager_Insert;
                    }
                    else
                    {
                        logModel.ActivityType = AdminActivityType.Edit;
                        logModel.DescriptionPattern = AdminActivityLogDescriptions.Manager_Edit;
                    }
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);

                    toastResult = ToastrResult.Success(Captions.Success, result.Message);
                }
                else
                    toastResult = ToastrResult.Error(Captions.Error, result.Message);
            }

            return Json(toastResult);
        }

        public IActionResult ManagerList_Read([DataSourceRequest] DataSourceRequest request)
        {
            CommandResult<IQueryable<ManagerUserModel>> result = _managerService.GetManagerListAsQuerable();
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
                dataSource = result.Data.ToDataSourceResult(request);
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

            List<ManagerUserReportModel> reportData = new List<ManagerUserReportModel>();
            CommandResult<IQueryable<ManagerUserReportModel>> result = _managerService.GetManagerReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.FullName).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"ManagersReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            //string model = string.Empty;

            // if (TempData.Peek("InitCustomersReportData") != null)
            //    model = TempData.Get<PaymentReportSearchViewModel>("InitCustomersReportData");

            List<ManagerUserReportModel> reportData = new List<ManagerUserReportModel>();
            CommandResult<IQueryable<ManagerUserReportModel>> result = _managerService.GetManagerReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.FullName).ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
           reportList.Columns.Add(Captions.FullName);
           reportList.Columns.Add(Captions.Mobile);
           reportList.Columns.Add(Captions.UserName);
           reportList.Columns.Add(Captions.AccessLevel);
           reportList.Columns.Add(Captions.Status);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            ColumnWidth[4] = 25;
            ColumnWidth[5] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.FullName,
                   item.Mobile,
                   item.UserName,
                   item.AccessLevelTitle,
                   (item.IsActive ? Captions.Active : Captions.DeActive)
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.ManagerUsers}", true);
            string filename = string.Format("{0}.xlsx", $"ManagersReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion



        [HttpGet]
        public async Task<IActionResult> RemoveManager(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            CommandResult<string> result = await _managerService.RemoveManagerAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.Managers,
                    DescriptionPattern = AdminActivityLogDescriptions.Manager_Delete,
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
    }
}
