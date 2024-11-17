using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.ReportModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ReportViewModels;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;
using Gold.SharedKernel.Contract;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ManagersActivityReport)]
    public class ManagerReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        public ManagerReportController(IReportService reportService, IUserService userService, IFileService fileService)
        {
            _reportService = reportService;
            _userService = userService;
            _fileService = fileService;
        }
        #region گزارش عملکرد مدیران
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var adminMenuList = _userService.GetAdminMenusAsSelectListByUserTypeAsync(0, cancellationToken).Result.Data;


           var dminMenuListResult = _reportService.GetManagersOperationReportAdminMenus();
      
            ViewBag.AdminMenuList = dminMenuListResult.Data.Select(x=>new SelectListItem
            {
                Text=x.Title,
                Value=x.Id.ToString(),
            }).ToList();
            List<SelectListItem> managerUsers = _userService.GetUsersFullNameAsSelectListByUserTypeAsync(0, UserType.Manager, cancellationToken).Result.Data;
            List<SelectListItem> sellerUsers = _userService.GetUsersFullNameAsSelectListByUserTypeAsync(0, UserType.Seller, cancellationToken).Result.Data;
            List<SelectListItem> adminUsers = _userService.GetUsersFullNameAsSelectListByUserTypeAsync(0, UserType.Admin, cancellationToken).Result.Data;
           
            var users= new List<SelectListItem>();
            users.AddRange(managerUsers);
            users.AddRange(sellerUsers);
            users.AddRange(adminUsers);

            ViewBag.ManagerUsers = users;
            ViewBag.ActivityTypes = EnumHelper<AdminActivityType>.GetAsList().Select(x => new SelectListItem { Text = x.GetDisplayName(), Value = x.ToString() });
            return View();
        }
        public IActionResult ManagerOperation_Read([DataSourceRequest] DataSourceRequest request, ManagersOperationReportViewModel model)
        {
            CommandResult<IQueryable<ManagersOperationReportModel>> result = _reportService.GetManagersOperationReport(model);

            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
            {
                dataSource = result.Data.ToDataSourceResult(request);
            }
            return Json(dataSource);
        }
        #endregion


        #region export data


        public JsonResult InitReportData(ManagersOperationReportViewModel model)
        {

            TempData.Set<ManagersOperationReportViewModel>("InitManagersOperationReportData", model);
            return Json(true);
        }

        public IActionResult ExportPdf()
        {
            ManagersOperationReportViewModel model = new ManagersOperationReportViewModel();

            List<ManagersOperationReportModel> reportData = new List<ManagersOperationReportModel>();
            if (TempData.Peek("InitManagersOperationReportData") != null)
                model = TempData.Get<ManagersOperationReportViewModel>("InitManagersOperationReportData");

            CommandResult<IQueryable<ManagersOperationReportModel>> result = _reportService.GetManagersOperationReport(model);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"ManagersOperationReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            var model = new ManagersOperationReportViewModel();

            List<ManagersOperationReportModel> reportData = new List<ManagersOperationReportModel>();
            if (TempData.Peek("InitManagersOperationReportData") != null)
                model = TempData.Get<ManagersOperationReportViewModel>("InitManagersOperationReportData");

            CommandResult<IQueryable<ManagersOperationReportModel>> result = _reportService.GetManagersOperationReport(model);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Page);
            reportList.Columns.Add(Captions.Operation);
            reportList.Columns.Add(Captions.User);
            reportList.Columns.Add(Captions.Date);
            reportList.Columns.Add(Captions.Description);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            ColumnWidth[4] = 25;
            ColumnWidth[5] = 100;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.Page,
                   item.Operation,
                   item.UserName,
                   item.PersianDate,
                   item.Description);
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, Captions.ManagerOperationReport, true);
            string filename = string.Format("{0}.xlsx", $"ManagersOperationReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion
    }
}
