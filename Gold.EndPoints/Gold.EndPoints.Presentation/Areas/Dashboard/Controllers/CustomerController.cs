using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.ReportModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ReportViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Tools;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Collections.Generic;
using System.Data;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Customers)]
    public class CustomerController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly ICustomerService _customerService;
        private readonly IProvinceService _provinceService;
        private readonly IUserService _userService;
        private readonly ILogManager _logManager;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;

        public CustomerController(IViewRenderService renderService,
            ICustomerService customerService,
            IProvinceService provinceService,
            IUserService userService,
            ILogManager logManager,
               IOptions<FilePathAddress> filePathAddressOptions,
            IFileService fileService)
        {
            this._renderService = renderService;
            this._customerService = customerService;
            _provinceService = provinceService;
            this._userService = userService;
            _logManager = logManager;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            try
            {
                var provinceListResult = await _provinceService.GetAllProvincesSelectListItemAsync(0, cancellationToken);
                if (provinceListResult.IsSuccess)
                {
                    ViewBag.Proviances = provinceListResult.Data.ToList();
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
            }
            return View();
        }

        #region Create Or Edit Customer
        [HttpGet]
        public async Task<JsonResult> CreateOrEditCustomer(int id, CancellationToken cancellationToken)
        {
            var provinceListResult = await _provinceService.GetAllProvincesSelectListItemAsync(0, cancellationToken);
            CreateOrEditCustomerViewModel model = new()
            {
                Id = 0,
                IsActive = true,
                Proviances = provinceListResult.Data.ToList()
            };
            string html = string.Empty;
            ToastrResult<string> toastrResult = new ToastrResult<string>();

            if (provinceListResult.IsSuccess)
                if (id == 0)
                {
                    html = await _renderService.RenderViewToStringAsync("_CreateOrEditCustomer", model, this.ControllerContext, false);
                    //empty model
                    toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
                }
                else
                {
                    //edit model
                    CommandResult<CreateOrEditCustomerViewModel> result = await _customerService.GetCustomerInfoForEditAsync(id, cancellationToken);

                    if (result.IsSuccess)
                    {

                        SelectListItem selectedProvince = new SelectListItem();
                        SelectListItem selectedCity = new SelectListItem();
                        CommandResult<List<SelectListItem>> proviancesResult = new CommandResult<List<SelectListItem>>();
                        if (result.Data.CityId.HasValue)
                        {
                            proviancesResult = await _provinceService.GetProviancesWithSelectedCityAsync(result.Data.CityId.Value, cancellationToken);
                            result.Data.Proviances = proviancesResult.Data;
                            foreach (var item in result.Data.Proviances)
                            {
                                if (item.Selected)
                                {
                                    selectedProvince = item;
                                    break;
                                }
                            }
                            var citiesResult = await _provinceService.GetCitiesOfProvinceSelectListItemAsync(int.Parse(selectedProvince.Value), cancellationToken);
                            if (citiesResult.IsSuccess)
                            {
                                foreach (var item in citiesResult.Data)
                                {
                                    if (int.Parse(item.Value) == result.Data.CityId.Value)
                                    {
                                        item.Selected = true;
                                        break;
                                    }
                                }
                                result.Data.Cities = citiesResult.Data;
                            }
                        }
                        else
                        {
                            proviancesResult = await _provinceService.GetAllProvincesSelectListItemAsync(0, cancellationToken);
                            result.Data.Proviances = proviancesResult.Data;
                        }
                        html = await _renderService.RenderViewToStringAsync("_CreateOrEditCustomer", result.Data, this.ControllerContext, true);
                        toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
                    }
                    else
                    {
                        html = await _renderService.RenderViewToStringAsync("_CreateOrEditCustomer", model, this.ControllerContext, false);
                        toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
                    }
                }
            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrEditCustomer(CreateOrEditCustomerViewModel model, CancellationToken cancellationToken)
        {

            ToastrResult toastrResult = new ToastrResult();

            //string html = string.Empty;
            //check bank card number if in create customer action
            if (!string.IsNullOrEmpty(model.CardNumberOwner) & model.Id <= 0)
            {
                if (string.IsNullOrEmpty(model.CardNumber))
                    ModelState.AddModelError(nameof(model.CardNumber), string.Format(ValidationMessages.Required, Captions.CardNumber));
            }

            if (!string.IsNullOrEmpty(model.CardNumber) & model.Id <= 0)
            {
                if (string.IsNullOrEmpty(model.CardNumberOwner))
                    ModelState.AddModelError(nameof(model.CardNumberOwner), string.Format(ValidationMessages.Required, Captions.CardNumberOwner));
            }

            if (model.CityId.HasValue)
            {
                CommandResult<bool> cityValidationResult = await _provinceService.IsValidCityIdAsync(model.CityId.Value, cancellationToken);
                if (!cityValidationResult.IsSuccess)
                {
                    ModelState.AddModelError(nameof(model.CityId), cityValidationResult.Message);
                }
            }
            if (model.Nationality.HasValue & model.Nationality == NationalityType.Iranian)
            {
                if (model.NationalCode is not null)
                {
                    if (model.NationalCode.Length < 10)
                        if (!NationCode.Check(model.NationalCode))
                            ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.MinLength, Captions.NationalCode_UserName, 10));
                }
                if (model.NationalCode is not null)
                {
                    if (model.NationalCode.Length > 10)
                        if (!NationCode.Check(model.NationalCode))
                            ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.MaxLength, Captions.NationalCode_UserName, 10));
                }
                if (model.NationalCode is not null)
                    if (!NationCode.Check(model.NationalCode))
                        ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.Invalid, Captions.NationalCode_UserName));
            }


            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            }
            else
            {
                if (model.Id == 0)
                {
                    //add new
                    CommandResult<CreateOrEditCustomerViewModel> result = await _customerService.CreateCustomerAsync(model, cancellationToken);
                    if (result.IsSuccess)
                    {
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.FullName);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.Customers,
                            DescriptionPattern = AdminActivityLogDescriptions.Customer_Insert,
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
                }
                else
                {
                    //update
                    CommandResult<CreateOrEditCustomerViewModel> result = await _customerService.UpdateCustomerAsync(model, cancellationToken);
                    if (result.IsSuccess)
                    {
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.FullName);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            AdminMenuId = (byte)ManagerPermission.Customers,
                            DescriptionPattern = AdminActivityLogDescriptions.Customer_Edit,
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
                }
            }

            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<JsonResult> IsDuplicateCustomerMobile(int id, string mobile, CancellationToken cancellationToken = default)
        {
            CommandResult isDuplicateResult = await _customerService.IsExistCustomerByMobileAsync(id, mobile, cancellationToken);
            return Json(isDuplicateResult.IsSuccess);
        }
        #endregion

        #region Remove Customer
        [HttpGet]
        public async Task<IActionResult> RemoveCustomer(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            CommandResult<string> result = await _customerService.RemoveCustomerAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.Customers,
                    DescriptionPattern = AdminActivityLogDescriptions.Customer_Delete,
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
        #endregion

        #region Reset Customer Password
        [HttpGet]
        public async Task<JsonResult> ResetPassword(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult<string>();
            CommandResult<string> result = await _customerService.ResetCustomerPasswordAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    AdminMenuId = (byte)ManagerPermission.Customers,
                    DescriptionPattern = AdminActivityLogDescriptions.Customer_Edit_ResetPassword,
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
        #endregion


        #region Essential Number
        [HttpGet]
        public async Task<JsonResult> AddEssentialNumber(int id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult result = await _customerService.IsExistCustomerByIdAsync(customerId, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
            {
                EssentialTelViewModel model = new();
                var essentialTels = _customerService.GetEssentialTelListAsQuerable();
                if (essentialTels.IsSuccess)
                    model.EssentialTels = essentialTels.Data.Where(x => x.CustomerId == customerId).ToList();

                model.CustomerId = customerId;
                model.EssentialTelId = id;
                html = await _renderService.RenderViewToStringAsync("_EssentialTelForm", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync("_EssentialTelForm", new EssentialTelModel(), this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<JsonResult> EditEssentialTel(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<EssentialTelModel> result = await _customerService.GetEssentialTelAsync(id, customerId, cancellationToken);
            EssentialTelViewModel model = new();
            string viewName = "_EssentialTelForm";
            string html = string.Empty;

            if (result.IsSuccess)
            {
                model.CustomerId = result.Data.CustomerId;
                model.EssentialTelId = result.Data.Id;
                model.RelationShip = result.Data.RelationShip;
                model.Tel = result.Data.Tel;
                var essentialTels = _customerService.GetEssentialTelListAsQuerable();
                if (essentialTels.IsSuccess)
                    model.EssentialTels = essentialTels.Data.Where(x => x.CustomerId == customerId).ToList();
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrEditEssentialNumber(EssentialTelViewModel model, bool inDocument, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var listResult = await _customerService.GetCustomerEssentialTelListAsync(model.CustomerId, cancellationToken);
            if (listResult.IsSuccess)
                model.EssentialTels = listResult.Data;
            string html = string.Empty;
            string viewName = "_EssentialTelForm";
            if (inDocument)
            {
                viewName = "_CreateOrEditCustomerEssentialTel";
            }
            if (!ModelState.IsValid)
            {
                //invalid model
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.FormNotValid, html);
            }
            else
            {
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(model.CustomerId, cancellationToken);

                //valid model
                if (model.EssentialTelId == 0)
                {
                    //add new
                    var addEssentialNumberResult = await _customerService.AddEssentialTelAsync(model, cancellationToken);
                    if (addEssentialNumberResult.IsSuccess)
                    {

                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.Tel);
                        logParams.Add("1", customerNameResult.Data);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.Customers,
                            DescriptionPattern = AdminActivityLogDescriptions.Customer_Insert_EssentialNumber,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        ModelState.Clear();
                        model = new();
                        model.CustomerId = addEssentialNumberResult.Data.CustomerId;
                        model.EssentialTelId = 0;
                        listResult = await _customerService.GetCustomerEssentialTelListAsync(model.CustomerId, cancellationToken);
                        if (listResult.IsSuccess)
                            model.EssentialTels = listResult.Data;
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Success(Captions.Success, addEssentialNumberResult.Message, html);
                    }
                    else
                    {
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Error(Captions.Error, addEssentialNumberResult.Message, html);
                    }
                }
                else
                {
                    //edit
                    var editEssentialNumberResult = await _customerService.EditEssentialTelAsync(model, cancellationToken);
                    if (editEssentialNumberResult.IsSuccess)
                    {

                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.Tel);
                        logParams.Add("1", customerNameResult.Data);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            AdminMenuId = (byte)ManagerPermission.Customers,
                            DescriptionPattern = AdminActivityLogDescriptions.Customer_Edit_EssentialNumber,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        ModelState.Clear();
                        model = new();
                        model.CustomerId = editEssentialNumberResult.Data.CustomerId;
                        model.EssentialTelId = 0;
                        listResult = await _customerService.GetCustomerEssentialTelListAsync(model.CustomerId, cancellationToken);
                        if (listResult.IsSuccess)
                            model.EssentialTels = listResult.Data;
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Success(Captions.Success, editEssentialNumberResult.Message, html);
                    }
                    else
                    {
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Error(Captions.Error, editEssentialNumberResult.Message, html);
                    }
                }
            }
            return Json(toastrResult);
        }

        public async Task<JsonResult> RemoveEssentialTel(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<string> result = await _customerService.RemoveEssentialNumberAsync(id, customerId, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
            {
                List<EssentialTelModel> essentialTels = new List<EssentialTelModel>();
                var listResult = await _customerService.GetCustomerEssentialTelListAsync(customerId, cancellationToken);
                if (listResult.IsSuccess)
                {
                    CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(customerId, cancellationToken);
                    essentialTels = listResult.Data;
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", result.Data);
                    logParams.Add("1", customerNameResult.Data);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Delete,
                        AdminMenuId = (byte)ManagerPermission.Customers,
                        DescriptionPattern = AdminActivityLogDescriptions.Customer_Delete_EssentialNumber,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                }
                html = await _renderService.RenderViewToStringAsync("_EssentialTelList", essentialTels, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

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

        #region Bank Card Number
        [HttpGet]
        public async Task<JsonResult> AddCardNumber(int id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult result = await _customerService.IsExistCustomerByIdAsync(customerId, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
            {
                CardNumberViewModel model = new();
                var bankCardNumbers = _customerService.GetCardNumberListAsQuerable();
                if (bankCardNumbers.IsSuccess)
                    model.CardNumbers = bankCardNumbers.Data.Where(x => x.CustomerId == customerId).ToList();

                model.CustomerId = customerId;
                model.CardNumberId = id;
                html = await _renderService.RenderViewToStringAsync("_CardNumberForm", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync("_CardNumberForm", new EssentialTelModel(), this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<JsonResult> EditCardNumber(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<CardNumberModel> result = await _customerService.GetCardNumberAsync(id, customerId, cancellationToken);
            CardNumberViewModel model = new();
            string viewName = "_CardNumberForm";
            string html = string.Empty;

            if (result.IsSuccess)
            {
                model.CustomerId = result.Data.CustomerId;
                model.CardNumberId = result.Data.Id;
                model.Owner = result.Data.Owner;
                model.CardNumber = result.Data.Number;
                var cardNumbers = _customerService.GetCardNumberListAsQuerable();
                if (cardNumbers.IsSuccess)
                    model.CardNumbers = cardNumbers.Data.Where(x => x.CustomerId == customerId).ToList();
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<JsonResult> AddOrEditCardNumber(CardNumberViewModel model, bool inDocument, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var listResult = await _customerService.GetCustomerCardNumberListAsync(model.CustomerId, cancellationToken);
            if (listResult.IsSuccess)
                model.CardNumbers = listResult.Data;
            string html = string.Empty;
            string viewName = "_CardNumberForm";
            if (inDocument)
            {
                viewName = "_CreateOrEditCustomerCardNumber";
            }
            if (!ModelState.IsValid)
            {
                //invalid model
                html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.FormNotValid, html);
            }
            else
            {
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(model.CustomerId, cancellationToken);

                //valid model
                if (model.CardNumberId == 0)
                {
                    //add new
                    var addCardNumberResult = await _customerService.AddCardNumberAsync(model, cancellationToken);
                    if (addCardNumberResult.IsSuccess)
                    {

                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.CardNumber);
                        logParams.Add("1", customerNameResult.Data);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Insert,
                            AdminMenuId = (byte)ManagerPermission.Customers,
                            DescriptionPattern = AdminActivityLogDescriptions.Customer_Insert_CardNumber,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                        ModelState.Clear();
                        model = new();
                        model.CustomerId = addCardNumberResult.Data.CustomerId;
                        model.CardNumberId = 0;
                        listResult = await _customerService.GetCustomerCardNumberListAsync(model.CustomerId, cancellationToken);
                        if (listResult.IsSuccess)
                            model.CardNumbers = listResult.Data;
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Success(Captions.Success, addCardNumberResult.Message, html);
                    }
                    else
                    {
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Error(Captions.Error, addCardNumberResult.Message, html);
                    }
                }
                else
                {
                    //edit
                    var editCardNumberResult = await _customerService.EditCardNumberAsync(model, cancellationToken);
                    if (editCardNumberResult.IsSuccess)
                    {
                        var logParams = new Dictionary<string, string>();
                        logParams.Add("0", model.CardNumber);
                        logParams.Add("1", customerNameResult.Data);
                        var logModel = new LogUserActivityModel()
                        {
                            ActivityType = AdminActivityType.Edit,
                            AdminMenuId = (byte)ManagerPermission.Customers,
                            DescriptionPattern = AdminActivityLogDescriptions.Customer_Edit_CardNumber,
                            Parameters = logParams,
                            UserId = HttpContext.User.GetUserIdAsInt()
                        };
                        await _userService.LogUserActivityAsync(logModel, cancellationToken);
                        ModelState.Clear();
                        model = new();
                        model.CustomerId = editCardNumberResult.Data.CustomerId;
                        model.CardNumberId = 0;
                        listResult = await _customerService.GetCustomerCardNumberListAsync(model.CustomerId, cancellationToken);
                        if (listResult.IsSuccess)
                            model.CardNumbers = listResult.Data;
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Success(Captions.Success, editCardNumberResult.Message, html);
                    }
                    else
                    {
                        html = await _renderService.RenderViewToStringAsync(viewName, model, this.ControllerContext);
                        toastrResult = ToastrResult<string>.Error(Captions.Error, editCardNumberResult.Message, html);
                    }
                }
            }
            return Json(toastrResult);
        }

        public async Task<JsonResult> RemoveCardNumber(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<string> result = await _customerService.RemoveCardNumberAsync(id, customerId, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
            {
                List<CardNumberModel> essentialTels = new List<CardNumberModel>();
                var listResult = await _customerService.GetCustomerCardNumberListAsync(customerId, cancellationToken);
                CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(customerId, cancellationToken);
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                logParams.Add("1", customerNameResult.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.Customers,
                    DescriptionPattern = AdminActivityLogDescriptions.Customer_Delete_CardNumber,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                html = await _renderService.RenderViewToStringAsync("_CardNumberList", listResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        public async Task<JsonResult> ChangeCardNumberOrder(long id, int customerId, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult result = await _customerService.ChangeCardNumberOrderNumberAsync(id, isUp, cancellationToken);
            List<CardNumberModel> model = new();

            string html = string.Empty;

            if (result.IsSuccess)
            {
                var listResult = await _customerService.GetCustomerCardNumberListAsync(customerId, cancellationToken);
                if (listResult.IsSuccess)
                    model = listResult.Data.ToList();
                html = await _renderService.RenderViewToStringAsync("_CardNumberList", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        #endregion

        #region Profile Image
        public async Task<IActionResult> GetProfileImages(int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CommandResult<List<ProfileImagesModel>> result = await _customerService.GetProfileImagesAsync(customerId, cancellationToken);
            if (result.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_ProfileImages", result.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        public async Task<JsonResult> RemoveProfileImage(long id, int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            CommandResult<string> result = await _customerService.RemoveProfileImageAsync(id, customerId, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
            {
                var listResult = await _customerService.GetProfileImagesAsync(customerId, cancellationToken);
                if (listResult.IsSuccess)
                {
                    CommandResult<string> customerNameResult = await _customerService.GetFullNameByIdAsync(customerId, cancellationToken);
                    var logParams = new Dictionary<string, string>();
                    logParams.Add("0", result.Data);
                    logParams.Add("1", customerNameResult.Data);
                    var logModel = new LogUserActivityModel()
                    {
                        ActivityType = AdminActivityType.Delete,
                        AdminMenuId = (byte)ManagerPermission.Customers,
                        DescriptionPattern = AdminActivityLogDescriptions.Customer_Delete_ProfileImage,
                        Parameters = logParams,
                        UserId = HttpContext.User.GetUserIdAsInt()
                    };
                }
                html = await _renderService.RenderViewToStringAsync("_ProfileImages", listResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileImage(string? imageName)
        {
            try
            {
                string patch = $"{_filePathAddress.CustomerProfileImage}/{imageName}";
                var fileBytes = await _fileService.GetFileBytesAsync(patch);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }
        #endregion

        public async Task<IActionResult> CustomerList_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = _customerService.GetCustomerListAsQuerable();
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
            {
                dataSource = result.Data.ToDataSourceResult(request);
                var list = dataSource.Data.Cast<CustomerModel>().ToList();
                foreach (var item in list)
                {
                    var accountStatusResult = await _customerService.GetCustomerAccountStatus(item.Id, default);
                    item.AccountStatus = accountStatusResult.Data;
                    if (item.AccountStatus == Captions.GoodPay)
                        item.AccountStatusClass = "badge badge-success";
                    else if (item.AccountStatus == Captions.DeadBeat)
                        item.AccountStatusClass = "badge badge-danger";
                }
                dataSource.Data = list;
            }
            return Json(dataSource);
        }

        public async Task<IActionResult> GetAccountStatus(int customerId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var result = await _customerService.GetCustomerAccountStatus(customerId, cancellationToken);
            toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, result.Data);
            return Json(toastrResult);
        }

        #region export data


        public JsonResult InitReportData()
        {

            //TempData.Set<string>("InitCustomersReportData", string.Empty);
            return Json(true);
        }

        public async Task<IActionResult> ExportPdf()
        {
            //string model = string.Empty;

            //if (TempData.Peek("InitCustomersReportData") != null)
            //    model = TempData.Get<string>("InitCustomersReportData");

            List<CustomerReportModel> reportData = new List<CustomerReportModel>();
            CommandResult<IQueryable<CustomerReportModel>> result = _customerService.GetCustomerReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.FullName).ToList();
            //foreach (var item in result.Data)
            //{
            //    var accountStatusResult = await _customerService.GetCustomerAccountStatus(item.Id, default);
            //    item.AccountStatus = accountStatusResult.Data;
            //}
            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"CustomersReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public async Task<IActionResult> ExportExcel()
        {
            //string model = string.Empty;

            // if (TempData.Peek("InitCustomersReportData") != null)
            //    model = TempData.Get<PaymentReportSearchViewModel>("InitCustomersReportData");

            List<CustomerReportModel> reportData = new List<CustomerReportModel>();
            CommandResult<IQueryable<CustomerReportModel>> result = _customerService.GetCustomerReportListAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.FullName).ToList();
            //foreach (var item in reportData)
            //{
            //    var accountStatusResult = await _customerService.GetCustomerAccountStatus(item.Id, default);
            //    item.AccountStatus = accountStatusResult.Data;
            //}

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Gender);
            reportList.Columns.Add(Captions.FullName);
            reportList.Columns.Add(Captions.Nationality);
            reportList.Columns.Add(Captions.BirthDate);
            reportList.Columns.Add(Captions.NationalCode);
            reportList.Columns.Add(Captions.Mobile);
            reportList.Columns.Add(Captions.FatherName);
            reportList.Columns.Add(Captions.JobTitle);
            reportList.Columns.Add(Captions.SanaCode);
            reportList.Columns.Add(Captions.EssentialTel);
            reportList.Columns.Add(Captions.RelationShipOfEssentialTel);
            reportList.Columns.Add(Captions.BankCardNumber);
            reportList.Columns.Add(Captions.CardNumberOwner);
            reportList.Columns.Add(Captions.City);
            reportList.Columns.Add(Captions.Address);
            reportList.Columns.Add(Captions.PostalCode);
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
            ColumnWidth[11] = 25;
            ColumnWidth[12] = 25;
            ColumnWidth[13] = 25;
            ColumnWidth[14] = 25;
            ColumnWidth[15] = 25;
            ColumnWidth[16] = 25;
            ColumnWidth[17] = 25;

            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.Gender,
                   item.FullName,
                   item.Nationaity,
                   item.BirthDate,
                   item.NationalCode,
                   item.Mobile,
                   item.FatherName,
                   item.JobTitle,
                   item.SanaCode,
                   item.EssentialTel,
                   item.RelationShip,
                   item.BankCardNo,
                   item.BankCardOwnerName,
                   item.CityName,
                   item.Address,
                   item.PostalCode,
                   (item.IsActive ? Captions.Active : Captions.DeActive)
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.Customers}", true);
            string filename = string.Format("{0}.xlsx", $"CustomersReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion


        #region Search in Customers by phone or bank card number
        [HttpGet]
        public async Task<IActionResult> SearchInCustomers(CustomerSearchType searchType)
        {
            ToastrResult<string> toastResult = new();
            string html = string.Empty;
            switch (searchType)
            {
                case CustomerSearchType.ByPhoneNumber:
                    html = await _renderService.RenderViewToStringAsync("_SearchByPhoneNumberForm", new SearchCustomerByPhoneNumberViewModel() { SearchType = CustomerSearchType.ByPhoneNumber }, this.ControllerContext);
                    break;
                case CustomerSearchType.ByBankCardNumber:
                    html = await _renderService.RenderViewToStringAsync("_SearchByBankCardNumberForm", new SearchCustomerByBankCardNumberViewModel() { SearchType = CustomerSearchType.ByBankCardNumber }, this.ControllerContext);
                    break;
                default:
                    html = await _renderService.RenderViewToStringAsync("_SearchByPhoneNumberForm", new SearchCustomerByPhoneNumberViewModel() { SearchType = CustomerSearchType.ByPhoneNumber }, this.ControllerContext);
                    break;
            }
            if (string.IsNullOrEmpty(html))
                toastResult = ToastrResult<string>.Warning(Captions.Warning, OperationResultMessage.OperationIsFailure, html);
            else
                toastResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            return Json(toastResult);
        }

        public IActionResult CustomerByPhoneNumberList_Read([DataSourceRequest] DataSourceRequest request, string? PhoneNumber)
        {
            DataSourceResult dataSource = new DataSourceResult();
            if (string.IsNullOrEmpty(PhoneNumber))
                return Json(dataSource);
            if (PhoneNumber.Length < 4)
                return Json(dataSource);
            PhoneNumber = PhoneNumber.ToEnglishNumberByCultureInfo();
            var result = _customerService.GetCustomerPhoneNumberListAsQuerable(PhoneNumber);
            if (result.IsSuccess)
                dataSource = result.Data.ToDataSourceResult(request);
            return Json(dataSource);
        }
        #region export CustomerPhoneNumberList data


        public JsonResult InitSearchByPhoneNumberReportData(string? PhoneNumber)
        {
            PhoneNumber ??= string.Empty;
            TempData.Set<string>("InitCustomerPhoneNumberListReportData", PhoneNumber);
            return Json(true);
        }

        public IActionResult SearchByPhoneNumberExportPdf()
        {
            List<CustomerPhoneNumberModel> reportData = new List<CustomerPhoneNumberModel>();
            string PhoneNumber = string.Empty;
            if (TempData.Peek("InitCustomerPhoneNumberListReportData") != null)
                PhoneNumber = TempData.Get<string>("InitCustomerPhoneNumberListReportData");

            var result = _customerService.GetCustomerPhoneNumberListAsQuerable(PhoneNumber);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            return new ViewAsPdf("ExportPdf_PhoneNumber", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"PhoneNumberReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult SearchByPhoneNumberExportExcel()
        {
            List<CustomerPhoneNumberModel> reportData = new List<CustomerPhoneNumberModel>();
            string PhoneNumber = string.Empty;
            if (TempData.Peek("InitCustomerPhoneNumberListReportData") != null)
                PhoneNumber = TempData.Get<string>("InitCustomerPhoneNumberListReportData");

            var result = _customerService.GetCustomerPhoneNumberListAsQuerable(PhoneNumber);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.FullName);
            reportList.Columns.Add(Captions.Mobile);
            reportList.Columns.Add(Captions.EssentialTel);
            reportList.Columns.Add(Captions.EssentialTelRatio);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            ColumnWidth[4] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.FullName,
                   item.Mobile,
                   item.EssentialTel,
                   item.EssentialTelRatio);
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, Captions.ManagerOperationReport, true);
            string filename = string.Format("{0}.xlsx", $"PhoneNumberReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion

        public IActionResult CustomerBankCardNumberList_Read([DataSourceRequest] DataSourceRequest request, string? BankCardNumber)
        {
            DataSourceResult dataSource = new DataSourceResult();
            if (string.IsNullOrEmpty(BankCardNumber))
                return Json(dataSource);
            if (BankCardNumber.Length < 4)
                return Json(dataSource);
            BankCardNumber = BankCardNumber.ToEnglishNumberByCultureInfo();
            var result = _customerService.GetCustomerBankCardNumberListAsQuerable(BankCardNumber);
            if (result.IsSuccess)
                dataSource = result.Data.ToDataSourceResult(request);
            return Json(dataSource);
        }

        #region export CustomerBankCardNumberList data


        public JsonResult InitSearchByBankCardNumberReportData(string? BankCardNumber)
        {
            BankCardNumber ??= string.Empty;
            TempData.Set<string>("InitCustomerBankCardNumberListReportData", BankCardNumber);
            return Json(true);
        }

        public IActionResult SearchByBankCardNumberExportPdf()
        {
            List<CustomerBankCardNoModel> reportData = new List<CustomerBankCardNoModel>();
            string BankCardNumber = string.Empty;
            if (TempData.Peek("InitCustomerBankCardNumberListReportData") != null)
                BankCardNumber = TempData.Get<string>("InitCustomerBankCardNumberListReportData");

            var result = _customerService.GetCustomerBankCardNumberListAsQuerable(BankCardNumber);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            return new ViewAsPdf("ExportPdf_BankCardNumber", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"BankCardNumberReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult SearchByBankCardNumberExportExcel()
        {
            string BankCardNumber = string.Empty;
            List<CustomerBankCardNoModel> reportData = new List<CustomerBankCardNoModel>();
            if (TempData.Peek("InitCustomerBankCardNumberListReportData") != null)
                BankCardNumber = TempData.Get<string>("InitCustomerBankCardNumberListReportData");
            var result = _customerService.GetCustomerBankCardNumberListAsQuerable(BankCardNumber);
            if (result.IsSuccess)
                reportData = result.Data.ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.FullName);
            reportList.Columns.Add(Captions.Mobile);
            reportList.Columns.Add(Captions.CardNumber);
            reportList.Columns.Add(Captions.CardNumberOwner);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            ColumnWidth[4] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.FullName,
                   item.Mobile,
                   item.Number,
                   item.Owner);
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, Captions.ManagerOperationReport, true);
            string filename = string.Format("{0}.xlsx", $"BankCardNumberReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion
        #endregion

    }
}
