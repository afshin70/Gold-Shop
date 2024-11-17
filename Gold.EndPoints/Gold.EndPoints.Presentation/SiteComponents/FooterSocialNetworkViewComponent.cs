using Gold.ApplicationService.Contract.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gold.EndPoints.Presentation.SiteComponents
{
    public class FooterSocialNetworkViewComponent : ViewComponent
    {
        private readonly ISettingService _settingService;
        public FooterSocialNetworkViewComponent(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var socialNetworksResult=await _settingService.GetAllSocialNetworkAsync();

            return await Task.FromResult((IViewComponentResult)View("~/Views/Shared/Components/FooterSocialNetwork.cshtml", socialNetworksResult.Data));
        }
    }
}
