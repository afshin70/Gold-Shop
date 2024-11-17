using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.SettingsModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.ApplicationService.Utility.Jobs;
using Gold.Domain.Enums;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;
using System.Threading;
using Gold.SharedKernel.Contract;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GoldPriceViewModels;
using Gold.ApplicationService.Contract.DTOs.Models.GoldPriceModels;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}")]
    public class SettingController : Controller
    {
        private readonly ISettingService _settingService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IViewRenderService renderService;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;

        public SettingController(ISettingService settingService, IOptions<FilePathAddress> filePathAddressOptions, IConfiguration configuration, IViewRenderService renderService, IUserService userService, IFileService fileService)
        {
            this._settingService = settingService;
            this._filePathAddress = filePathAddressOptions.Value;
            this._configuration = configuration;
            this.renderService = renderService;
            _userService = userService;
            _fileService = fileService;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region LoanSettings - تنظیمات وام
        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Setting)]
        public async Task<JsonResult> LoanSettings(CancellationToken cancellationToken)
        {
            ToastrResult<string> toastResult = new();
            LoanSettingsViewModel model = new();
            var result = await _settingService.GetSettingAsync<LoanSettingsViewModel>(Domain.Enums.SettingType.LoanSetting, cancellationToken);
            string html = string.Empty;

            if (result.IsSuccess)
            {
                model = result.Data;
                html = await renderService.RenderViewToStringAsync("_LoanSettingsForm", model, this.ControllerContext);
                toastResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                model = new()
                {
                    MonthlyProfitPercentage = 0.00m.ToString(),
                    PenaltyFactor = 0.00m.ToString(),
                    TimeToSendReminderMessagesToUsers = new TimeSpan(13, 00, 00) //"12:00"
                };
                html = await renderService.RenderViewToStringAsync("_LoanSettingsForm", model, this.ControllerContext);
                toastResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            return Json(toastResult);
        }

        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Setting)]
        public async Task<JsonResult> LoanSettings(LoanSettingsViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult result = new();
            // string html = string.Empty;
            if (model.MonthlyProfitPercentage is not null)
            {
                if (decimal.TryParse(model.MonthlyProfitPercentage, out decimal monthlyProfitPercentage))
                    if (monthlyProfitPercentage is > 100 or < 0)
                        ModelState.AddModelError(nameof(model.MonthlyProfitPercentage), string.Format(ValidationMessages.Range, Captions.MonthlyProfitPercentage, "0", "100"));
            }

            if (model.PenaltyFactor is not null)
            {
                if (decimal.TryParse(model.PenaltyFactor, out decimal penaltyFactor))
                    if (penaltyFactor is > 1 or < 0)
                        ModelState.AddModelError(nameof(model.PenaltyFactor), string.Format(ValidationMessages.Range, Captions.PenaltyFactor, "0", "1"));
            }

            if (!ModelState.IsValid)
            {
                // html = await renderService.RenderViewToStringAsync("_LoanSettingsForm", model, this.ControllerContext);
                result = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
                return Json(result);
            }
            var updateSettingResult = await _settingService.UpdateSettingAsync<LoanSettingsViewModel>(model, SettingType.LoanSetting, cancellationToken);
            //html = await renderService.RenderViewToStringAsync("_LoanSettingsForm", model, this.ControllerContext);
            if (updateSettingResult.IsSuccess)
            {
                //register hangFire setting
                #region set job for Raminder Message
                RecurringJob.AddOrUpdate<RaminderMessageJobService>(Guid.NewGuid().ToString(),
                        x => x.ReminderOneDayBeforeInstallmentDateAsync(),
                        Cron.Daily(model.TimeToSendReminderMessagesToUsers.Value.Hours, model.TimeToSendReminderMessagesToUsers.Value.Minutes), TimeZoneInfo.Local);
                #endregion

                #region set job for bairthday
                RecurringJob.AddOrUpdate<SendMessageBySubjectHappyBirthdayJobService>(Guid.NewGuid().ToString(),
                        x => x.SendMessageBySubjectHappyBirthdayAsync(),
                        Cron.Daily(model.TimeToSendReminderMessagesToUsers.Value.Hours, model.TimeToSendReminderMessagesToUsers.Value.Minutes), TimeZoneInfo.Local);
                #endregion

                //log activity
                var activity = new LogUserActivityModel
                {
                    ActivityType = AdminActivityType.Edit,
                    AdminMenuId = (byte)ManagerPermission.Setting,
                    DescriptionPattern = AdminActivityLogDescriptions.Setting_Edit_LoanSettings,
                    Parameters = null,
                    UserId = HttpContext.User.GetUserIdAsInt(),
                };
                await _userService.LogUserActivityAsync(activity, cancellationToken);

                result = ToastrResult.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            else
                result = ToastrResult.Error(Captions.Error, updateSettingResult.Message);

            return Json(result);
        }
        #endregion


        #region ScriptSettings - تنظیمات اسکریپت
        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Setting)]
        public async Task<JsonResult> ScriptSettings(CancellationToken cancellationToken)
        {
            ToastrResult<string> toastResult = new();
            ScriptsSettingsViewModel model = new();
            var result = await _settingService.GetSettingAsync<ScriptsSettingsViewModel>(Domain.Enums.SettingType.ScriptSetting, cancellationToken);
            string html = string.Empty;
            if (result.Data != null)
            {
                model = result.Data;

            }
            html = await renderService.RenderViewToStringAsync("_ScriptsForm", model, this.ControllerContext);
            toastResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);

            return Json(toastResult);
        }

        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Setting)]
        public async Task<JsonResult> ScriptSettings(ScriptsSettingsViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> result = new();
            string html = string.Empty;
            if (!ModelState.IsValid)
            {
                html = await renderService.RenderViewToStringAsync("_ScriptsForm", model, this.ControllerContext);
                result = ToastrResult<string>.Error(Captions.Error, UserMessages.FormNotValid, html);
                return Json(result);
            }
            var updateSettingResult = await _settingService.UpdateSettingAsync<ScriptsSettingsViewModel>(model, Domain.Enums.SettingType.ScriptSetting, cancellationToken);
            html = await renderService.RenderViewToStringAsync("_ScriptsForm", model, this.ControllerContext); ;
            if (updateSettingResult.IsSuccess)
            {
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    AdminMenuId = (byte)ManagerPermission.Setting,
                    DescriptionPattern = AdminActivityLogDescriptions.Setting_Edit_Scripts,
                    Parameters = null,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
              await  _userService.LogUserActivityAsync(logModel, cancellationToken);
                result = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
                result = ToastrResult<string>.Error(Captions.Error, updateSettingResult.Message, html);

            return Json(result);
        }
        #endregion

        #region Default Message Type متن پیشفرض پیام ها
        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Setting)]
        public async Task<JsonResult> CreateOrEditDefaultMessage(SettingType input = SettingType.MessageType_DocumentRegistration, CancellationToken cancellationToken = default)
        {
            ToastrResult<string> result = new();
            string html = string.Empty;
            DefaultMessageViewModel model = new();
            var messageTypeResult = await _settingService.GetSettingAsync<string>(input, cancellationToken);
            var allMessageTypes = _settingService.GetMessageTypesAsList().Data.Select(x => new SelectListItem
            {
                Selected = (x == input),
                Text = x.GetDisplayName(),
                Value = ((int)x).ToString()
            }).ToList();
            model = new()
            {
                Content = messageTypeResult.Data,
                ContentParameters = _settingService.GetMessageContentParameters(input).Data,
                MessageType = input,
                MessageTypes = allMessageTypes
            };
            html = await renderService.RenderViewToStringAsync("_DefaultMessagesForm", model, this.ControllerContext);
            result = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            return Json(result);
        }

        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Setting)]
        public async Task<JsonResult> CreateOrEditDefaultMessage(DefaultMessageViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> result = new();
            string html = string.Empty;



            ModelState.Remove(nameof(model.MessageType));
            ModelState.Remove(nameof(model.ContentParameters));
            ModelState.Remove(nameof(model.MessageTypes));
            model = new()
            {
                Content = model.Content,
                ContentParameters = _settingService.GetMessageContentParameters(model.MessageType).Data,
                MessageType = model.MessageType,
                MessageTypes = _settingService.GetMessageTypesAsList().Data.Select(x => new SelectListItem
                {
                    Selected = (x == model.MessageType),
                    Text = x.GetDisplayName(),
                    Value = x.ToString()
                }).ToList()
            };
            if (!ModelState.IsValid)
            {
                html = await renderService.RenderViewToStringAsync("_DefaultMessagesForm", model, this.ControllerContext);
                result = ToastrResult<string>.Error(Captions.Error, UserMessages.FormNotValid, html);
                return Json(result);
            }

            model.Content ??= string.Empty;
            var updateResult = await _settingService.UpdateSettingAsync<string>(model.Content, model.MessageType, cancellationToken);
            
            if (updateResult.IsSuccess)
            {
               
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    AdminMenuId = (byte)ManagerPermission.Setting,
                    DescriptionPattern = AdminActivityLogDescriptions.Setting_Edit_DefaultTextMessages,
                    Parameters = null,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                ModelState.Clear();
                model = new()
                {
                    Content = updateResult.Data,
                    ContentParameters = _settingService.GetMessageContentParameters(model.MessageType).Data,
                    MessageType = model.MessageType,
                    MessageTypes = _settingService.GetMessageTypesAsList().Data.Select(x => new SelectListItem
                    {
                        Selected = (x == model.MessageType),
                        Text = x.GetDisplayName(),
                        Value = x.ToString()
                    }).ToList()
                };
                html = await renderService.RenderViewToStringAsync("_DefaultMessagesForm", model, this.ControllerContext);
                result = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
            {
                html = await renderService.RenderViewToStringAsync("_DefaultMessagesForm", model, this.ControllerContext);
                result = ToastrResult<string>.Error(Captions.Error, updateResult.Message, html);
            }

            return Json(result);
        }
        #endregion

        #region Social Networks تنظیمات شبکه های اجتماعی

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public IActionResult SocialNetwork(CancellationToken cancellationToken)
        {
            return View();
        }
        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public async Task<IActionResult> CreateOrEditSocialNetwork(int id, CancellationToken cancellationToken)
        {
            string html = string.Empty;
            ToastrResult<string> toastResult = new();
            SocialNetworkViewModel model = new();
            if (id == 0)
            {
                //render add view
                model = new();
                html = await renderService.RenderViewToStringAsync("_SocialNetworkForm", model, this.ControllerContext);
                toastResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
            {
                //render edit view
                var result = await _settingService.GetSocialNetworkByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    html = await renderService.RenderViewToStringAsync("_SocialNetworkForm", result.Data, this.ControllerContext);
                    toastResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
                }
                else
                {
                    model = new();
                    html = await renderService.RenderViewToStringAsync("_SocialNetworkForm", model, this.ControllerContext);
                    toastResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
                }
            }
            return Json(toastResult);
        }
        
        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public async Task<JsonResult> CreateOrEditSocialNetwork(SocialNetworkViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult result = new();
            string html = string.Empty;
            if (model.SocialNetworkId != 0)
            {
                if (model.Icon is null)
                    ModelState.Remove(nameof(model.Icon));
            }
            if (!ModelState.IsValid)
            {
                result = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid); //ToastrResult<string>.Warning(Captions.Warning, UserMessages.FormNotValid, html);
                return Json(result);
            }
            if (model.SocialNetworkId == 0)
            {
                //add
                var addSocialNetworkResult = await _settingService.AddSocialNetworkAsync(model, cancellationToken);

                if (addSocialNetworkResult.IsSuccess)
                {
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.Title);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Insert,
                        AdminMenuId = (byte)ManagerPermission.SocialNetwork,
                        DescriptionPattern = AdminActivityLogDescriptions.Setting_Insert_SocialNetworks,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    result = ToastrResult.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted);
                }
                else
                {
                    result = ToastrResult.Error(Captions.Error, addSocialNetworkResult.Message);
                }
            }
            else
            {
                //edit
                var updateSocialNetworkResult = await _settingService.UpdateSocialNetworkAsync(model, cancellationToken);

                if (updateSocialNetworkResult.IsSuccess)
                {
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.Title);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Edit,
                        AdminMenuId = (byte)ManagerPermission.SocialNetwork,
                        DescriptionPattern = AdminActivityLogDescriptions.Setting_Edit_SocialNetworks,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);

                    result = ToastrResult.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted);
                }
                else
                {
                    result = ToastrResult.Error(Captions.Error, updateSocialNetworkResult.Message);
                }
            }


            return Json(result);
        }
       
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public async Task<IActionResult> RemoveSocialNetwork(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            var result = await _settingService.RemoveSocialNetworkAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.SocialNetwork,
                    DescriptionPattern = AdminActivityLogDescriptions.Setting_Delete_SocialNetworks,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);

                toastrResult = ToastrResult.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }

            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public IActionResult SocialNetworkList_Read([DataSourceRequest] DataSourceRequest request)
        {
            var list = _settingService.GetSocialNetworkAsIQueryable().Data.Select(x => new SocialNetworkModel
            {
                Id = x.Id,
                Image = $"/{_filePathAddress.SocialNetworkIcon}/{x.ImageName}",
                Title = x.Title,
                Url = x.Url,
            });

            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result);
        }

        #region export data

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public JsonResult InitReportData(PaymentReportSearchViewModel model)
        {

            //TempData.Set<PaymentReportSearchViewModel>("InitPaymentsReportData", model);
            return Json(true);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public IActionResult ExportPdf()
        {
            List<SocialNetworkReportModel> reportData = _settingService.GetSocialNetworkAsIQueryable().Data.Select(x => new SocialNetworkReportModel
            {
                Id = x.Id,
                Title = x.Title,
                Url = x.Url
            }).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Portrait,
                FileName = $"SocialNetworks-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SocialNetwork)]
        public IActionResult ExportExcel()
        {
            List<SocialNetworkReportModel> reportData = _settingService.GetSocialNetworkAsIQueryable().Data.Select(x => new SocialNetworkReportModel
            {
                Id = x.Id,
                Title = x.Title,
                Url = x.Url
            }).ToList();

            DataTable paymentReportList = new DataTable("ExportExcel");
            paymentReportList.Columns.Add("#");
            paymentReportList.Columns.Add(Captions.Title);
            paymentReportList.Columns.Add(Captions.Url);

            double[] ColumnWidth = new double[paymentReportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                paymentReportList.Rows.Add
                    (i++,
                   item.Title,
                   item.Url);
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(paymentReportList, ColumnWidth, Captions.PaymentReport, true);
            string filename = string.Format("{0}.xlsx", $"SocialNetworks" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion

        #endregion


        #region نرخ طلا

        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.GoldPrice)]
        public async Task<IActionResult> GoldPrice(CancellationToken cancellationToken)
        {
			GoldPriceViewModel model=new GoldPriceViewModel();
			//var result = await _settingService.GetLastGoldPriceInfoAsync(cancellationToken);
   //         if (result.IsSuccess & result.Data !=null)
   //             model = result.Data;
			return View(model);
        }

        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.GoldPrice)]
        public async Task<IActionResult> AddGoldPriceInfo(GoldPriceViewModel model,CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            }
            else
            {
                model.UserId= User.GetUserIdAsInt();
                CommandResult<GoldPriceViewModel> result =await _settingService.AddGoldPriceInfoAsync(model, cancellationToken);
                if (result.IsSuccess)
                {
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.Karat18);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Insert,
                        AdminMenuId = (byte)ManagerPermission.SocialNetwork,
                        DescriptionPattern = AdminActivityLogDescriptions.Setting_Insert_GoldPrice,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    toastrResult = ToastrResult.Success(Captions.Success, result.Message);
                }
                else
                    toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.GoldPrice)]
        public async Task<IActionResult> RemoveGoldPrice(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            var result = await _settingService.RemoveGoldPriceAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.SocialNetwork,
                    DescriptionPattern = AdminActivityLogDescriptions.Setting_Delete_GoldPrice,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                toastrResult = ToastrResult.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }

            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.GoldPrice)]
        public IActionResult GoldPriceList_Read([DataSourceRequest] DataSourceRequest request)
        {
            var list = _settingService.GetGoldPriceAsIQueryable().Data
                .OrderByDescending(x=>x.RegisterDate)
                .Select(x => new GoldPriceGridModel
                    {
                        Id = x.Id,
                        OneGram18KaratGold=$"{x.Karat18.ToString("N0")} {Captions.Tooman}",
                        RegisterDate=x.RegisterDate.GeorgianToPersian(ShowMode.OnlyDateAndTime),
                        UserName=x.User.FullName
                    });

            DataSourceResult result = list.ToDataSourceResult(request);
            return Json(result);
        }

        #region export data

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.GoldPrice)]
        public JsonResult InitGoldPriceReportData()
        {

            //TempData.Set<PaymentReportSearchViewModel>("InitPaymentsReportData", model);
            return Json(true);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.GoldPrice)]
        public IActionResult ExportGoldPricesPdf()
        {
            var reportData = _settingService.GetGoldPriceAsIQueryable().Data
                .OrderByDescending(x => x.RegisterDate)
                .Select(x => new GoldPriceExportPdfModel
                {
                    Id = x.Id,
                    Karat18 = x.Karat18.ToString("N0"),
                    RegisterDate = x.RegisterDate.GeorgianToPersian(ShowMode.OnlyDateAndTime),
                    UserName = x.User.FullName,
                    Anas=x.Anas!=null? x.Anas.Value.ToString("N0"):string.Empty,
                    Coin=x.Coin!=null? x.Coin.Value.ToString("N0"):string.Empty,
                    GramCoin=x.GramCoin != null? x.GramCoin.Value.ToString("N0"):string.Empty,
                    HalfCoin=x.HalfCoin != null? x.HalfCoin.Value.ToString("N0"):string.Empty,
                    OldCoin=x.OldCoin != null? x.OldCoin.Value.ToString("N0"):string.Empty,
                    QuarterCoin=x.QuarterCoin != null? x.QuarterCoin.Value.ToString("N0"):string.Empty,
                    Shekel=x.Shekel != null? x.Shekel.Value.ToString("N0"):string.Empty,
                }).ToList();

            return new ViewAsPdf("ExportGoldPricesPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Portrait,
                FileName = $"GoldPrices-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.GoldPrice)]
        public IActionResult ExportGoldPricesExcel()
        {
            var reportData = _settingService.GetGoldPriceAsIQueryable().Data
                .OrderByDescending(x => x.RegisterDate)
                .Select(x => new GoldPriceExportPdfModel
                {
                    Id = x.Id,
                    Karat18 = x.Karat18.ToString("N0"),
                    RegisterDate = x.RegisterDate.GeorgianToPersian(ShowMode.OnlyDateAndTime),
                    UserName = x.User.FullName,
                    Anas = x.Anas != null ? x.Anas.Value.ToString("N0") : string.Empty,
                    Coin = x.Coin != null ? x.Coin.Value.ToString("N0") : string.Empty,
                    GramCoin = x.GramCoin != null ? x.GramCoin.Value.ToString("N0") : string.Empty,
                    HalfCoin = x.HalfCoin != null ? x.HalfCoin.Value.ToString("N0") : string.Empty,
                    OldCoin = x.OldCoin != null ? x.OldCoin.Value.ToString("N0") : string.Empty,
                    QuarterCoin = x.QuarterCoin != null ? x.QuarterCoin.Value.ToString("N0") : string.Empty,
                    Shekel = x.Shekel != null ? x.Shekel.Value.ToString("N0") : string.Empty,
                });

            DataTable paymentReportList = new DataTable("ExportExcel");
            paymentReportList.Columns.Add("#");
            paymentReportList.Columns.Add(Captions.Shekel);
            paymentReportList.Columns.Add(Captions.GoldAnas);
            paymentReportList.Columns.Add(Captions.OneGram18KaratGold);
            paymentReportList.Columns.Add(Captions.BankCoin);
            paymentReportList.Columns.Add(Captions.BankHalfCoin);
            paymentReportList.Columns.Add(Captions.BankQuarterCoin);
            paymentReportList.Columns.Add(Captions.GramCoin);
            paymentReportList.Columns.Add(Captions.OldCoin);

            double[] ColumnWidth = new double[paymentReportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 35;
            ColumnWidth[2] = 35;
            ColumnWidth[3] = 35;
            ColumnWidth[4] = 35;
            ColumnWidth[5] = 35;
            ColumnWidth[6] = 35;
            ColumnWidth[7] = 35;
            ColumnWidth[8] = 35;
            int i = 1;
            foreach (var item in reportData)
            {
                paymentReportList.Rows.Add
                    (i++,
                   item.Shekel,
                   item.Anas,
                   item.Karat18,
                   item.Coin,
                   item.HalfCoin,
                   item.QuarterCoin,
                   item.GramCoin,
                   item.OldCoin);
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(paymentReportList, ColumnWidth, Captions.PaymentReport, true);
            string filename = string.Format("{0}.xlsx", $"GoldPrices" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion
        #endregion

    }
}
