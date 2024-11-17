using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerPymentsModel;
using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.ReportModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ReportViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Entities;
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
using Gold.ApplicationService.Contract.Abstractions;
using System.Threading;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}",ManagerPermission.PaymentsInPendingConfirmation)]
    public class CustomerPaymentController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly FilePathAddress _filePathAddress;
        private readonly ICustomerService _customerService;
        private readonly IFileService _fileService;
		private readonly IReportService _reportService;

		public CustomerPaymentController(ICustomerService customerService, IViewRenderService renderService, IOptions<FilePathAddress> filePathAddressOptions, IFileService fileService, IReportService reportService)
		{
			_customerService = customerService;
			_renderService = renderService;
			_filePathAddress = filePathAddressOptions.Value;
			_fileService = fileService;
			_reportService = reportService;
		}

		public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmPayment(long id, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<ConfirmCustomerPaymentViewModel> customerPaymentResult = await _customerService.GetCustomerPaymentForConfirmAsync(id, cancellationToken);
            string html = string.Empty;

            if (customerPaymentResult.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_ConfirmPayment", customerPaymentResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, customerPaymentResult.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, customerPaymentResult.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(ConfirmCustomerPaymentViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            if (model.IsPayInstallment)
            {
                if (string.IsNullOrEmpty(model.Description))
                    ModelState.AddModelError(nameof(model.Description), string.Format(ValidationMessages.Required, Captions.Description));
                if (string.IsNullOrEmpty(model.CustomerMessageContent))
                    ModelState.AddModelError(nameof(model.Description), string.Format(ValidationMessages.Required, Captions.CustomerMessageContent));
            }

            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html);
            }
            else
            {
                var logModel = new LogUserActivityModel()
                {
                    AdminMenuId = (byte)ManagerPermission.PaymentsInPendingConfirmation,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                CommandResult<ConfirmCustomerPaymentViewModel> confirmResultResult = await _customerService.ConfirmCustomerPaymentAsync(model, logModel, cancellationToken);

                if (confirmResultResult.IsSuccess)
                    toastrResult = ToastrResult<string>.Success(Captions.Success, confirmResultResult.Message, html);
                else
                    toastrResult = ToastrResult<string>.Error(Captions.Error, confirmResultResult.Message, html);
            }
           
            return Json(toastrResult);
        }

		[HttpGet]
		public async Task<IActionResult> RejectPayment(long id, CancellationToken cancellationToken)
		{
			ToastrResult<string> toastrResult = new ToastrResult<string>();
			CommandResult<RejectionCustomerPaymentViewModel> customerPaymentResult = await _customerService.GetCustomerPaymentForRejectAsync(id, cancellationToken);
			string html = string.Empty;

			if (customerPaymentResult.IsSuccess)
			{
				html = await _renderService.RenderViewToStringAsync("_RejectionPayment", customerPaymentResult.Data, this.ControllerContext);
				toastrResult = ToastrResult<string>.Success(Captions.Success, customerPaymentResult.Message, html);
			}
			else
			{
				toastrResult = ToastrResult<string>.Error(Captions.Error, customerPaymentResult.Message, html);
			}
			return Json(toastrResult);
		}


		[HttpPost]
        public async Task<IActionResult> RejectPayment(RejectionCustomerPaymentViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;

            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html);
            }
            else
            {
                var logModel = new LogUserActivityModel()
                {
                    AdminMenuId = (byte)ManagerPermission.PaymentsInPendingConfirmation,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                CommandResult<RejectionCustomerPaymentViewModel> confirmResultResult = await _customerService.RejectCustomerPaymentAsync(model, logModel, cancellationToken);

                if (confirmResultResult.IsSuccess)
                    toastrResult = ToastrResult<string>.Success(Captions.Success, confirmResultResult.Message, html);
                else
                    toastrResult = ToastrResult<string>.Error(Captions.Error, confirmResultResult.Message, html);
            }
            return Json(toastrResult);
        }

        public IActionResult PendingConfirmationList_Read([DataSourceRequest] DataSourceRequest request, PaymentReceiptsInPendingConfirmationSearchViewModel model)
        {
            var result = _customerService.GetPaymentReceiptsInPendingConfirmationListAsQuerable(model);

            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
            {
                dataSource = result.Data.ToDataSourceResult(request);
            }
            return Json(dataSource);
        }

		#region export data


		public JsonResult InitReportData(PaymentReceiptsInPendingConfirmationSearchViewModel model)
		{
			TempData.Set<PaymentReceiptsInPendingConfirmationSearchViewModel>("InitPaymentReceiptsInPendingReportData", model);
			return Json(true);
		}

		public IActionResult ExportPdf()
		{
			var model = new PaymentReceiptsInPendingConfirmationSearchViewModel();

			var reportData = new List<PaymentReceiptsInPendingConfirmationModel>();
			if (TempData.Peek("InitPaymentReceiptsInPendingReportData") != null)
				model = TempData.Get<PaymentReceiptsInPendingConfirmationSearchViewModel>("InitPaymentReceiptsInPendingReportData");

			CommandResult<IQueryable<PaymentReceiptsInPendingConfirmationModel>> result = _customerService.GetPaymentReceiptsInPendingConfirmationListAsQuerable(model);
			if (result.IsSuccess)
				reportData = result.Data.ToList();

			return new ViewAsPdf("ExportPdf", reportData, null)
			{
				PageSize = Size.A4,
				PageMargins = new Margins(8, 0, 4, 0),
				PageOrientation = Orientation.Portrait,
				FileName = $"PaymentReceiptsInPendingReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
			};
		}
		public IActionResult ExportExcel()
		{
			var model = new PaymentReceiptsInPendingConfirmationSearchViewModel();

			var reportData = new List<PaymentReceiptsInPendingConfirmationModel>();
			if (TempData.Peek("InitPaymentReceiptsInPendingReportData") != null)
				model = TempData.Get<PaymentReceiptsInPendingConfirmationSearchViewModel>("InitPaymentReceiptsInPendingReportData");

			CommandResult<IQueryable<PaymentReceiptsInPendingConfirmationModel>> result = _customerService.GetPaymentReceiptsInPendingConfirmationListAsQuerable(model);
			if (result.IsSuccess)
				reportData = result.Data.ToList();

			DataTable reportList = new DataTable("ExportExcel");
			reportList.Columns.Add("#");
			reportList.Columns.Add(Captions.DocumentNumber);
			reportList.Columns.Add(Captions.FullName);
			reportList.Columns.Add(Captions.DocumentDate);
			reportList.Columns.Add(Captions.InstallmentPrice);
			reportList.Columns.Add(Captions.PayDateTime);
			reportList.Columns.Add(Captions.PayAmount);
			reportList.Columns.Add(Captions.RegisterDate);
			reportList.Columns.Add(Captions.Status);

			double[] ColumnWidth = new double[reportList.Columns.Count];

			ColumnWidth[0] = 8;
			ColumnWidth[1] = 25;
			ColumnWidth[2] = 25;
			ColumnWidth[3] = 25;
			ColumnWidth[4] = 25;
			ColumnWidth[5] = 25;
			ColumnWidth[6] = 25;
			ColumnWidth[7] = 25;
			ColumnWidth[8] = 25;
			int i = 1;
			foreach (var item in reportData)
			{
				reportList.Rows.Add
					(i++,
				   item.DocumentNumber,
				   item.FullName,
				   item.PersianDocumentDate,
				   item.InstallmentAmount.FormatMoney(),
				   item.PersianPayDate,
				   item.PayAmount.FormatMoney(),
				   item.PersianRegisterDate,
				   item.StatusDescription
                   );
			}
			byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, Captions.PaymentReport, true);
			string filename = string.Format("{0}.xlsx", $"PaymentReceiptsInPendingReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
			Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
			return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
		}
		#endregion

		[HttpGet]
        public async Task<IActionResult> RemoveCustomerPayment(long id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult<string>();

            var logModel = new LogUserActivityModel()
            {
                AdminMenuId = (byte)ManagerPermission.PaymentsInPendingConfirmation,
                UserId = HttpContext.User.GetUserIdAsInt()
            };
            CommandResult deleteResult = await _customerService.RemoveCustomerPaymentAsync(id, logModel, cancellationToken);
            if (deleteResult.IsSuccess)
            {
                toastrResult = ToastrResult.Success(Captions.Success, deleteResult.Message);
            }
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, deleteResult.Message);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerPaymentImage(string? imageName)
        {
            string patch = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(imageName))
                    patch = $"{_filePathAddress.CustomerPaymentImages}/{imageName}";
                else
                patch = $"{_filePathAddress.CustomerPaymentImages}/{imageName}";
                var fileBytes = await _fileService.GetFileBytesAsync(patch);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                patch = $"{_filePathAddress.CustomerPaymentImages}/NoImage.png";
                var fileBytes = await _fileService.GetFileBytesAsync(patch);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
                return File(fileBytes, fileContentType);
            }
        }

    }
}
