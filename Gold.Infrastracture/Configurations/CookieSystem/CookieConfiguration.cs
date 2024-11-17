using Gold.Domain.Contract.IRepositories;
using Gold.Infrastracture.Configurations.JWTSystem;
using Gold.Infrastracture.Repositories.UOW;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Configurations.CookieSystem
{
    public static class CookieConfiguration
    {
        public static void AddCookieAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(option =>
                {
                    option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                    options.AccessDeniedPath = "/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    options.Events.OnValidatePrincipal = async (context) =>
                    {
                            //check user security stamp 
                       var _uow= (IUnitOfWork) context.HttpContext.RequestServices.GetService(typeof(IUnitOfWork));
                        if (_uow is not null)
                        {
                            string userName = context.HttpContext.User.GetUserName();
                            if (Guid.TryParse(context.HttpContext.User.GetSecurityStamp(), out Guid securityStamp))
                            {
                                var validationResult = await _uow.UserRepository.ValidateSecurityStampByUserNameAsync(userName, securityStamp);
                                if (!validationResult.IsSuccess)
                                    context.RejectPrincipal();
                            }
                            else
                            {
                                context.RejectPrincipal();
                            }
                        }
                        else
                        {
                            context.RejectPrincipal();
                        }
                    };
                });
        }
    }
}
