using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.Models.GoldPriceModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.GoldPriceViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SiteContent;
using Gold.Domain.Entities;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading;

namespace Gold.EndPoints.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IViewRenderService _renderService;
        private readonly ISettingService _settingService;
        private readonly IContentManagerService _contentManager;
        private readonly ICaptchaResolver _captchaResolver;
        public HomeController(ILogger<HomeController> logger, IViewRenderService renderService, ISettingService settingService, IContentManagerService contentManager, ICaptchaResolver captchaResolver)
        {
            _logger = logger;
            _renderService = renderService;
            _settingService = settingService;
            _contentManager = contentManager;
            _captchaResolver = captchaResolver;
        }

        public IActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    if (User.GetUserType() == UserType.Customer)
            //        return Redirect("/Profile");
            //    if (User.GetUserType() is UserType.Admin or UserType.Seller or UserType.Manager)
            //        return Redirect("/Dashboard");
            //}
            //else
            //{
            //    return Redirect("/login");
            //}

            return View();
        }


        public async Task<IActionResult> GetGoldPriceInfo(CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var result = await _settingService.GetGoldPriceInfoAsync(cancellationToken);
                if (result.IsSuccess)
                {
                    html = await _renderService.RenderViewToStringAsync("_GoldPriceInfo", result.Data, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
                }
                else
                {
                    toastrResult = ToastrResult<string>.Error(Captions.Error, string.Format(OperationResultMessage.NotFound, Captions.GoldPrice), html);
                }
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.ForSeeGoldPrcieMustLogin, html);
            }

            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<IActionResult> CalculateGoldPrice(CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                GoldCalculatorViewModel model = new()
                {
                    StonePrice = "0",
                    GramsGoldPrice = "0",
                    GalleryProfit = "0",
                    Tax = "0",
                };
                var result = await _settingService.GetSettingAsync<LoanSettingsViewModel>(Domain.Enums.SettingType.LoanSetting, cancellationToken);
                if (result.IsSuccess)
                {
                    model.GalleryProfit = string.IsNullOrEmpty(result.Data.MaxProfitGallery) ? "0" : result.Data.MaxProfitGallery;
                    model.Tax = result.Data.Tax.HasValue ? result.Data.Tax.Value.ToString() : "0";
                }

                var goldPriceInfoResult = await _settingService.GetGoldPriceInfoAsync(cancellationToken);
                if (goldPriceInfoResult.IsSuccess)
                    model.GramsGoldPrice = goldPriceInfoResult.Data.Karat18.ToString("N0");

                html = await _renderService.RenderViewToStringAsync("_CalculateGoldPrice", model, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, UserMessages.ForUseNeedToLogin, html);
            }
            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<IActionResult> CalculateGoldPrice(GoldCalculatorViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<CalculatedGoldPriceModel> toastrResult = null;
            if (ModelState.IsValid)
            {
                CommandResult<CalculatedGoldPriceModel> commandResult = await _settingService.GetGoldPriceAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                    toastrResult = ToastrResult<CalculatedGoldPriceModel>.Success(Captions.Success, commandResult.Message, commandResult.Data);
                else
                    toastrResult = ToastrResult<CalculatedGoldPriceModel>.Error(Captions.Error, commandResult.Message, commandResult.Data);
            }
            else
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<CalculatedGoldPriceModel>(ToastrType.Error, false, UserMessages.FormNotValid, new());
            }
            return Json(toastrResult);
        }

        [Route("/IsAuthenticate")]
        public bool IsAuthenticate()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpGet("/AboutUs")]
        public async Task<IActionResult> AboutUs()
        {
            return View();
        }

        #region faq
        [HttpGet("/FAQ")]
        [HttpGet("/FAQ/{categoryId}")]
        public async Task<IActionResult> FAQ(int? categoryId)
        {
            var categories =  _contentManager.GetFAQCategoryAsQuerable().Data.OrderBy(x=>x.OrderNo).ToList();
            ViewBag.FAQCategories = categories;
            List<FAQModel> list = new();
            FAQCategoryModel category = new();
            if (categoryId.HasValue)
                category.Id= categoryId.Value;
            else
                category = categories.FirstOrDefault();
            if (category is not null)
            {
                list = _contentManager.GetFAQAsQuerable().Data
                    .Where(x => x.CategoryId == category.Id)
                    .OrderBy(x => x.OrderNo)
                    .ToList();
            }
            return View(list);
        }

        #endregion



        [HttpGet("/Branchs")]
        public async Task<IActionResult> Branchs()
        {

            return View();
        }

        [HttpGet("/ContactUs")]
        public async Task<IActionResult> ContactUs()
        {
            var captchaModel = _captchaResolver.GenerateCaptchaAsBase64Content(Captions.CaptchaPage_ContactUs, false);
            return View();
        }

        [HttpPost]
        [Route("/ContactUs")]
        public async Task<IActionResult> ContactUs(ContactUsViweModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = null;

            bool isValidCaptcha = true;
            string? captchaValue = HttpContext.Session.GetString(Captions.CaptchaPage_ContactUs);
            if (!model.Captcha.IsEmptyOrNull())
            {
                if (!_captchaResolver.IsValid(captchaValue, model.Captcha))
                {
                    isValidCaptcha = false;
                    ModelState.AddModelError(nameof(model.Captcha), string.Format(ValidationMessages.Invalid, Captions.SecurityCode));
                }
            }

            if (ModelState.IsValid)
            {
                CommandResult<ContactUsViweModel> commandResult = await _contentManager.CreateContactUsMessageAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                    toastrResult = ToastrResult.Success(Captions.Success, UserMessages.YourMessageIsRegistered);
                else
                    toastrResult = ToastrResult.Error(Captions.Error, commandResult.Message);
            }
            else
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
            }
            return Json(toastrResult);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}