using Gold.EndPoints.Presentation.Models;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Gold.EndPoints.Presentation.Utility
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class Exception
    {
        private readonly RequestDelegate _next;

        public Exception(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (System.Exception ex)
            {
                ILogManager? logService = (ILogManager?)httpContext.RequestServices.GetService(typeof(ILogManager));
                if (logService is not null)
                    logService.RaiseLog(ex);
                httpContext.Response.Redirect("/Error");
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionExtensions
    {
        public static IApplicationBuilder UseException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Exception>();
        }
    }
}
