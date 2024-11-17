using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.Models.SellerModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.FAQViewModels;
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
using Gold.SharedKernel.Contract;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ManageFAQ)]
    public class FAQController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly IContentManagerService _contentManager;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IViewRenderService _renderService;
        public FAQController(ISettingService settingService, IViewRenderService renderService, IUserService userService, IFileService fileService, IContentManagerService contentManager)
        {
            _settingService = settingService;
            _renderService = renderService;
            _userService = userService;
            _fileService = fileService;
            _contentManager = contentManager;
        }

        public IActionResult Index()
        {
            CreateOrEditFAQViewModel model = new();
            model.Categories = _contentManager.GetFAQCategoryAsQuerable().Data
                        .OrderBy(x => x.OrderNo)
                        .Select(x => new SelectListItem
                        {
                            Text = x.Title,
                            Value = x.Id.ToString()
                        }).ToList();
            model.Categories.Add(new SelectListItem
            {
                Selected = true,
                Text = Captions.SelectIt,
                Value = string.Empty
            });
            return View(model);
        }


        #region FAQ
        [HttpGet]
        public async Task<IActionResult> CreateOrEditFAQ(int? id, CancellationToken cancellationToken)
        {
            ToastrResult<FAQModel> result = new();
            //string html = string.Empty;
            FAQModel model = new();
            if (id.HasValue)
            {
                CommandResult<FAQModel> commandResult = await _contentManager.GetFAQByIdAsync(id.Value, cancellationToken);

                if (commandResult.IsSuccess)
                    model = commandResult.Data;
                else
                    model = new();
            }
            else
                model = new();
            //html = await _renderService.RenderViewToStringAsync("_CreateOrEditFAQForm", model, this.ControllerContext);
            result = ToastrResult<FAQModel>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEditFAQ(CreateOrEditFAQViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult result = new();
            if (ModelState.IsValid)
            {
                var commandResult = await _contentManager.CreateOrEditFAQAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                {
                    if (model.Id == null)
                    {
                        //add log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", commandResult.Data.Question);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.ManageFAQ,
                            DescriptionPattern = AdminActivityLogDescriptions.FAQ_Insert,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    }
                    else
                    {
                        //edit log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", commandResult.Data.Question);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            AdminMenuId = (byte)ManagerPermission.ManageFAQ,
                            DescriptionPattern = AdminActivityLogDescriptions.FAQ_Edit,
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
        public async Task<JsonResult> RemoveFAQ(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            CommandResult<string> result = await _contentManager.RemoveFAQAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.ManageFAQ,
                    DescriptionPattern = AdminActivityLogDescriptions.FAQ_Delete,
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

        public IActionResult FAQList_Read([DataSourceRequest] DataSourceRequest request, int? CategoryId)
        {
          //  CommandResult<IQueryable<FAQModel>> result = _contentManager.GetFAQAsQuerable();
            var result = _contentManager.GetFAQAsList();
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
            {
                    if (CategoryId.HasValue)
                        result.Data = result.Data.Where(x => x.CategoryId == CategoryId.Value).ToList();

                dataSource = result.Data.ToDataSourceResult(request);
            }
            return Json(dataSource);
        }

        public async Task<JsonResult> ChangeFAQOrder(int id, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult result = await _contentManager.ChangeFAQOrderNumberAsync(id, isUp, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            return Json(toastrResult);
        }

        #region FAQ Export Data
        public JsonResult InitReportData()
        {
            return Json(true);
        }
        public IActionResult ExportPdf()
        {
            List<FAQModel> reportData = new();
            CommandResult<IQueryable<FAQModel>> result = _contentManager.GetFAQAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.OrderNo).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"FAQReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            List<FAQModel> reportData = new();
            CommandResult<IQueryable<FAQModel>> result = _contentManager.GetFAQAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.OrderNo).ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Question);
            reportList.Columns.Add(Captions.Answer);
            reportList.Columns.Add(Captions.Category);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.Question,
                   item.Answer,
                   item.Category
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.FAQ}", true);
            string filename = string.Format("{0}.xlsx", $"SellerReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion

        #endregion

        #region FAQ Category
        [HttpGet]
        public async Task<IActionResult> CreateOrEditFAQCategory(int? id, CancellationToken cancellationToken)
        {
            ToastrResult<string> result = new();
            string html = string.Empty;

            CreateOrEditFAQCategoryViewModel model = new();
            if (id.HasValue)
            {
                CommandResult<FAQCategoryModel> commandResult = await _contentManager.GetFAQCategoryByIdAsync(id.Value, cancellationToken);
                if (commandResult.IsSuccess)
                {
                    model.Id = commandResult.Data.Id;
                    model.Title = commandResult.Data.Title;
                }
            }
            else
                model = new();
            html = await _renderService.RenderViewToStringAsync("_CreateOrEditFAQCategoryForm", model, this.ControllerContext);
            result = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEditFAQCategory(CreateOrEditFAQCategoryViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult result = new();
            if (ModelState.IsValid)
            {
                var commandResult = await _contentManager.CreateOrEditFAQCategoryAsync(model, cancellationToken);
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
                            AdminMenuId = (byte)ManagerPermission.ManageFAQ,
                            DescriptionPattern = AdminActivityLogDescriptions.FAQCategory_Insert,
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
                            AdminMenuId = (byte)ManagerPermission.ManageFAQ,
                            DescriptionPattern = AdminActivityLogDescriptions.FAQCategory_Edit,
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
        public async Task<JsonResult> RemoveFAQCategory(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            CommandResult<string> result = await _contentManager.RemoveFAQCategoryAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.ManageFAQ,
                    DescriptionPattern = AdminActivityLogDescriptions.FAQCategory_Delete,
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
        public IActionResult FAQCategoryList_Read([DataSourceRequest] DataSourceRequest request)
        {
            CommandResult<IQueryable<FAQCategoryModel>> result = _contentManager.GetFAQCategoryAsQuerable();
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
                dataSource = result.Data.ToDataSourceResult(request);
            return Json(dataSource);
        }

        public async Task<JsonResult> ChangeFAQCategoryOrder(int id, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult result = await _contentManager.ChangeFAQCategoryOrderNumberAsync(id, isUp, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            return Json(toastrResult);
        }
        #endregion
    }
}
