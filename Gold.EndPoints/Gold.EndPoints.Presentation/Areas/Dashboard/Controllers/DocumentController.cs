using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Mvc;
using Gold.SharedKernel.ExtentionMethods;
using Gold.EndPoints.Presentation.Utility.Statics;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gold.SharedKernel.DTO.FileAddress;
using Microsoft.Extensions.Options;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.SharedKernel.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Http.Metadata;
using Gold.Domain.Entities;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}")]
    public class DocumentController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        private readonly ISMSSender _smsSender;
        private readonly IFileService _fileService;
        private readonly ICustomerMessageService _customerMessageService;
        private readonly FilePathAddress _filePathAddress;

        public DocumentController(
            IViewRenderService renderService,
            ICustomerService customerService,
            ISMSSender smsSender,
            IFileService fileService,
            IOptions<FilePathAddress> filePathAddressOptions,
            IUserService userService,
            ICustomerMessageService customerMessageService)
        {
            _renderService = renderService;
            this._customerService = customerService;
            this._smsSender = smsSender;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
            _userService = userService;
            _customerMessageService = customerMessageService;
        }

        #region ایجاد سند
        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public IActionResult CreateDocument(CancellationToken cancellationToken)
        {
            return View();
        }

        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public async Task<IActionResult> CreateDocument(CreateDocumentViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult<List<InstallmentDateModel>>();

            if (model.Collaterals == null || model.Collaterals.Count < 1)
                ModelState.AddModelError(nameof(model.Collaterals), UserMessages.AtLeastOneCollateralIsRequiredForTheDocument);

            var nationalCodeValidator = new PersianNationalCode();

            if (model.NationalCode is not null & model.Nationality is not null)
                if (model.Nationality == NationalityType.Iranian)
                    if (!nationalCodeValidator.IsValid(model.NationalCode))
                        ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.Invalid, Captions.National_Identification_Code));

            if (!ModelState.IsValid)
            {
                //invalid data
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, Captions.Warning);
            }
            else
            {
                CommandResult<bool> existResult = await _customerService.IsExistCustomerByNationalCodeAsync(model.NationalCode, cancellationToken);
                if (existResult.IsSuccess && model.IsNewCstomer)
                {
                    //new customer, is exist
                    toastrResult = ToastrResult.Error(Captions.Error, string.Format(UserMessages.IsDuplicated, Captions.NationalCode));
                }
                else
                {
                    var result = await _customerService.CreateDocumentAsync(model, cancellationToken);
                    if (result.IsSuccess)
                    {
                        //log new doc is added
                        var logNewDocParams = new Dictionary<string, string>();
                        logNewDocParams.Add("0", model.DocumentNo.ToString());
                        logNewDocParams.Add("1", model.FullName ?? string.Empty);
                        var logNewDocModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.NewDocument,
                            DescriptionPattern = AdminActivityLogDescriptions.Document_Insert,
                            Parameters = logNewDocParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logNewDocModel, cancellationToken);

                        if (model.IsNewCstomer)
                        {
                            //log add customer in new document
                            var logParams = new Dictionary<string, string>();
                            logParams.Add("0", model.FullName ?? string.Empty);
                            var logModel = new LogUserActivityModel()
                            {
                                ActivityType = AdminActivityType.Insert,
                                AdminMenuId = (byte)ManagerPermission.NewDocument,
                                DescriptionPattern = AdminActivityLogDescriptions.Document_Insert_Customer,
                                Parameters = logParams,
                                UserId = HttpContext.User.GetUserIdAsInt()
                            };
                            await _userService.LogUserActivityAsync(logModel, cancellationToken);
                        }

                        toastrResult = ToastrResult.Success(Captions.Success, result.Message);
                    }
                    else
                        toastrResult = ToastrResult.Error(Captions.Error, result.Message);
                }

            }

            return Json(toastrResult);
        }

        #region ویرایش اطلاعات مشتری در سند جدید
        [HttpGet]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public async Task<JsonResult> EditCustomerSummaryInfo(string nationalCode, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            if (string.IsNullOrEmpty(nationalCode))
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, string.Format(ValidationMessages.Required, Captions.NationalCode), html);
            }
            else
            {
                var result = await _customerService.GetCustomerSummaryInfo(nationalCode, cancellationToken);
                if (result.IsSuccess)
                {
                    EditCustomerSummaryInfoWithEssentialTelAndCardNoViewModel model = new();
                    var essentialTellsResult = await _customerService.GetCustomerEssentialTelListAsync(result.Data.CustomerId, cancellationToken);
                    model.EditCustomerSummaryInfo = result.Data;
                    if (essentialTellsResult.IsSuccess)
                    {
                        model.EssentialTel = new EssentialTelViewModel
                        {
                            CustomerId = result.Data.CustomerId,
                            EssentialTelId = 0,
                            EssentialTels = essentialTellsResult.Data,
                        };

                    }
                    else
                    {
                        model.EssentialTel = new EssentialTelViewModel
                        {
                            CustomerId = result.Data.CustomerId,
                            EssentialTelId = 0,
                            EssentialTels = new List<EssentialTelModel>(),
                        };
                    }

                    var bankCardNumbersResult = await _customerService.GetCustomerCardNumberListAsync(result.Data.CustomerId, cancellationToken);
                    if (bankCardNumbersResult.IsSuccess)
                    {
                        model.CardNumber = new()
                        {
                            CustomerId = result.Data.CustomerId,
                            CardNumberId = 0,
                            CardNumbers = bankCardNumbersResult.Data
                        };
                    }
                    else
                    {
                        model.CardNumber = new()
                        {
                            CustomerId = result.Data.CustomerId,
                            CardNumberId = 0,
                            CardNumbers = new()
                        };
                    }
                    html = await _renderService.RenderViewToStringAsync("_EditCustomerSummaryInfo", model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
                }
                else
                {
                    toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.UserNotFound, html);
                }
            }
            return Json(toastrResult);
        }

        [HttpPost]
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public async Task<JsonResult> EditCustomerSummaryInfo(EditCustomerSummaryInfoViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            if (!ModelState.IsValid)
            {
                html = await _renderService.RenderViewToStringAsync("_CreateOrEditCustomerSummaryInfo", model, this.ControllerContext);
                string errorMessages = UserMessages.FormNotValid;
                foreach (var errMsg in ModelState.GetAllErrorMessages())
                {
                    errorMessages += $"<br>{errMsg}";
                }
                toastrResult = ToastrResult<string>.Error(Captions.Error, errorMessages, html);
                return Json(toastrResult);
            }
            else
            {
                var updateResult = await _customerService.UpdateCustomerSummaryInfoAsync(model, cancellationToken);
                html = await _renderService.RenderViewToStringAsync("_CreateOrEditCustomerSummaryInfo", model, this.ControllerContext);
                if (updateResult.IsSuccess)
                {
                    #region log
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.CustomerName);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Edit,
                        AdminMenuId = (byte)ManagerPermission.NewDocument,
                        DescriptionPattern = AdminActivityLogDescriptions.Document_Edit_CustomerInfo,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    #endregion
                    toastrResult = ToastrResult<string>.Success(Captions.Success, updateResult.Message, html);
                }
                else
                    toastrResult = ToastrResult<string>.Error(Captions.Error, updateResult.Message, html);

                return Json(toastrResult);
            }

        }

        #endregion
        #region شماره ضروری
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpGet]
        public async Task<JsonResult> EditEssentialTel(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<EssentialTelModel> result = await _customerService.GetEssentialTelAsync(id, customerId, cancellationToken);
            EssentialTelViewModel model = new();
            string html = string.Empty;
            string viewName = "_CreateOrEditCustomerEssentialTel";
            if (result.IsSuccess)
            {
                model.CustomerId = result.Data.CustomerId;
                model.EssentialTelId = result.Data.Id;
                model.RelationShip = result.Data.RelationShip;
                model.Tel = result.Data.Tel;

                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);

                toastrResult = ToastrResult<string>.Success(Captions.Warning, result.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);

                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpPost]
        public async Task<JsonResult> AddOrEditEssentialNumber(EssentialTelViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            var listResult = await _customerService.GetCustomerEssentialTelListAsync(model.CustomerId, cancellationToken);
            if (listResult.IsSuccess)
                model.EssentialTels = listResult.Data;
            string viewName = "_CreateOrEditCustomerEssentialTel";
            string html = string.Empty;
            if (!ModelState.IsValid)
            {
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, Captions.Warning, html);
            }
            else
            {
                CommandResult<EssentialTelViewModel> addOrEditEssentialNumberResult = new CommandResult<EssentialTelViewModel>();
                //valid model
                if (model.EssentialTelId == 0) //add new
                    addOrEditEssentialNumberResult = await _customerService.AddEssentialTelAsync(model, cancellationToken);
                else //edit
                    addOrEditEssentialNumberResult = await _customerService.EditEssentialTelAsync(model, cancellationToken);
                //check result
                if (addOrEditEssentialNumberResult.IsSuccess)
                {
                    #region log
                    CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(model.CustomerId, cancellationToken);
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.Tel);
                    logParams.Add("1", customerNameResult.Data);
                    var logModel = new LogUserActivityModel()
                    {
                        AdminMenuId = (byte)ManagerPermission.NewDocument,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    if (model.EssentialTelId <= 0)
                    {
                        //add
                        logModel.ActivityType = AdminActivityType.Insert;
                        logModel.DescriptionPattern = AdminActivityLogDescriptions.Document_Insert_EssentialNumber;
                    }
                    else
                    {
                        //edit
                        logModel.ActivityType = AdminActivityType.Edit;
                        logModel.DescriptionPattern = AdminActivityLogDescriptions.Document_Edit_EssentialNumber;
                    }
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    #endregion

                    ModelState.Clear();
                    model.CustomerId = addOrEditEssentialNumberResult.Data.CustomerId;
                    model.EssentialTelId = 0;
                    model.Tel = string.Empty;
                    model.RelationShip = string.Empty;
                    listResult = await _customerService.GetCustomerEssentialTelListAsync(model.CustomerId, cancellationToken);
                    if (listResult.IsSuccess)
                        model.EssentialTels = listResult.Data;
                    html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, addOrEditEssentialNumberResult.Message, html);
                }
                else
                {
                    html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Error(Captions.Error, addOrEditEssentialNumberResult.Message, html);
                }

            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public async Task<JsonResult> RemoveEssentialTel(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CommandResult<string> result = await _customerService.RemoveEssentialNumberAsync(id, customerId, cancellationToken);
            if (result.IsSuccess)
            {
                List<EssentialTelModel> essentialTels = new List<EssentialTelModel>();
                var listResult = await _customerService.GetCustomerEssentialTelListAsync(customerId, cancellationToken);
                if (listResult.IsSuccess)
                    essentialTels = listResult.Data;
                #region log
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(customerId, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                logParams.Add("1", customerNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.NewDocument,
                    DescriptionPattern = AdminActivityLogDescriptions.Document_Delete_EssentialNumber,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                #endregion

                html = await _renderService.RenderViewToStringAsync("_EssentialTelList", essentialTels, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public async Task<JsonResult> ChangeEssentialTelOrder(long id, int customerId, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult result = await _customerService.ChangeEssentialTelOrderNumberAsync(id, isUp, cancellationToken);
            List<EssentialTelModel> model = new List<EssentialTelModel>();
            string html = string.Empty;


            if (result.IsSuccess)
            {
                var listResult = await _customerService.GetCustomerEssentialTelListAsync(customerId, cancellationToken);
                if (listResult.IsSuccess)
                    model = listResult.Data.ToList();
                html = await _renderService.RenderViewToStringAsync("_EssentialTelList", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        #endregion

        #region شماره کارتهای بانکی
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpGet]
        public async Task<JsonResult> EditCardNumber(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var result = await _customerService.GetCardNumberAsync(id, customerId, cancellationToken);
            CardNumberViewModel model = new();
            string html = string.Empty;
            string viewName = "_CreateOrEditCustomerBankCardsNo";
            if (result.IsSuccess)
            {
                model.CustomerId = result.Data.CustomerId;
                model.CardNumberId = result.Data.Id;
                model.CardNumber = result.Data.Number;
                model.Owner = result.Data.Owner;

                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Warning, result.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpPost]
        public async Task<JsonResult> AddOrEditBankCardNumber(CardNumberViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            var listResult = await _customerService.GetCustomerCardNumberListAsync(model.CustomerId, cancellationToken);
            if (listResult.IsSuccess)
                model.CardNumbers = listResult.Data;
            string viewName = "_CreateOrEditCustomerBankCardsNo";
            string html = string.Empty;
            if (!ModelState.IsValid)
            {
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, Captions.Warning, html);
            }
            else
            {
                CommandResult<CardNumberViewModel> addOrEditBankCardNumberResult = new CommandResult<CardNumberViewModel>();
                //valid model
                if (model.CardNumberId == 0) //add new
                    addOrEditBankCardNumberResult = await _customerService.AddCardNumberAsync(model, cancellationToken);
                else //edit
                    addOrEditBankCardNumberResult = await _customerService.EditCardNumberAsync(model, cancellationToken);
                //check result
                if (addOrEditBankCardNumberResult.IsSuccess)
                {
                    #region log
                    CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(model.CustomerId, cancellationToken);
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", model.CardNumber);
                    logParams.Add("1", customerNameResult.Data);
                    var logModel = new LogUserActivityModel()
                    {
                        AdminMenuId = (byte)ManagerPermission.NewDocument,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    if (model.CardNumberId <= 0)
                    {
                        //add
                        logModel.ActivityType = AdminActivityType.Insert;
                        logModel.DescriptionPattern = AdminActivityLogDescriptions.Document_Insert_BankCardNumber;
                    }
                    else
                    {
                        //edit
                        logModel.ActivityType = AdminActivityType.Edit;
                        logModel.DescriptionPattern = AdminActivityLogDescriptions.Document_Edit_BankCardNumber;
                    }
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    #endregion

                    ModelState.Clear();
                    model.CustomerId = addOrEditBankCardNumberResult.Data.CustomerId;
                    model.CardNumberId = 0;
                    model.Owner = string.Empty;
                    model.CardNumber = string.Empty;
                    listResult = await _customerService.GetCustomerCardNumberListAsync(model.CustomerId, cancellationToken);
                    if (listResult.IsSuccess)
                        model.CardNumbers = listResult.Data;
                    html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, addOrEditBankCardNumberResult.Message, html);
                }
                else
                {
                    html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Error(Captions.Error, addOrEditBankCardNumberResult.Message, html);
                }

            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public async Task<JsonResult> RemoveCardNumber(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CommandResult<string> result = await _customerService.RemoveCardNumberAsync(id, customerId, cancellationToken);
            if (result.IsSuccess)
            {
                List<EssentialTelModel> essentialTels = new List<EssentialTelModel>();
                var listResult = await _customerService.GetCustomerEssentialTelListAsync(customerId, cancellationToken);
                if (listResult.IsSuccess)
                    essentialTels = listResult.Data;
                #region log
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(customerId, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                logParams.Add("1", customerNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.NewDocument,
                    DescriptionPattern = AdminActivityLogDescriptions.Document_Delete_BankCardNumber,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                #endregion

                html = await _renderService.RenderViewToStringAsync("_BankCardNumberList", essentialTels, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        public async Task<JsonResult> ChangeCardNumberOrder(long id, int customerId, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CommandResult result = await _customerService.ChangeCardNumberOrderNumberAsync(id, isUp, cancellationToken);
            List<CardNumberModel> model = new();
            if (result.IsSuccess)
            {
                var listResult = await _customerService.GetCustomerCardNumberListAsync(customerId, cancellationToken);
                if (listResult.IsSuccess)
                    model = listResult.Data.ToList();
                html = await _renderService.RenderViewToStringAsync("_BankCardNumberList", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        #endregion

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpGet]
        public async Task<IActionResult> GetInstallmentAmount(string remainAmount, byte installmentCount, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            long remainAmountValue = 0;
            long.TryParse(remainAmount.Replace(",", "").ToString(), out remainAmountValue);
            var result = await _customerService.InstallmentAmountCalculator(remainAmountValue, installmentCount, cancellationToken);
            if (result.IsSuccess)
            {
                toastrResult = ToastrResult<string>.Success(Captions.Success, UserMessages.InstallmentAmountIsCalculated, result.Data.ToString("N0"));
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, result.Data.ToString("N0"));
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpGet]
        public async Task<JsonResult> GetCustomerInfo(string nationalCode, CancellationToken cancellationToken)
        {
            ToastrResult<DocumentOwnerInformationModel> toastrResult = new ToastrResult<DocumentOwnerInformationModel>();
            var result = await _customerService.GetDocumentOwnerInfoAsync(nationalCode, cancellationToken);
            if (result.IsSuccess)
            {
                toastrResult = ToastrResult<DocumentOwnerInformationModel>.Success(Captions.Success, result.Message, result.Data);
            }
            else
            {
                result.Data = new DocumentOwnerInformationModel();
                toastrResult = ToastrResult<DocumentOwnerInformationModel>.Error(Captions.Error, result.Message, result.Data);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpGet]
        public JsonResult GetInstallmentsDate(byte installmentCount, string installmentDate)
        {
            ToastrResult<List<InstallmentDateModel>> toastrResult = new ToastrResult<List<InstallmentDateModel>>();
            CommandResult<List<InstallmentDateModel>> result = _customerService.CalculateInstallmentDate(installmentCount, installmentDate);
            if (result.IsSuccess)
            {
                toastrResult = ToastrResult<List<InstallmentDateModel>>.Success(Captions.Success, result.Message, result.Data);
            }
            else
            {
                toastrResult = ToastrResult<List<InstallmentDateModel>>.Error(Captions.Error, result.Message, result.Data);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpGet]
        public async Task<IActionResult> ValidateStepOne(int docNumber, string? nationalCode, string? documentDate, NationalityType? nationality, CancellationToken cancellationToken)
        {
            ToastrResult<bool> toastrResult = ToastrResult<bool>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, false);

            string msg = string.Empty;

            #region validate doc number
            if (docNumber == 0)
            {
                msg = string.Format(ValidationMessages.Required, Captions.DocumentNumber);
            }
            else if (docNumber < 0 | docNumber > int.MaxValue)
            {
                msg += $"<br/>{string.Format(ValidationMessages.Invalid, Captions.DocumentNumber)}";
            }
            else
            {
                CommandResult<bool> result = await _customerService.IsExistDocumentByStatusAnNumberAsync(DocumentStatus.Active, docNumber, cancellationToken);
                if (result.IsSuccess)
                {
                    //is exist its valid
                    msg += $"<br/>{string.Format(UserMessages.IsDuplicated, Captions.DocumentNumber)}";
                }
            }
            #endregion

            #region validate document Date
            PersianDate persianDateValidator = new PersianDate("/");
            if (string.IsNullOrEmpty(documentDate))
            {
                msg += $"<br/>{string.Format(ValidationMessages.Required, Captions.Date)}";
            }
            else if (!persianDateValidator.IsValid(documentDate))
            {
                msg += $"<br/>{string.Format(ValidationMessages.Invalid, Captions.Date)}";
            }
            else
            {
                string[] date = documentDate.Split("/");
                int day = int.Parse(date[2]);
                if (day == 30 | day == 31)
                {
                    msg += $"<br/>{UserMessages.PersianDateCanNotBe31And30}";
                }
            }
            #endregion


            #region validate national code and nationalityType
            if (nationality is null)
            {
                msg += $"<br/>{string.Format(ValidationMessages.Required, Captions.Nationality)}";
            }
            else
            {
                var nationalCodeValidator = new PersianNationalCode();
                if (nationalCode is not null)
                {
                    if (nationalCode.Length < 3)
                    {
                        msg += $"<br/>{string.Format(ValidationMessages.MinLength, Captions.National_Identification_Code, "3")}";
                    }
                    else
                    {
                        if (nationality == NationalityType.Iranian)
                        {
                            if (!nationalCodeValidator.IsValid(nationalCode))
                            {
                                msg += $"<br/>{string.Format(ValidationMessages.Invalid, Captions.National_Identification_Code)}";
                            }
                        }
                        if (string.IsNullOrEmpty(msg))
                        {
                            CommandResult<bool> existResult = await _customerService.IsExistCustomerByNationalCodeAsync(nationalCode, cancellationToken);
                            if (existResult.IsSuccess)
                            {
                                CommandResult<bool> activeResult = await _customerService.IsActiveCustomerByNationalCodeAsync(nationalCode, cancellationToken);
                                if (activeResult.IsSuccess)
                                    toastrResult.Data = true;
                                else
                                    msg += $"<br/>{activeResult.Message}";
                            }
                        }
                    }
                }
                else
                {
                    msg += $"<br/>{string.Format(ValidationMessages.Required, Captions.National_Identification_Code)}";
                }
            }

            #endregion
            if (!string.IsNullOrEmpty(msg))
                toastrResult = ToastrResult<bool>.Error(Captions.Error, msg, false);
            return Json(toastrResult);
        }


        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpGet]
        public async Task<IActionResult> ValidateDocDate(string? documentDate, CancellationToken cancellationToken)
        {
            ToastrResult<bool> toastrResult = ToastrResult<bool>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, false);
            string msg = string.Empty;

            PersianDate persianDateValidator = new PersianDate("/");
            var docDate = documentDate.ParsePersianToGorgian();

            if (string.IsNullOrEmpty(documentDate))
                msg += $"<br/>{string.Format(ValidationMessages.Required, Captions.Date)}";
            else if (!persianDateValidator.IsValid(documentDate))
                msg += $"<br/>{string.Format(ValidationMessages.Invalid, Captions.Date)}";
            else if (docDate is null)
                msg += $"<br/>{string.Format(ValidationMessages.Invalid, Captions.Date)}";
            else
            {
                string[] date = documentDate.Split("/");
                int day = int.Parse(date[2]);
                if (day == 30 | day == 31)
                    msg += $"<br/>{UserMessages.PersianDateCanNotBe31And30}";
            }

            if (string.IsNullOrEmpty(msg))
            {
                if (docDate.Value.Date > DateTime.Now.Date)
                    toastrResult.Data = true;
                else
                    toastrResult.Data = false;
            }
            else
                toastrResult = ToastrResult<bool>.Error(Captions.Error, msg, false);
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.NewDocument)]
        [HttpPost]
        public IActionResult ValidateSecondStep(CreateDocumentSecondStepViewModel model)
        {
            //ToastrResult<bool> toastrResult = new();// ToastrResult<bool>.Warning(Captions.Warning, string.Empty, false);
            ToastrResult toastrResult = ToastrResult<bool>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted);

            //DateTime? birthDate = null;
            //if (!string.IsNullOrEmpty(model.BirthDate))
            //{
            //    birthDate = DateTimeTools.ParsePersianToGorgian(model.BirthDate);
            //    if (birthDate is null)
            //        ModelState.AddModelError(nameof(model.BirthDate), string.Format(ValidationMessages.InvalidFormat, Captions.BirthDate));
            //    if (birthDate >= DateTime.Now)
            //        ModelState.AddModelError(nameof(model.BirthDate), string.Format(ValidationMessages.NotMoreThan, Captions.BirthDate, Captions.Today));
            //}
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<bool>(ToastrType.Error, false, UserMessages.FormNotValid, false);
            }

            return Json(toastrResult);
        }
        #endregion


        #region لیست اسناد
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        public IActionResult Index()
        {
            //ViewBag.InstallmentSumAmount();
            return View();
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> GetPaymentImage(string? imageName)
        {
            try
            {
                string paymentImagesPath = $"{_filePathAddress.PaymentImages}/{imageName}";
                var fileBytes = await _fileService.GetFileBytesAsync(paymentImagesPath);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(paymentImagesPath);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> GetCollateralImage(string? imageName)
        {
            try
            {
                string paymentImagesPath = $"{_filePathAddress.CollateralsDocs}/{imageName}";
                var fileBytes = await _fileService.GetFileBytesAsync(paymentImagesPath);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(paymentImagesPath);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> Detail(long id, CancellationToken cancellationToken)
        {
            CommandResult<DocumentDetailModel> docDetailResult = await _customerService.GetDocumentDetailAsync(id, cancellationToken);
            if (!docDetailResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(docDetailResult.Data);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> GetInstallmentList(long documentId, CancellationToken cancellationToken)
        {
            CommandResult<List<InstallmentModel>> instllmentListResult = await _customerService.GetInstallmentsAsync(documentId, cancellationToken);
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            if (instllmentListResult.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_InstallmentList", instllmentListResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, instllmentListResult.Message, html);
            }
            else
            {
                //html = await _renderService.RenderViewToStringAsync("_InstallmentList", instllmentListResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, instllmentListResult.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> GetInstallmentInfo(long documentId, CancellationToken cancellationToken)
        {
            var instllmentInfoResult = await _customerService.GetInstallmentsInfoAsync(documentId, cancellationToken);
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            if (instllmentInfoResult.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_InstallmentInfo", instllmentInfoResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, instllmentInfoResult.Message, html);
            }
            else
            {
                //html = await _renderService.RenderViewToStringAsync("_InstallmentList", instllmentListResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, instllmentInfoResult.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> GetCollateralInfo(long documentId, CancellationToken cancellationToken)
        {
            var instllmentInfoResult = await _customerService.GetCollateralInfoAsync(documentId, cancellationToken);
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            if (instllmentInfoResult.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_CollateralInfo", instllmentInfoResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, instllmentInfoResult.Message, html);
            }
            else
            {
                //html = await _renderService.RenderViewToStringAsync("_InstallmentList", instllmentListResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, instllmentInfoResult.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> PaymentDetail(long installmentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            InstallmentPymentsDetailModel model = new();
            CommandResult<InstallmentPymentsDetailModel> paymentDetailResult = await _customerService.GetInstallmentPaymentsAsync(installmentId, cancellationToken);
            string html = string.Empty;

            if (paymentDetailResult.IsSuccess)
            {
                model = paymentDetailResult.Data;
                html = await _renderService.RenderViewToStringAsync("_PaymentDetail", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, paymentDetailResult.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync("_PaymentDetail", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, paymentDetailResult.Message, html);
            }
            return Json(toastrResult);
        }

        #region پرداخت
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> CreateOrEditPayment(long installmentId, long paymentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<bool> installmentIsPayResult = await _customerService.IsPayInstallmentAsync(installmentId, cancellationToken);
            CommandResult<bool> installmentIsUnPayedResult = await _customerService.IsFirstUnPayedInstallmentAsync(installmentId, cancellationToken);
            string html = string.Empty;
            if (installmentIsPayResult.IsSuccess)
            {
                if (installmentIsPayResult.Data)
                {
                    toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.TheInstallmentIsPayed, html);
                }
                else
                {
                    if (installmentIsUnPayedResult.IsSuccess)
                    {
                        CommandResult<InstallmentDetailModel> installmentDetailModelResult = await _customerService.GetInstallmentDetailAsync(installmentId);
                        CommandResult<long> sumOfRemainAmountResult = await _customerService.GetSumOfRemainAmountToInstallment(installmentId, 0, cancellationToken);

                        CreateOrEditPaymentViewModel model = new()
                        {
                            InstallmentId = installmentId,
                            PaymentId = 0,
                            DocumentNumber = installmentDetailModelResult.Data.DocumentNumber,
                            InstallmentAmount = installmentDetailModelResult.Data.InstallmentAmount,
                            InstallmentCount = installmentDetailModelResult.Data.InstallmentCount,
                            InstallmentNumber = installmentDetailModelResult.Data.InstallmentNumber,
                            InstallmentDate = installmentDetailModelResult.Data.InstallmentDate,
                            IsPayInstallment = installmentDetailModelResult.Data.IsPayInstallment,
                            SumOfRemainAmount = sumOfRemainAmountResult.Data,
                            //PaymentTypes = _customerService.GetPamentTypesSelectListItem(null),
                            Payments = _customerService.GetPaymentsByInstallmentIdAsync(installmentId, cancellationToken).Result.Data,
                        };
                        if (installmentId > 0 & paymentId > 0)
                        {
                            //edit
                            CommandResult<CreateOrEditPaymentViewModel> paymentResult = await _customerService.GetPaymentForEditAsync(paymentId, installmentId, cancellationToken);
                            if (paymentResult.IsSuccess)
                            {
                                html = await _renderService.RenderViewToStringAsync("_CreateOrEditPayment", paymentResult.Data, this.ControllerContext);
                                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
                            }
                            else
                            {
                                html = await _renderService.RenderViewToStringAsync("_CreateOrEditPayment", model, this.ControllerContext);
                                toastrResult = ToastrResult<string>.Error(Captions.Error, paymentResult.Message, html);
                            }
                        }
                        else
                        {
                            //CommandResult<CalculationInstallmentDelayModel> result = await _customerService.CalculationInstallmentDelayInfoAsync(installmentId, paymentId, model.InstallmentDate.GeorgianToPersian(ShowMode.OnlyDate), cancellationToken);
                            //model.DelayDay = result.Data.CurrentInstallmentDelayDay;
                            //add
                            html = await _renderService.RenderViewToStringAsync("_CreateOrEditPayment", model, this.ControllerContext);
                            toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
                        }
                    }
                    else
                    {
                        toastrResult = ToastrResult<string>.Error(Captions.Error, installmentIsUnPayedResult.Message, html);
                    }
                }
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, installmentIsPayResult.Message, html);
            }

            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpPost]
        public async Task<IActionResult> CreateOrEditPayment(CreateOrEditPaymentViewModel model, CancellationToken cancellationToken)
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
                model.LogUserActivity = new LogUserActivityModel()
                {
                    AdminMenuId = (byte)ManagerPermission.DocumentsList,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                CommandResult<CreateOrEditPaymentViewModel> updateResult = await _customerService.CreateOrEditPaymentAsync(model, cancellationToken);
                if (updateResult.IsSuccess)
                {
                    model.Payments ??= Enumerable.Empty<PaymentModel>();
                    html = await _renderService.RenderViewToStringAsync("_PaymentList", model.Payments, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, updateResult.Message, html);
                }
                else
                {
                    toastrResult = ToastrResult<string>.Error(Captions.Error, updateResult.Message, html);
                }
            }

            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentDescription(long installmentId, long paymentId, string amount, int delayDay, CancellationToken cancellationToken)
        {
            ToastrResult<PaymentDescriptionWithMessageModel> toastrResult = new ToastrResult<PaymentDescriptionWithMessageModel>();
            if (amount is not null)
            {
                if (long.TryParse(amount.Replace(",", ""), out long newAmount))
                {
                    var result = await _customerService.GeneratePaymentDescriptionWithMessageAsync(installmentId, paymentId, newAmount, delayDay, cancellationToken);
                    if (result.IsSuccess)
                        toastrResult = ToastrResult<PaymentDescriptionWithMessageModel>.Success(Captions.Success, result.Message, result.Data);
                    else
                        toastrResult = ToastrResult<PaymentDescriptionWithMessageModel>.Error(Captions.Error, result.Message, result.Data);
                }
            }

            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePaymentInfo(GeneratePaymentInfoRequestModel model, CancellationToken cancellationToken)
        {
            ToastrResult<GeneratePaymentInfoModel> toastrResult = new ToastrResult<GeneratePaymentInfoModel>();
            GeneratePaymentInfoModel resultData = new();


            if (model.IsCalcWithPaymentDate)
            {
                var result = await _customerService.CalculationInstallmentDelayInfoAsync(model.InstallmentId, model.PaymentId, model.PaymentDate, cancellationToken);
                resultData.DelayDay = result.Data.CurrentInstallmentDelayDay;
                model.DelayDay = result.Data.CurrentInstallmentDelayDay;
            }
            else
            {
                resultData.DelayDay = model.DelayDay;
            }

            if (!string.IsNullOrEmpty(model.PaymentAmount))
            {
                if (long.TryParse(model.PaymentAmount.Replace(",", ""), out long newAmount))
                {
                    var result = await _customerService.GeneratePaymentDescriptionWithMessageAsync(model.InstallmentId, model.PaymentId, newAmount, model.DelayDay, cancellationToken);
                    if (result.IsSuccess)
                    {
                        resultData.Message = result.Data.Message;
                        resultData.Description = result.Data.Description;

                    }
                }
            }

            toastrResult = ToastrResult<GeneratePaymentInfoModel>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, resultData);
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateSettleMessage(long documentId, string settleDate, string deliveryDate, string returnedAmount, string discountAmount, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            if (!string.IsNullOrEmpty(returnedAmount) & !string.IsNullOrEmpty(discountAmount))
            {
                if (long.TryParse(returnedAmount.Replace(",", ""), out long newReturnedAmount) & long.TryParse(discountAmount.Replace(",", ""), out long newDiscountAmount))
                {
                    CommandResult<SettleDocumentMessageModel> result = await _customerService.GenerateSettleDocumentMessageAsync(documentId, settleDate, deliveryDate, newReturnedAmount, newDiscountAmount, cancellationToken);
                    if (result.IsSuccess)
                        toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, result.Data.Message);
                    else
                        toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, result.Data.Message);
                }
            }

            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> CalculationInstallmentDelay(long installmentId, string paymentDate, long selectedPaymentId, CancellationToken cancellationToken)
        {
            CommandResult<CalculationInstallmentDelayModel> result = await _customerService.CalculationInstallmentDelayInfoAsync(installmentId, selectedPaymentId, paymentDate, cancellationToken);
            return Json(result.Data);
        }


        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> RemovePayment(long installmentId, long paymentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<bool> installmentIsPayResult = await _customerService.IsPayInstallmentAsync(installmentId, cancellationToken);
            string html = string.Empty;
            if (installmentIsPayResult.IsSuccess)
            {
                if (installmentIsPayResult.Data)
                {
                    //instllment is payed and can not be change it
                    toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.ItIsNotPossibleToPerformThisOperation, html);
                }
                else
                {
                    var logModel = new LogUserActivityModel
                    {
                        AdminMenuId = (byte)ManagerPermission.DocumentsList,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };

                    CommandResult<bool> deletePymentResult = await _customerService.RemoveInstallmentPayment(installmentId, paymentId, logModel, cancellationToken);
                    if (deletePymentResult.IsSuccess)
                    {
                        if (deletePymentResult.Data)
                        {
                            ModelState.Clear();
                            CommandResult<InstallmentDetailModel> installmentDetailModelResult = await _customerService.GetInstallmentDetailAsync(installmentId);
                            CommandResult<long> sumOfRemainAmountResult = await _customerService.GetSumOfRemainAmountToInstallment(installmentId, 0, cancellationToken);
                            CreateOrEditPaymentViewModel model = new()
                            {
                                InstallmentId = installmentId,
                                PaymentId = 0,
                                DocumentNumber = installmentDetailModelResult.Data.DocumentNumber,
                                InstallmentAmount = installmentDetailModelResult.Data.InstallmentAmount,
                                InstallmentCount = installmentDetailModelResult.Data.InstallmentCount,
                                InstallmentNumber = installmentDetailModelResult.Data.InstallmentNumber,
                                InstallmentDate = installmentDetailModelResult.Data.InstallmentDate,
                                IsPayInstallment = installmentDetailModelResult.Data.IsPayInstallment,
                                SumOfRemainAmount = sumOfRemainAmountResult.Data,
                                // PaymentTypes = _customerService.GetPamentTypesSelectListItem(null),
                                Payments = _customerService.GetPaymentsByInstallmentIdAsync(installmentId, cancellationToken).Result.Data,
                            };

                            html = await _renderService.RenderViewToStringAsync("_CreateOrEditPayment", model, this.ControllerContext);
                            toastrResult = ToastrResult<string>.Success(Captions.Success, deletePymentResult.Message, html);
                        }
                        else
                        {
                            toastrResult = ToastrResult<string>.Error(Captions.Error, deletePymentResult.Message, html);
                        }
                    }
                    else
                    {
                        toastrResult = ToastrResult<string>.Error(Captions.Error, deletePymentResult.Message, html);
                    }
                }
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, installmentIsPayResult.Message, html);
            }
            return Json(toastrResult);
        }
        #endregion

        #region لغو پرداخت
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> UnPaymentInstallment(long installmentId, long documentId, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            var logModel = new LogUserActivityModel
            {
                AdminMenuId = (byte)ManagerPermission.DocumentsList,
                UserId = HttpContext.User.GetUserIdAsInt()
            };
            CommandResult result = await _customerService.UnPaymentDocumentInstallment(installmentId, documentId, logModel, cancellationToken);
            if (result.IsSuccess)
            {
                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }
            return Json(toastrResult);
        }
        #endregion

        #region ویرایش توضیحات مدیر
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> EditDocumentDescription(long id, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<string> documentDescriptionResult = await _customerService.GetDocumentDescriptionAsync(id, cancellationToken);
            string html = string.Empty;
            EditDocumentDescriptionViewModel model = new();
            if (documentDescriptionResult.IsSuccess)
            {
                model.Id = id;
                model.Description = documentDescriptionResult.Data;
                html = await _renderService.RenderViewToStringAsync("_EditDocumentDescription", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, documentDescriptionResult.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync("_EditDocumentDescription", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, documentDescriptionResult.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpPost]
        public async Task<IActionResult> EditDocumentDescription(EditDocumentDescriptionViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, string.Empty);
            }
            else
            {
                //model is valid
                var updateResult = await _customerService.UpdateDocumentAdminDescriptionAsync(model, cancellationToken);
                if (updateResult.IsSuccess)
                {
                    #region log
                    CommandResult<string> customerNameResult = await _customerService.GetFullNameByDocumentIdAsync(model.Id, cancellationToken);
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", updateResult.Data.DocumentNo.ToString());
                    logParams.Add("1", customerNameResult.Data);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Edit,
                        DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit_DocumentDescription,
                        AdminMenuId = (byte)ManagerPermission.DocumentsList,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    #endregion

                    toastrResult = ToastrResult<string>.Success(Captions.Success, updateResult.Message, string.IsNullOrEmpty(model.Description) ? string.Empty : model.Description);
                }
                else
                    toastrResult = ToastrResult<string>.Error(Captions.Error, updateResult.Message, string.Empty);
            }

            return Json(toastrResult);
        }

        #endregion

        #region تسویه سند
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> SettleDocument(long documentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CommandResult<SettleDocumentViewModel> result = await _customerService.GetDocumentForSettleAsync(documentId, cancellationToken);
            if (result.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_SettleDocument", result.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                // html = await _renderService.RenderViewToStringAsync("_SettleDocument", result.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpPost]
        public async Task<IActionResult> SettleDocument(SettleDocumentViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, Captions.Warning);
            }
            else
            {
                var settleResult = await _customerService.SettleDocumentAsync(model, cancellationToken);
                if (settleResult.IsSuccess)
                {
                    #region log
                    CommandResult<string> customerNameResult = await _customerService.GetFullNameByDocumentIdAsync(model.DocumentId, cancellationToken);
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", settleResult.Data.ToString());
                    logParams.Add("1", customerNameResult.Data);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Edit,
                        DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit_DocumentCleared,
                        AdminMenuId = (byte)ManagerPermission.DocumentsList,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    #endregion
                    toastrResult = ToastrResult.Success(Captions.Success, settleResult.Message);
                }
                else
                {
                    toastrResult = ToastrResult.Error(Captions.Error, settleResult.Message);
                }
            }
            return Json(toastrResult);
        }
        #endregion

        #region لغو تسویه سند
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> UnSettleDocument(long documentId, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            var result = await _customerService.UnSettleDocumentAsync(documentId, cancellationToken);
            if (result.IsSuccess)
            {
                #region log
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByDocumentIdAsync(documentId, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data.ToString());
                logParams.Add("1", customerNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit_ChangeToActive,
                    AdminMenuId = (byte)ManagerPermission.DocumentsList,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                #endregion

                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }
            return Json(toastrResult);
        }
        #endregion

        #region ویرایش سند سند
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> EditDocument(long documentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CommandResult<EditDocumentViewModel> result = await _customerService.GetDocumentForEditAsync(documentId, cancellationToken);
            if (result.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_EditDocumentInfo", result.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpPost]
        public async Task<IActionResult> EditDocument(EditDocumentViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult<string>();
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            }
            else
            {
                CommandResult<EditDocumentViewModel> result = await _customerService.EditDocumentInfoAsync(model, cancellationToken);
                if (result.IsSuccess)
                {
                    #region log
                    CommandResult<string> customerNameResult = await _customerService.GetFullNameByDocumentIdAsync(model.DocumentId, cancellationToken);
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", result.Data.DocumentNo.ToString());
                    logParams.Add("1", customerNameResult.Data);
                    logParams.Add("2", result.Data.OldDocumentNo.Value.ToString());
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Edit,
                        DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit,
                        AdminMenuId = (byte)ManagerPermission.DocumentsList,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                    await _userService.LogUserActivityAsync(logModel, cancellationToken);
                    #endregion
                    toastrResult = ToastrResult.Success(Captions.Success, result.Message);
                }
                else
                {
                    toastrResult = ToastrResult.Error(Captions.Error, result.Message);
                }
            }

            return Json(toastrResult);
        }
        #endregion

        #region حذف سند
        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> RemoveDocument(long id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            var result = await _customerService.RemoveDocumentAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                #region log
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByDocumentIdAsync(id, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data.ToString());
                logParams.Add("1", customerNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Delete,
                    AdminMenuId = (byte)ManagerPermission.DocumentsList,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                #endregion
                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }
            return Json(toastrResult);
        }
        #endregion

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        public async Task<IActionResult> DocumentList_Read([DataSourceRequest] DataSourceRequest request, SearchDocumentViewModel model)
        {
            CommandResult<IQueryable<DocumentModel>> result = _customerService.GetDocumentListAsQuerable(model);

            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
            {
                dataSource = result.Data.ToDataSourceResult(request);

                var filterdList = dataSource.Data.Cast<DocumentModel>().ToList();
                foreach (var item in filterdList)
                {
                    if (item.DocStatus == DocumentStatus.Active)
                    {
                        var sumOfRemainAmountResult = await _customerService.GetDocumentSumOfAmount(item.Id, default);
                        if (sumOfRemainAmountResult.IsSuccess)
                        {
                            item.SumOfRemainAmount = Math.Abs(sumOfRemainAmountResult.Data);
                            item.SumOfRemainAmountStatus = (sumOfRemainAmountResult.Data < 0 ? Captions.Creditor : string.Empty);
                           
                        }

                    }
                }
                dataSource.Data = filterdList;
            }
            return Json(dataSource);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpPost]
        public async Task<IActionResult> InstallmentAmountSum(SearchDocumentViewModel model, CancellationToken cancellationToken)
        {
            long sum = 0;
            CommandResult<long> result = await _customerService.InstallmentAmountSumAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                sum = result.Data;
            }
            return Json($"{sum.ToString("N0")} {Captions.Tooman}");
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpPost]
        public async Task<IActionResult> RemainAmountSum(SearchDocumentViewModel model, CancellationToken cancellationToken)
        {
            long sum = 0;
            string sumOfRemainAmountStatus = string.Empty;
            CommandResult<long> result = await _customerService.RemainAmountSumAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                sum = result.Data;
                if (sum < 0)
                    sumOfRemainAmountStatus = Captions.Creditor;
            }
            return Json($"{Math.Abs(result.Data).ToString("N0")} {Captions.Tooman} {sumOfRemainAmountStatus}");
        }

        #region جزئیات ضمانت

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> CollateralDetail(long documentId, long collateralId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CreateOrEditCollateralViewModel model = new();
            var documentStatusResult = await _customerService.GetDocumentStatusByIdAsync(documentId, cancellationToken);
            if (documentStatusResult.IsSuccess)
            {
                if (documentStatusResult.Data == DocumentStatus.Active)
                {
                    var collaterals = _customerService.GetCollaterals(documentId);
                    if (collaterals.IsSuccess)
                        model.Collaterals = collaterals.Data;
                    var collateralTypes = await _customerService.GetCollateralsTypeSelectListItemsAsync(0, cancellationToken);
                    if (collateralTypes.IsSuccess)
                        model.CollateralTypes = collateralTypes.Data;
                    model.DocumentId = documentId;
                    model.CollateralId = 0;

                    html = await _renderService.RenderViewToStringAsync("_CreateOrEditCollateral", model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
                }
                else
                {
                    toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.OnlyCanEditCollateralDetailsForActiveDocument, html);
                }
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, documentStatusResult.Message, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<IActionResult> CreateOrEditCollateral(long id, long documentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CreateOrEditCollateralViewModel model = new();
            string html = string.Empty;
            if (documentId > 0 & id > 0)
            {
                model.DocumentId = documentId;
                model.CollateralId = id;

                //edit
                CommandResult<CreateOrEditCollateralViewModel> collateralResult = await _customerService.GetCollateralForEditAsync(documentId, id, cancellationToken);
                if (collateralResult.IsSuccess)
                {
                    //collateral is founded 
                    collateralResult.Data.CollateralTypes = _customerService.GetCollateralTypesAsync(collateralResult.Data.CollateralTypeId.Value, cancellationToken).Result.Data;
                    html = await _renderService.RenderViewToStringAsync("_CreateOrEditCollateral", collateralResult.Data, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, collateralResult.Message, html);
                }
                else
                {
                    //collateral is not founded 
                    html = await _renderService.RenderViewToStringAsync("_CreateOrEditCollateral", model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Error(Captions.Error, collateralResult.Message, html);
                }
            }
            else
            {
                //add
                model.DocumentId = documentId;
                model.CollateralId = 0;
                model.CollateralTypes = _customerService.GetCollateralTypesAsync(0, cancellationToken).Result.Data;
                html = await _renderService.RenderViewToStringAsync("_CreateOrEditCollateral", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpPost]
        public async Task<IActionResult> CreateOrEditCollateral(CreateOrEditCollateralViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            if (!ModelState.IsValid)
            {
                model.ImageUrl = string.Empty;
                //model.CollateralTypes = _customerService.GetCollateralsTypeSelectListItemsAsync(0, cancellationToken).Result.Data;
                //model.Collaterals = _customerService.GetCollateralsAsync(model.DocumentId, cancellationToken).Result.Data;
                //html = await _renderService.RenderViewToStringAsync("_CreateOrEditCollateral", model, this.ControllerContext);
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html);
            }
            else
            {
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByDocumentIdAsync(model.DocumentId, cancellationToken);

                if (model.CollateralId > 0 & model.DocumentId > 0)
                {
                    //edit
                    CommandResult<CreateOrEditCollateralViewModel> updateResult = await _customerService.UpdateCollateralAsync(model, cancellationToken);
                    if (updateResult.IsSuccess)
                    {
                        var collateralList = _customerService.GetCollaterals(model.DocumentId);
                        html = await _renderService.RenderViewToStringAsync("_CollateralList", collateralList.Data, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Success(Captions.Success, updateResult.Message, html);

                        #region log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", updateResult.Data.DocumentNo.ToString());
                        logParams.Add("1", customerNameResult.Data);
                        if (!string.IsNullOrEmpty(model.CollateralTypeTitle))
                            logParams.Add("2", model.CollateralTypeTitle);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Edit_Collateral,
                            AdminMenuId = (byte)ManagerPermission.DocumentsList,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                        #endregion


                    }
                    else
                    {
                        toastrResult = ToastrResult<string>.Error(Captions.Error, updateResult.Message, html);
                    }
                }
                else
                {
                    //add
                    CommandResult<CreateOrEditCollateralViewModel> addResult = await _customerService.AddCollateralAsync(model, cancellationToken);
                    if (addResult.IsSuccess)
                    {
                        var collateralList = _customerService.GetCollaterals(model.DocumentId);
                        html = await _renderService.RenderViewToStringAsync("_CollateralList", collateralList.Data, this.ControllerContext);

                        #region log
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", addResult.Data.DocumentNo.ToString());
                        logParams.Add("1", customerNameResult.Data);
                        if (!string.IsNullOrEmpty(model.CollateralTypeTitle))
                            logParams.Add("2", model.CollateralTypeTitle);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Insert_Collateral,
                            AdminMenuId = (byte)ManagerPermission.DocumentsList,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                        #endregion


                        toastrResult = ToastrResult<string>.Success(Captions.Success, addResult.Message, html);
                    }
                    else
                    {
                        toastrResult = ToastrResult<string>.Error(Captions.Error, addResult.Message, html);
                    }
                }
            }

            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        [HttpGet]
        public async Task<JsonResult> RemoveCollateral(long id, long documentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CommandResult<string> result = await _customerService.RemoveCollateralAsync(id, documentId, cancellationToken);
            if (result.IsSuccess)
            {
                List<CollateralModel> collaterals = new List<CollateralModel>();
                var listResult = _customerService.GetCollaterals(documentId);
                if (listResult.IsSuccess)
                    collaterals = listResult.Data;

                #region log
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByDocumentIdAsync(documentId, cancellationToken);
                CommandResult<int> documentNumberResult = await _customerService.GetDocumentNumberByIdAsync(documentId, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", documentNumberResult.Data.ToString());
                logParams.Add("1", customerNameResult.Data);
                logParams.Add("2", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    DescriptionPattern = AdminActivityLogDescriptions.DocumentList_Delete_Collateral,
                    AdminMenuId = (byte)ManagerPermission.DocumentsList,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                #endregion

                html = await _renderService.RenderViewToStringAsync("_CollateralList", collaterals, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }
        #endregion

        #region export data

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        public JsonResult InitReportData(SearchDocumentViewModel model)
        {

            TempData.Set<SearchDocumentViewModel>("InitDocumentsReportData", model);
            return Json(true);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        public async Task<IActionResult> ExportPdf()
        {
            SearchDocumentViewModel model = new SearchDocumentViewModel();
            List<DocumentModel> reportData = new List<DocumentModel>();
            if (TempData.Peek("InitDocumentsReportData") != null)
                model = TempData.Get<SearchDocumentViewModel>("InitDocumentsReportData");

            CommandResult<IQueryable<DocumentModel>> result = _customerService.GetDocumentListAsQuerable(model);

            if (result.IsSuccess)
            {

                foreach (var item in result.Data.OrderBy(x => x.FullName).ToList())
                {
                    if (item.DocStatus == DocumentStatus.Active)
                    {
                        var sumOfRemainAmountResult = await _customerService.GetDocumentSumOfAmount(item.Id, default);
                        if (sumOfRemainAmountResult.IsSuccess)
                        {
                            item.SumOfRemainAmount = Math.Abs(sumOfRemainAmountResult.Data);
                            item.SumOfRemainAmountStatus = (sumOfRemainAmountResult.Data < 0 ? Captions.Creditor : string.Empty);
                        }

                    }
                    reportData.Add(item);
                }
                // reportData = result.Data.ToList();
                TempData.Set<SearchDocumentViewModel>("InitDocumentsReportData", model);
            }

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"Documents-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.DocumentsList)]
        public async Task<IActionResult> ExportExcel()
        {
            SearchDocumentViewModel model = new SearchDocumentViewModel();
            List<DocumentModel> reportData = new List<DocumentModel>();
            if (TempData.Peek("InitDocumentsReportData") != null)
                model = TempData.Get<SearchDocumentViewModel>("InitDocumentsReportData");

            CommandResult<IQueryable<DocumentModel>> result = _customerService.GetDocumentListAsQuerable(model);

            if (result.IsSuccess)
            {
                foreach (var item in result.Data.OrderBy(x => x.FullName).ToList())
                {
                    if (item.DocStatus == DocumentStatus.Active)
                    {
                        var sumOfRemainAmountResult = await _customerService.GetDocumentSumOfAmount(item.Id, default);
                        if (sumOfRemainAmountResult.IsSuccess)
                        {
                            item.SumOfRemainAmount = Math.Abs(sumOfRemainAmountResult.Data);
                            item.SumOfRemainAmountStatus = (sumOfRemainAmountResult.Data < 0 ? Captions.Creditor : string.Empty);
                        }

                    }
                    reportData.Add(item);
                }
                // reportData = result.Data.ToList();
                TempData.Set<SearchDocumentViewModel>("InitDocumentsReportData", model);
            }


            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.FullName);
            reportList.Columns.Add(Captions.DocumentNumber);
            reportList.Columns.Add(Captions.Collateral);
            reportList.Columns.Add(Captions.DocumentDate);
            reportList.Columns.Add(Captions.InstallmentAmount);
            reportList.Columns.Add(Captions.RemainInstallmentCount);
            reportList.Columns.Add(Captions.SumOfRemainAmount);
            reportList.Columns.Add(Captions.Gallery);
            reportList.Columns.Add(Captions.Description);
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
            ColumnWidth[9] = 25;
            ColumnWidth[10] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.FullName,
                   item.DocumentNo,
                   item.CollateralListInfo.Replace("<br/>", "\n"),
                   item.PersianDocumentDate,
                   item.InstallmentAmount,
                   item.RemainInstallmentCount,
                   $"{item.SumOfRemainAmount.ToString("N0")}{item.SumOfRemainAmountStatus}",
                   item.Gallery,
                   item.AdminDescription,
                   item.Status);
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, Captions.PaymentReport, true);
            string filename = string.Format("{0}.xlsx", $"Documents" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion

        #endregion
    }
}
