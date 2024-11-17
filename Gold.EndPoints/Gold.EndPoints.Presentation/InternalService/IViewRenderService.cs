using Microsoft.AspNetCore.Mvc;

namespace Gold.EndPoints.Presentation.InternalService
{
    public interface IViewRenderService
    {
        Task<string> RenderPageToStringAsync(string viewName, object model);
        Task<string> RenderViewToStringAsync(string viewName, object model, ControllerContext controllerContext, bool isPartial = false);
    }
}
