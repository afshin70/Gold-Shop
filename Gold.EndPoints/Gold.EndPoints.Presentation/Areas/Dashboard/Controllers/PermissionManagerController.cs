using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AccessLevelModels;
using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.ManagerModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AccessLevelViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.Domain.Entities;
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
using Microsoft.CodeAnalysis;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;
using System.Security.Policy;
using System.Threading;
using Gold.SharedKernel.Contract;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.AccessLevels)]
    public class PermissionManagerController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public PermissionManagerController(IUserService userService, IViewRenderService renderService, IFileService fileService)
        {
            _userService = userService;
            _renderService = renderService;
            _fileService = fileService;
        }
        public IActionResult AccessLevel()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEditAccessLevel(int id, CancellationToken cancellationToken)
        {
            CommandResult<CreateOrEditAccessLevelViewModel> result = await _userService.GetAccessLevelForEditAsync(id, cancellationToken);
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            if (result.IsSuccess)
            {
                //edit
                html = await _renderService.RenderViewToStringAsync("_CreateOrEditAccessLevel", result.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                //add
                html = await _renderService.RenderViewToStringAsync("_AddOrEditAccessLevel", result.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrEditAccessLevel(CreateOrEditAccessLevelViewModel model, CancellationToken cancellationToken)
        {

            ToastrResult toastrResult = new();
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            }
            else
            {
                CommandResult<CreateOrEditAccessLevelViewModel> result = await _userService.CreateOrEditAccessLevelAsync(model, cancellationToken);
                if (result.IsSuccess)
                {
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.Title);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType =model.AccessLevelId<=0? AdminActivityType.Insert: AdminActivityType.Edit ,
                        AdminMenuId = (byte)ManagerPermission.AccessLevels,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt(),
                        DescriptionPattern= model.AccessLevelId <= 0 ? AdminActivityLogDescriptions.AccessLevel_Insert: AdminActivityLogDescriptions.AccessLevel_Edit
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    toastrResult = ToastrResult.Success(Captions.Success, result.Message);
                }
                else
                    toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveAccessLevel(byte id,CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            CommandResult<string> result = await _userService.RemoveAccessLevelAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.AccessLevels,
                    DescriptionPattern = AdminActivityLogDescriptions.AccessLevel_Delete,
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
        public IActionResult AccessLevelList_Read([DataSourceRequest] DataSourceRequest request)
        {
            CommandResult<IQueryable<AccessLevelModel>> result = _userService.GetAccessLevelListAsQuerable();
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

            List<AccessLevelReportModel> reportData = new List<AccessLevelReportModel>();
            CommandResult<IQueryable<AccessLevelReportModel>> result = _userService.GetAccessLevelReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.Title).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"PermissionReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            //string model = string.Empty;

            // if (TempData.Peek("InitCustomersReportData") != null)
            //    model = TempData.Get<PaymentReportSearchViewModel>("InitCustomersReportData");

            List<AccessLevelReportModel> reportData = new List<AccessLevelReportModel>();
            CommandResult<IQueryable<AccessLevelReportModel>> result = _userService.GetAccessLevelReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.Title).ToList();

            DataTable reportList = new DataTable("ExportExcel");
           reportList.Columns.Add("#");
           reportList.Columns.Add(Captions.Title);
            reportList.Columns.Add(Captions.AccessLevels);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 80;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.Title,
                   item.PermissionsTitle.Aggregate((i, j) => i + " - " + j)
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.ManagerUsers}", true);
            string filename = string.Format("{0}.xlsx", $"PermissionReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion

    }
}
