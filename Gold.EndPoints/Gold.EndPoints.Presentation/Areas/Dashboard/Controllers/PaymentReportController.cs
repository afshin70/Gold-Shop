using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.Convertor;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Collections.Generic;
using System.Data;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
	[Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.PaymentReport)]
    public class PaymentReportController : Controller
	{
		private readonly IReportService _reportService;
		private readonly IFileService _fileService;
		public PaymentReportController(IReportService reportService, IUserService userService, IFileService fileService)
		{
			_reportService = reportService;
			_fileService = fileService;
		}



		#region گزارش پرداخت ها
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult PaymentReportList_Read([DataSourceRequest] DataSourceRequest request, PaymentReportSearchViewModel model)
		{
			CommandResult<IQueryable<ConfirmedPaymentReport>> result = _reportService.GetPaymentsReport(model);

			DataSourceResult dataSource = new DataSourceResult();
			if (result.IsSuccess)
			{
				dataSource = result.Data.ToDataSourceResult(request);
			}
			return Json(dataSource);
		}
        public async Task<IActionResult> PaymentAmountSum(PaymentReportSearchViewModel model, CancellationToken cancellationToken)
        {
            long sum = 0;
            CommandResult<long> result = await _reportService.PaymentAmountSumAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                sum = result.Data;
            }
            return Json($"{sum.ToString("N0")} {Captions.Tooman}");
        }
        
        #endregion

        #region export data


        public JsonResult InitReportData(PaymentReportSearchViewModel model)
		{
            
                TempData.Set<PaymentReportSearchViewModel>("InitPaymentsReportData", model);
            return Json(true);
        }

        public IActionResult ExportPdf()
        {
            PaymentReportSearchViewModel model = new PaymentReportSearchViewModel();

            List<ConfirmedPaymentReport> reportData = new List<ConfirmedPaymentReport>();
            if (TempData.Peek("InitPaymentsReportData") != null)
                model = TempData.Get<PaymentReportSearchViewModel>("InitPaymentsReportData");

            CommandResult<IQueryable<ConfirmedPaymentReport>> result = _reportService.GetPaymentsReport(model);
            if (result.IsSuccess)
            {
                reportData = result.Data.ToList();
            }

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Portrait,
                FileName = $"Payments-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
		{
            PaymentReportSearchViewModel model = new PaymentReportSearchViewModel();

            List<ConfirmedPaymentReport> reportData = new List<ConfirmedPaymentReport>();
            if (TempData.Peek("InitPaymentsReportData") != null)
                model = TempData.Get<PaymentReportSearchViewModel>("InitPaymentsReportData");

            CommandResult<IQueryable<ConfirmedPaymentReport>> result = _reportService.GetPaymentsReport(model);
            if (result.IsSuccess)
            {
                reportData = result.Data.ToList();
            }

            DataTable reportList = new DataTable("ExportExcel");
			reportList.Columns.Add("#");
			reportList.Columns.Add(Captions.DocumentNumber);
			reportList.Columns.Add(Captions.FullName);
			reportList.Columns.Add(Captions.InstallmentDate);
			reportList.Columns.Add(Captions.InstallmentPrice);
			reportList.Columns.Add(Captions.InstallmentNumebr);
			reportList.Columns.Add(Captions.PaymentDate);
            reportList.Columns.Add(Captions.PaymentAmount);

			double[] ColumnWidth = new double[reportList.Columns.Count];

			ColumnWidth[0] = 8;
			ColumnWidth[1] = 25;
			ColumnWidth[2] = 25;
			ColumnWidth[3] = 25;
			ColumnWidth[4] = 25;
			ColumnWidth[5] = 25;
			ColumnWidth[6] = 25;
			ColumnWidth[7] = 25;
			int i = 1;
			foreach (var item in reportData)
			{
                reportList.Rows.Add
					(i++,
				   item.DocumentNumber,
				   item.FullName,
				   item.PersianInstallmentDate,
				   item.InstallmentAmount,
				   item.InstallmentNumber,
				   item.PersianPaymentDate,
				   item.PaymentAmount);
			}
			byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, Captions.PaymentReport, true);
			string filename = string.Format("{0}.xlsx", $"Payments" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
			Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
			return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
		}
		#endregion
	}
}
