using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerPymentsModel;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.Interfaces;
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
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.RequestsToEditInformation)]
    public class EditInformationController : Controller
    {
        private readonly IUserService _userService;
        private readonly FilePathAddress _filePathAddress;
        private readonly IFileService _fileService;
        private readonly ICustomerService _customerService;

        public EditInformationController(IUserService userService, IOptions<FilePathAddress> filePathAddressOptions, IFileService fileService, ICustomerService customerService)
        {
            _userService = userService;
            this._filePathAddress = filePathAddressOptions.Value;
            _fileService = fileService;
            _customerService = customerService;
        }




        #region درخواست ویرایش اطلاعات 
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EditInformationRequests_Read([DataSourceRequest] DataSourceRequest request, bool? isActive)
        {
            CommandResult<IQueryable<EditInformationRequestModel>> result = _userService.EditInformationRequestListAsQuerable(isActive);
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
            {
                dataSource = result.Data.ToDataSourceResult(request);
            }
            return Json(dataSource);
        }

        #region export data


        public JsonResult InitReportData(bool? isActive)
        {
            string model = isActive is null?string.Empty:isActive.Value.ToString();
            TempData.Set<string>("InitEditInformationRequestData", model);
            return Json(true);
        }

        public IActionResult ExportPdf()
        {
            bool? model = null;

            var reportData = new List<EditInformationRequestModel>();
            if (TempData.Peek("InitEditInformationRequestData") != null)
            {
                var tempResult = TempData.Get<string>("InitEditInformationRequestData");
                if (!string.IsNullOrEmpty(tempResult))
                    model = bool.Parse(tempResult);
            }
            CommandResult<IQueryable<EditInformationRequestModel>> result = _userService.EditInformationRequestListAsQuerable(model);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Portrait,
                FileName = $"EditInformationRequest-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            bool? model = null;

            var reportData = new List<EditInformationRequestModel>();
            if (TempData.Peek("InitEditInformationRequestData") != null)
            {
                var tempResult = TempData.Get<string>("InitEditInformationRequestData");
                if (!string.IsNullOrEmpty(tempResult))
                    model = bool.Parse(tempResult);
            }

            CommandResult<IQueryable<EditInformationRequestModel>> result = _userService.EditInformationRequestListAsQuerable(model);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.FullName);
            reportList.Columns.Add(Captions.NationalCode);
            reportList.Columns.Add(Captions.RequestDate);
            reportList.Columns.Add(Captions.Message);
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
                   item.NationalCode,
                   item.PersianRequestDate,
                   item.Message,
                   (item.IsActive?Captions.Active:Captions.DeActive));
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.EditInformationRequest} {Captions.Report}", true);
            string filename = string.Format("{0}.xlsx", $"EditInformationRequest" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion


        [HttpGet]
        public async Task<IActionResult> GetEditInformationRequestImage(string? imageName)
        {
            try
            {
                string paymentImagesPath = $"{_filePathAddress.EditInformationRequest}/{imageName}";
                var fileBytes = await _fileService.GetFileBytesAsync(paymentImagesPath);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(paymentImagesPath);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        [HttpGet]
        public async Task<IActionResult> ArchiveEditInformationRequest(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            CommandResult result = await _userService.ArchiveEditInformationRequestAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                #region log
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByEditInformationRequestIdAsync(id, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", customerNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    DescriptionPattern = AdminActivityLogDescriptions.RequestsToEditInformation_Edit_Archived,
                    AdminMenuId = (byte)ManagerPermission.RequestsToEditInformation,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                #endregion

                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            return Json(toastrResult);
        }

        

        [HttpGet]
        public async Task<IActionResult> RemoveEditInformationRequest(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult<string>();
            CommandResult result = await _userService.RemoveEditInformationRequestAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                #region log
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByEditInformationRequestIdAsync(id, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", customerNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    DescriptionPattern = AdminActivityLogDescriptions.RequestsToEditInformation_Delete,
                    AdminMenuId = (byte)ManagerPermission.RequestsToEditInformation,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                #endregion

                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            return Json(toastrResult);
        }
        #endregion
    }
}
