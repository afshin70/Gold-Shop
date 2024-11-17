using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AuthenticationModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels.RegisterCustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Entities.AuthEntities;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.ConstData;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Tools;
using Hangfire.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading;

namespace Gold.EndPoints.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IViewRenderService _renderService;
        private readonly ILogManager _logManager;
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;

        private readonly ICaptchaResolver _captchaResolver;

        public AccountController(IAuthenticationManager authenticationManager, ICaptchaResolver captchaResolver, IViewRenderService renderService, ILogManager logManager, ICustomerService customerService, IUserService userService)
        {
            this._authenticationManager = authenticationManager;
            _captchaResolver = captchaResolver;
            _renderService = renderService;
            _logManager = logManager;
            _customerService = customerService;
            _userService = userService;
        }


        [HttpGet]
        [Route("/Login")]
        public IActionResult Login(string? ReturnUrl)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            //TempData["CaptchaPage"] = Captions.CaptchaPage_Login;
            var captchaModel = _captchaResolver.GenerateCaptchaAsBase64Content(Captions.CaptchaPage_Login, false);
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            bool isValidCaptcha = true;
            string? captchaValue = HttpContext.Session.GetString(Captions.CaptchaPage_Login);
            if (!model.Captcha.IsEmptyOrNull())
            {
                if (!_captchaResolver.IsValid(captchaValue, model.Captcha))
                {
                    isValidCaptcha = false;
                    ModelState.AddModelError(nameof(model.Captcha), string.Format(ValidationMessages.Invalid, Captions.SecurityCode));
                }
            }
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Warning, false, string.Empty, isValidCaptcha.ToString());
            }
            else
            {
                model.UserName = model.UserName.ToEnglishNumbers();
                var result = await _authenticationManager.LoginUserAsync(HttpContext, model, cancellationToken);
                if (result.IsSuccess)
                {
                    string resultUrl = "/";
                    //Success login
                    string? returnUrl = (string?)TempData["ReturnUrl"];
                    if (returnUrl is not null)
                    {
                        resultUrl = returnUrl;
                    }
                    else
                    {
                        switch (result.Data)
                        {
                            case UserType.Customer:
                                resultUrl = "/";
                                break;
                            case UserType.Seller:
                                resultUrl = "/Dashboard/Product";
                                break;
                            case UserType.Admin:
                                resultUrl = "/Dashboard";
                                break;
                            case UserType.Manager:
                                resultUrl = "/Dashboard";
                                break;
                            default:
                                resultUrl = "/";
                                break;
                        }
                    }

                    toastrResult = ToastrResult<string>.Success(string.Empty, result.Message, resultUrl);
                }
                else
                {
                    //Failure login
                    ModelState.AddModelError(nameof(model.UserName), result.Message);
                    toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Warning, false, string.Empty, string.Empty);
                }
            }


            return Json(toastrResult);
        }

        [HttpGet]
        [Route("/LogOut")]
        public async Task<IActionResult> LogOut(CancellationToken cancellationToken)
        {
            var logoutResult = await _authenticationManager.LogOutUserAsync(HttpContext, cancellationToken);
            if (logoutResult.IsSuccess)
            {
                return RedirectToAction(nameof(Login));
            }
            return Redirect("/");
        }

        #region ثبت نام مشتری
        [HttpGet]
        //[Route("/Register")]
        //[Route("/Register/{NationalityType}/{NationalCode}/{Mobile}")]
        public IActionResult Register(RegisterCustomerBaseInfoViewModel model)
        {
            ModelState.Clear();
            model ??= new();
            
            return View(model);
        }

        [HttpPost]
        //[Route("/Register")]
        public async Task<IActionResult> Register(RegisterCustomerBaseInfoViewModel model, CancellationToken cancellationToken)
        {
            string? captchaValue = HttpContext.Session.GetString(Captions.CaptchaPage_RegisterICustomernSite);
            if (!string.IsNullOrEmpty(model.Captcha))
            {
                if (!_captchaResolver.IsValid(captchaValue, model.Captcha))
                {
                    ModelState.AddModelError(nameof(model.Captcha), string.Format(ValidationMessages.Invalid, Captions.SecurityCode));
                }
            }
            if (model.NationalityType == NationalityType.Iranian)
            {
                if (model.NationalCode is not null)
                {
                    model.NationalCode = model.NationalCode.ToEnglishNumberByCultureInfo();
                    if (model.NationalCode.Length < 10)
                        if (!NationCode.Check(model.NationalCode))
                            ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.MinLength, Captions.NationalCode, 10));

                    if (model.NationalCode.Length > 10)
                        if (!NationCode.Check(model.NationalCode))
                            ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.MaxLength, Captions.NationalCode, 10));

                    if (!NationCode.Check(model.NationalCode))
                        ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.Invalid, Captions.NationalCode));
                }
                else
                {
                    ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.Required, Captions.NationalCode));
                }
            }
            else
            {
                if (model.NationalCode is null)
                    ModelState.AddModelError(nameof(model.NationalCode), string.Format(ValidationMessages.Required, Captions.IdentificationCode));
            }

            CommandResult<bool> isExistCustomer = await _userService.IsExistUserByUserNameAsync(model.NationalCode, cancellationToken);
            if (isExistCustomer.IsSuccess)
            {
                if (isExistCustomer.Data)
                    ModelState.AddModelError(nameof(model.NationalCode), string.Format(UserMessages.AnotherUserHasRegisteredOnTheSiteWithThisInformation));
            }
            else
            {
                ModelState.AddModelError(nameof(model.NationalCode), isExistCustomer.Message);
            }
            if (!ModelState.IsValid)
            {
                ModelState.SetModelValue(nameof(model.Captcha), new ValueProviderResult(string.Empty, CultureInfo.InvariantCulture));
                return View(model);
            }

            var result = await _authenticationManager.ProcessRegisterCustomerOTPCodeAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                return View(nameof(VerifyRegisterCode), result.Data);
            }
            else
            {
                ModelState.AddModelError(nameof(model.NationalCode), result.Message);
                model.Captcha = string.Empty;
                return View();
            }
        }
        [HttpPost("/VerifyRegisterCode")]
        public async Task<IActionResult> VerifyRegisterCode(RegisterCustomerVerifyCodeViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (model.VerifyCodeNumber != null)
                {
                    for (int i = 0; i < model.VerifyCodeNumber.Length; i++)
                        model.VerifyCode += $"{model.VerifyCodeNumber[i].ToEnglishNumbers()}";
                }
                else
                {
                    ModelState.AddModelError(nameof(model.VerifyCode), string.Format(ValidationMessages.Required, Captions.VerifyCode));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(nameof(model.VerifyCode), string.Format(ValidationMessages.Numberic, Captions.VerifyCode));
            }

            if (!ModelState.IsValid)
            {
                CommandResult<TimeSpan> smsTimerResult = await _authenticationManager.GetRegisterCustomerSmsTimerAsync(model, cancellationToken);
                model.Timer = smsTimerResult.Data;
                return View(model);
            }

            if (string.IsNullOrEmpty(model.NationalCode) | string.IsNullOrEmpty(model.Mobile) | model.NationalityType == null)
                return Redirect("/Account/Register");

            CommandResult<RegisterCustomerVerifyCodeViewModel> result = await _authenticationManager.VerifyRegisterCustomerOTPCodeAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                RegisterCustomerViewModel resetPasswordModel = new()
                {
                    NationalCode = result.Data.NationalCode,
                    Mobile = result.Data.Mobile,
                    NationalityType = result.Data.NationalityType,
                    Token = result.Data.Token,
                };
                //redirect to Final step for Register
                return View(nameof(RegisterCustomer), resetPasswordModel);
                //return RedirectToAction(nameof(ResetPassword), resetPasswordModel);
            }
            else
            {
                CommandResult<TimeSpan> smsTimerResult = await _authenticationManager.GetRegisterCustomerSmsTimerAsync(model, cancellationToken);
                model.Timer = smsTimerResult.Data;
                ModelState.AddModelError(nameof(model.VerifyCode), result.Message);
                return View(model);
            }
        }

        [Route("/RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomerViewModel model, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(model.NationalCode) | string.IsNullOrEmpty(model.Mobile) | !model.NationalityType.HasValue | !model.Token.HasValue)
                return Redirect("/Account/Register");

            if (model.NationalityType.HasValue & model.NationalityType == NationalityType.Iranian)
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
            CommandResult<bool> isExistCustomer = await _userService.IsExistUserByUserNameAsync(model.NationalCode, cancellationToken);
            if (isExistCustomer.IsSuccess)
            {
                if (isExistCustomer.Data)
                    ModelState.AddModelError(nameof(model.NationalCode), string.Format(UserMessages.AnotherUserHasRegisteredOnTheSiteWithThisInformation));
            }
            else
            {
                ModelState.AddModelError(nameof(model.NationalCode), isExistCustomer.Message);
            }
            if (!ModelState.IsValid)
                return View(model);
            CreateOrEditCustomerViewModel customer = new();
            customer.IsActive = true;
            customer.NationalCode = model.NationalCode;
            customer.Nationality = model.NationalityType;
            customer.Mobile = model.Mobile;
            customer.FullName = model.FullName;
            customer.Gender = model.Gender;
            customer.FatherName = model.FatherName;
            customer.BirthDate = model.BirthDate;
            customer.EssentialTels = new EssentialTelViewModel
            {
                Tel = model.EssentialTel,
                RelationShip = model.RelationShip
            };
            CommandResult result = await _customerService.CreateCustomerAsync(customer, cancellationToken);
            if (result.IsSuccess)
            {
                var loginModel = new LoginViewModel
                {
                    UserName = customer.NationalCode,
                    Password = customer.Mobile,
                    RememberMe = true
                };
                var loginResult = await _authenticationManager.LoginUserAsync(HttpContext, loginModel, cancellationToken);
                if (loginResult.IsSuccess)
                {
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FullName), loginResult.Message);
                    return View(model);
                }
                //redirect to home
               // return Redirect("/");
            }
            else
            {
                ModelState.AddModelError(nameof(model.FullName), result.Message);
                return View(model);
            }
        }

        [HttpPost]
        [Route("/ResendRegisterVerifyCode")]
        public async Task<IActionResult> ResendRegisterCustomerVerifyCode(NationalityType nationalityType, string? userName, string? mobile, CancellationToken cancellationToken)
        {
            ToastrResult<RegisterCustomerVerifyCodeViewModel> toastrResult = new();
            if (string.IsNullOrEmpty(mobile))
                toastrResult = ToastrResult<RegisterCustomerVerifyCodeViewModel>.Warning(string.Empty, string.Format(ValidationMessages.Invalid, Captions.Mobile), new());
            else if (string.IsNullOrEmpty(userName))
                toastrResult = ToastrResult<RegisterCustomerVerifyCodeViewModel>.Warning(string.Empty, string.Format(ValidationMessages.Invalid, Captions.NationalCode_UserName), new());
            else
            {
                var resendSmsResult = await _authenticationManager.ResendRegisterCustomerVerifyOTPCodeAsync(nationalityType, userName, mobile, cancellationToken);
                if (resendSmsResult.IsSuccess)
                    toastrResult = ToastrResult<RegisterCustomerVerifyCodeViewModel>.Success(string.Empty, UserMessages.TheVerifyCodeISended, resendSmsResult.Data);
                else
                    toastrResult = ToastrResult<RegisterCustomerVerifyCodeViewModel>.Warning(string.Empty, resendSmsResult.Message, resendSmsResult.Data);
            }
            return Json(toastrResult);
        }
        #endregion

        #region بازیابی رمز ورود
        [HttpGet]
        [Route("/ForgetPassword")]
        public IActionResult ForgetPassword()
        {

            return View();
        }

        [HttpPost]
        [Route("/ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model, CancellationToken cancellationToken)
        {
            string? captchaValue = HttpContext.Session.GetString(Captions.CaptchaPage_ForgetPassword);
            if (!model.Captcha.IsEmptyOrNull())
            {
                if (!_captchaResolver.IsValid(captchaValue, model.Captcha))
                {
                    ModelState.AddModelError(nameof(model.Captcha), string.Format(ValidationMessages.Invalid, Captions.SecurityCode));
                }
            }
            if (!ModelState.IsValid)
                return View(model);
            CommandResult<VerifyResetPasswordCodeViewModel> result = await _authenticationManager.ProcessForgetPasswordAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                return View(nameof(VerifyCode), result.Data);
                //return RedirectToAction(nameof(VerifyResetPasswordCode), result.Data);
            }
            else
            {
                ModelState.AddModelError(nameof(model.UserName), result.Message);
                model.Captcha = string.Empty;
                return View();
            }
        }

        [Route("/VerifyCode")]
        public async Task<IActionResult> VerifyCode(VerifyResetPasswordCodeViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (model.VerifyCodeNumber != null)
                {
                    for (int i = 0; i < model.VerifyCodeNumber.Length; i++)
                    {
                        model.VerifyCode += $"{model.VerifyCodeNumber[i].ToEnglishNumbers()}";
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(model.VerifyCode), string.Format(ValidationMessages.Required, Captions.VerifyCode));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(nameof(model.VerifyCode), string.Format(ValidationMessages.Numberic, Captions.VerifyCode));
            }
            if (!ModelState.IsValid)
            {
                CommandResult<TimeSpan> smsTimerResult = await _authenticationManager.GetResetPasswordSmsTimerAsync(model, cancellationToken);
                model.Timer = smsTimerResult.Data;
                return View(model);
            }
            if (string.IsNullOrEmpty(model.UserName) | string.IsNullOrEmpty(model.Mobile))
                return Redirect("/ForgetPassword");

            CommandResult<VerifyResetPasswordCodeViewModel> result = await _authenticationManager.VerifyResetPasswordOTPCodeAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                ChangePasswordByTokenViewModel resetPasswordModel = new()
                {
                    UserName = result.Data.UserName,
                    PasswordChanged = false,
                    Token = result.Data.Token,
                };
                //redirect to change password
                return View(nameof(ResetPassword), resetPasswordModel);
                //return RedirectToAction(nameof(ResetPassword), resetPasswordModel);
            }
            else
            {
                CommandResult<TimeSpan> smsTimerResult = await _authenticationManager.GetResetPasswordSmsTimerAsync(model, cancellationToken);
                model.Timer = smsTimerResult.Data;
                ModelState.AddModelError(nameof(model.VerifyCode), result.Message);
                return View(model);
            }
        }


        [Route("/ResetPassword")]
        public async Task<IActionResult> ResetPassword(ChangePasswordByTokenViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (string.IsNullOrEmpty(model.UserName) | !model.Token.HasValue)
                return Redirect("/ForgetPassword");

            CommandResult result = await _authenticationManager.ChangePasswordByTokenAsync(model, cancellationToken);
            if (result.IsSuccess)
            {
                //redirect to change password
                model.PasswordChanged = true;
                return View("SuccessResetPasswordMessage");
            }
            else
            {
                ModelState.AddModelError(nameof(model.Password), result.Message);
                return View(model);
            }
        }


        [HttpPost]
        [Route("/ResendVerifyCode")]
        public async Task<IActionResult> ResendPasswordRecoveryVerifyCode(string? userName, string? mobile, CancellationToken cancellationToken)
        {
            ToastrResult<VerifyResetPasswordCodeViewModel> toastrResult = new();
            if (string.IsNullOrEmpty(mobile))
                toastrResult = ToastrResult<VerifyResetPasswordCodeViewModel>.Warning(string.Empty, string.Format(ValidationMessages.Invalid, Captions.Mobile), new());
            else if (string.IsNullOrEmpty(userName))
                toastrResult = ToastrResult<VerifyResetPasswordCodeViewModel>.Warning(string.Empty, string.Format(ValidationMessages.Invalid, Captions.UserName), new());
            else
            {
                var resendSmsResult = await _authenticationManager.ResendVerifyResetPasswordOTPCodeAsync(userName, mobile, cancellationToken);
                if (resendSmsResult.IsSuccess)
                    toastrResult = ToastrResult<VerifyResetPasswordCodeViewModel>.Success(string.Empty, UserMessages.TheVerifyCodeISended, resendSmsResult.Data);
                else
                    toastrResult = ToastrResult<VerifyResetPasswordCodeViewModel>.Warning(string.Empty, resendSmsResult.Message, resendSmsResult.Data);
            }
            return Json(toastrResult);
        }


        #endregion

        [Route("/test")]
        public IActionResult test()
        {
            return View("SuccessResetPasswordMessage");
        }

        #region تغییر رمز ورود ادمین

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)},{nameof(UserType.Seller)},{nameof(UserType.Customer)}")]
        [HttpGet]
        [Route("/ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            ChangeUserPasswordViewModel model = new();

            html = await _renderService.RenderViewToStringAsync("_ChangeUserPassword", model, this.ControllerContext);
            toastrResult = ToastrResult<string>.Success(Captions.Warning, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            return Json(toastrResult);
        }

        [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)},{nameof(UserType.Seller)},{nameof(UserType.Customer)}")]
        [HttpPost]
        [Route("/ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Warning, false, UserMessages.FormNotValid, string.Empty);
            }
            else
            {
                //model is valid
                ChangeUserPasswordModel changeUserPasswordModel = new()
                {
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    UserName = User.GetUserName(),
                };

                CommandResult updateResult = await _authenticationManager.ChangePasswordAsync(changeUserPasswordModel, cancellationToken);
                if (updateResult.IsSuccess)
                {
                    var loginModel = new LoginViewModel
                    {
                        Password = changeUserPasswordModel.NewPassword,
                        RememberMe = true,
                        UserName = User.GetUserName(),
                    };
                    //await _authenticationManager.LogOutUserAsync(HttpContext, cancellationToken);
                    await _authenticationManager.LoginUserAsync(HttpContext, loginModel, cancellationToken);
                    toastrResult = ToastrResult.Success(Captions.Success, updateResult.Message);
                }
                else
                    toastrResult = ToastrResult.Warning(Captions.Warning, updateResult.Message);
            }

            return Json(toastrResult);
        }

        #endregion
    }
}
