using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Gold.EndPoints.Presentation.Utility
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class NotFoundPageMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundPageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);
            if (httpContext.Response.StatusCode==(int)HttpStatusCode.NotFound)
                httpContext.Response.Redirect("/NotFound");
        }
    }

    public static class NotFoundPageMiddlewareExtensions
    {
        public static IApplicationBuilder UseNotFoundPage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NotFoundPageMiddleware>();
        }
    }
}
