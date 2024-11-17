using Gold.ApplicationService.ServiceCollector;
using Gold.EndPoints.Presentation.InternalService;
using Gold.Infrastracture.Configurations.CookieSystem;
using Gold.Infrastracture.LogSystem;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Infrastracture.ServiceCollector;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;
using NuGet.Configuration;
using NuGet.Packaging;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebMarkupMin.AspNetCore6;
using Hangfire;
using Hangfire.Dashboard;
using Rotativa.AspNetCore;
using Gold.EndPoints.Presentation.Utility;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
#region ApplicationService DI
builder.Services.Configure<FilePathAddress>(builder.Configuration.GetSection("FilePathAddress"));

builder.Services.AddApplicationSerices();
#endregion

//cookie auth configuration
//builder.Services.AddCookieAuthentication();
builder.Services.AddAuthentication(option =>
                {
                    option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(options =>
                {
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                    options.AccessDeniedPath = "/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                });

#region Infrastructure DI
builder.Services.AddRepositories();
builder.Services.AddEFCoreMSSQLContext(builder.Configuration.GetConnectionString("ConnectionSrting"));
builder.Services.AddApplicationLogSystem(builder.Configuration.GetConnectionString("SystemActivityConnectionSrting"));
builder.Services.AddSmsService(builder.Configuration);
builder.Services.AddCaptchaService();
//builder.Services.AddInMemoryStorage();
#endregion

builder.Services.AddKendo();
builder.Services.AddMvc().AddRazorRuntimeCompilation();
builder.Services.AddMvc().AddNewtonsoftJson(jsonOptions =>
{
    jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
builder.Services.AddSingleton<HtmlEncoder>(
  HtmlEncoder.Create(allowedRanges: new[]
  {
      UnicodeRanges.BasicLatin,
      UnicodeRanges.Arabic
  }));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddWebMarkupMin(options =>
{
    options.AllowMinificationInDevelopmentEnvironment = false;
    options.AllowCompressionInDevelopmentEnvironment = false;
}).AddHtmlMinification().AddHttpCompression().AddXmlMinification();

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("ConnectionSrting"));
});

builder.Services.AddTransient<IViewRenderService, ViewRenderService>();
string documentFilesAddress = builder.Configuration.GetSection("DocumentFileAddress").Value;


builder.Services.AddHangfireServer();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseException();
    //  app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}
app.UseNotFoundPage();
app.UseWebMarkupMin();
app.UseHttpsRedirection();
//var options = new DashboardOptions
//{
//	Authorization = new List<IDashboardAuthorizationFilter>
//			  {
//				  new HangfireAuthorizationFilter ()
//			   }
//};
//app.UseHangfireDashboard("/hangfire", options);
//app.UseHangfireDashboard("/hangfire");
app.UseHangfireServer();

//app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "")),
    RequestPath = "",
    OnPrepareResponse = (ctx) =>
    {
        //reject request 
        //secure the path:/Files/Documents for files

        if (ctx.Context.Request.Path.StartsWithSegments(documentFilesAddress))
        {
            ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
            ctx.Context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            ctx.Context.Response.ContentLength = 0;
            ctx.Context.Response.Body = Stream.Null;
        }
    }
});

app.UseRouting();

//app.UseAuthorization();
app.UseAuthentication();
RotativaConfiguration.Setup(app.Environment.WebRootPath);
app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
});

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
                   name: "areaRoute",
                   pattern: "{area:exists}/{controller=Account}/{action=Index}/{id?}/");
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    //endpoints.MapControllerRoute(
    //  name: "areas",
    //  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    //);
});

app.Run();
