using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SendMessageViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Data;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}",ManagerPermission.SendMessage)]
    public class SendMessageController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly ICustomerService _customerService;
        private readonly IFileService _fileService;
        public SendMessageController(IViewRenderService renderService, ICustomerService customerService, IFileService fileService)
        {
            _renderService = renderService;
            _customerService = customerService;
            _fileService = fileService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Send(SendMessageViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> result = new();
            string html = string.Empty;
            if (!ModelState.IsValid)
            {
                result = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html);
            }
            else
            {
                CommandResult<List<SendMessageModel>> listResult = await _customerService.GetCustomerListForSendMessageAsync(model, cancellationToken);
                if (listResult.IsSuccess)
                {
                    html = await _renderService.RenderViewToStringAsync("SendMessageSearchList", listResult.Data, this.ControllerContext);
                    result = ToastrResult<string>.Success(Captions.Success, listResult.Message, html);
                }
                else
                    result = ToastrResult<string>.Error(Captions.Error, listResult.Message, html);
            }
            result.Data = html;
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendToCustomers(SendMessageContentViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> result = new();
            string html = string.Empty;
            if (model.CustomerIds is null)
                ModelState.AddModelError(nameof(model.CustomerIds), UserMessages.NotSelectedRecored);

            if (!ModelState.IsValid)
            {
                result = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html);
            }
            else
            {
                CommandResult commandResult = await _customerService.SendMessageToCustomersAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                    result = ToastrResult<string>.Success(Captions.Success, commandResult.Message, html);
                else
                    result = ToastrResult<string>.Error(Captions.Error, commandResult.Message, html);
            }
            result.Data = html;
            return Json(result);
        }

        #region export data
        public JsonResult InitReportData(SendMessageViewModel model)
        {
            TempData.Set<SendMessageViewModel>("InitSendMessageReportData", model);
            return Json(true);
        }

        public async Task<IActionResult> ExportPdf()
        {
            SendMessageViewModel model = new();

            if (TempData.Peek("InitSendMessageReportData") != null)
                model = TempData.Get<SendMessageViewModel>("InitSendMessageReportData");

            List<SendMessageModel> reportData = new List<SendMessageModel>();
            CommandResult<List<SendMessageModel>> result = await _customerService.GetCustomerListForSendMessageAsync(model, default);
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.CustomerName).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"SendMessageReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public async Task<IActionResult> ExportExcel()
        {
            SendMessageViewModel model = new();

            if (TempData.Peek("InitSendMessageReportData") != null)
                model = TempData.Get<SendMessageViewModel>("InitSendMessageReportData");

            List<SendMessageModel> reportData = new List<SendMessageModel>();
            CommandResult<List<SendMessageModel>> result = await _customerService.GetCustomerListForSendMessageAsync(model, default);
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.CustomerName).ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.FullName);
            reportList.Columns.Add(Captions.Nationality);
            reportList.Columns.Add(Captions.NationalCode_UserName);
            reportList.Columns.Add(Captions.Mobile);
            reportList.Columns.Add(Captions.AccountStatus);
            reportList.Columns.Add(Captions.RegisteryDate);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            ColumnWidth[4] = 25;
            ColumnWidth[5] = 25;
            ColumnWidth[6] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.CustomerName,
                   item.Nationality,
                   item.NationalCode,
                   item.Mobile,
                   item.AccountStatus,
                   item.RegisterDate
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.CustomerInfo}", true);
            string filename = string.Format("{0}.xlsx", $"SendMessageReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion

    }
}
