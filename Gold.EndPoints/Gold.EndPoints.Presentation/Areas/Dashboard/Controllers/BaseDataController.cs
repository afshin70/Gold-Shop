using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)},{nameof(UserType.Customer)},{nameof(UserType.Seller)}")]
    public class BaseDataController : Controller
    {
        private readonly IProvinceService _provinceService;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly ISellerService _sellerService;

        public BaseDataController(IProvinceService provinceService,ISettingService settingService,ICustomerService customerService,ISellerService sellerService)
        {
            _provinceService=provinceService;
           _settingService = settingService;
           _customerService = customerService;
           _sellerService = sellerService;
        }
        [HttpGet]
        public async Task<JsonResult> GallerySellers(int id,CancellationToken cancellationToken)
        {
            IEnumerable<SelectListItem> selers = new List<SelectListItem>();
            var result = await _sellerService.GetSellersOfGalleryAsync(id,0, cancellationToken);
            if (result.IsSuccess)
            {
                selers = result.Data;
            }
            return Json(selers);
        }

        [HttpGet]
        public async Task<JsonResult> GallerySellersForRegisterDocument(int id, CancellationToken cancellationToken)
        {
            IEnumerable<SelectListItem> selers = new List<SelectListItem>();
            var result = await _sellerService.GetActiveSellersOfGalleryAsync(id, 0, cancellationToken);
            if (result.IsSuccess)
            {
                selers = result.Data;
            }
            return Json(selers);
        }

        [HttpGet]
        public async Task<JsonResult> Cities(int id,CancellationToken cancellationToken)
        {
            IEnumerable<SelectListItem> citiesItems = new List<SelectListItem>();
            var result = await _provinceService.GetCitiesOfProvinceAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                 citiesItems = result.Data.Select(x => new SelectListItem
                {
                    Text=x.Title,
                    Value=x.Id.ToString()
                });
            }
            return Json(citiesItems);
        }
        
        [HttpGet]
        public async Task<JsonResult> Proviances(CancellationToken cancellationToken)
        {
            IEnumerable<SelectListItem> proviancesItems = new List<SelectListItem>();
            var result = await _provinceService.GetAllProvincesAsync(cancellationToken);
            if (result.IsSuccess)
            {
                proviancesItems = result.Data.Select(x => new SelectListItem
                {
                    Text=x.Title,
                    Value=x.Id.ToString()
                });
            }
            return Json(proviancesItems);
        }


        [HttpGet] 
       public async Task<JsonResult> GetMonthlyProfitPercentageValue(CancellationToken cancellationToken)
        {
            decimal monthlyProfitPercentage;
            var result=await _settingService.GetSettingAsync< LoanSettingsViewModel >(Domain.Enums.SettingType.LoanSetting, cancellationToken);
            if (result.IsSuccess)
            {
                if (!decimal.TryParse(result.Data.MonthlyProfitPercentage,out monthlyProfitPercentage))
                {
                    monthlyProfitPercentage = default(decimal);
                }
            }
            else
            {
                monthlyProfitPercentage=default(decimal);
            }
            return Json(monthlyProfitPercentage.ToString());
        }

	

	}
}
