using Gold.EndPoints.Presentation.Models;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.ConstData;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Configurations.ValidationAttribute.Authorization
{
    //public class UserAccess : AuthorizeAttribute, IAuthorizationFilter
    public class UserAccess : Attribute, IAuthorizationFilter
    {
        private string loginUrl = "/login";
        private string accessDeniedUrl = "/AccessDenied";
        private ManagerPermission? _managerPermission;
        private List<UserType> _allowedUserTypes;
        public UserAccess(string userTypes)
        {
            _allowedUserTypes = GetUserTypes(userTypes);
        }
        public UserAccess(string userTypes, ManagerPermission permission)
        {
            _managerPermission = permission;
            _allowedUserTypes = GetUserTypes(userTypes);
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            string returnUrl = $"ReturnUrl={context.HttpContext.Request.Path}";
            RedirectResult redirectToLoginUrl = new RedirectResult($"{loginUrl}?{returnUrl}");
            RedirectResult redirectToAccessDeniedUrl = new RedirectResult($"{accessDeniedUrl}");

            if (context.HttpContext.User.Identity is null)
                await ProcessUnauthorizedResultAsync(context, redirectToLoginUrl);
            else
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    #region check user security stamp 
                    IUnitOfWork? _uow = (IUnitOfWork)context.HttpContext.RequestServices.GetService(typeof(IUnitOfWork));
                    if (_uow is not null)
                    {
                        string userName = context.HttpContext.User.GetUserName();
                        if (Guid.TryParse(context.HttpContext.User.GetSecurityStamp(), out Guid securityStamp))
                        {
                            var validationResult = _uow.UserRepository.ValidateSecurityStampByUserName(userName, securityStamp);
                            if (!validationResult.IsSuccess)
                                await ProcessUnauthorizedResultAsync(context, redirectToLoginUrl);
                        }
                        else
                            ProcessAccessDeniedResult(context, redirectToAccessDeniedUrl);
                    }
                    else
                        ProcessInternalServerErrorResult(context, new ArgumentNullException(nameof(_uow)));
                    #endregion

                    var userType = context.HttpContext.User.GetUserType();
                    if (userType is null)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Result = redirectToLoginUrl;
                    }
                    if (_allowedUserTypes.Any(x => x == userType))
                    {
                        if (userType == UserType.Manager)
                        {
                            //check manager user type permissions
                            if (_managerPermission.HasValue)
                            {
                                var userPermissions = context.HttpContext.User.GetManagerPermissions();
                                if (!userPermissions.Any(x => x == _managerPermission.Value))
                                    ProcessAccessDeniedResult(context, redirectToAccessDeniedUrl);
                            }
                        }
                    }
                    else
                        ProcessAccessDeniedResult(context, redirectToAccessDeniedUrl);
                }
                else
                    await ProcessUnauthorizedResultAsync(context, redirectToLoginUrl);
            }
        }

        /// <summary>
        /// Get user types from user request
        /// </summary>
        /// <param name="userType">userType claim value</param>
        /// <returns>List of UserType enum</returns>
        private List<UserType> GetUserTypes(string userType)
        {
            List<UserType> userTypes = new List<UserType>();
            var userTypesListAsString = userType.Split(",").ToList();
            if (userTypesListAsString is not null)
            {
                foreach (var item in userTypesListAsString)
                {
                    if (Enum.TryParse(item, out UserType type))
                        userTypes.Add(type);
                }
            }
            return userTypes;
        }

        /// <summary>
        /// Process Access Denied Result
        /// </summary>
        /// <param name="authorizationFilterContext">Authorize Context</param>
        /// <param name="redirectResult">Redirect Result</param>
        private void ProcessAccessDeniedResult(AuthorizationFilterContext authorizationFilterContext, RedirectResult redirectResult)
        {
            if (authorizationFilterContext.HttpContext.Request.IsAjaxRequest())
            {
                ToastrResult toastrResult = ToastrResult.Error(Captions.AccessDenied, UserMessages.YouDontHaveAccessContactAdministrator);
                authorizationFilterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                authorizationFilterContext.Result = new JsonResult(toastrResult);
            }
            else
            {
                authorizationFilterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                authorizationFilterContext.Result = redirectResult;
            }
        }

        /// <summary>
        /// Process Access Unauthorized Result
        /// </summary>
        /// <param name="authorizationFilterContext">Authorize Context</param>
        /// <param name="redirectResult">Redirect Result</param>
        /// <returns></returns>
        private async Task ProcessUnauthorizedResultAsync(AuthorizationFilterContext authorizationFilterContext, RedirectResult redirectResult)
        {
            if (authorizationFilterContext.HttpContext.Request.IsAjaxRequest())
            {
                ToastrResult toastrResult = ToastrResult.Error(Captions.AccessDenied, UserMessages.YouDontHaveAccessContactAdministrator);
                authorizationFilterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                authorizationFilterContext.Result = new JsonResult(toastrResult);
            }
            else
            {
                await authorizationFilterContext.HttpContext.SignOutAsync();
                authorizationFilterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                authorizationFilterContext.Result = redirectResult;
            }
        }

        /// <summary>
        /// Process Access Internal Server Error Result
        /// </summary>
        /// <param name="authorizationFilterContext">Authorize Context</param>
        /// <param name="exception">Redirect Result</param>
        private void ProcessInternalServerErrorResult(AuthorizationFilterContext authorizationFilterContext, Exception exception)
        {
            if (authorizationFilterContext.HttpContext.Request.IsAjaxRequest())
            {
                ToastrResult toastrResult = ToastrResult.Error(Captions.Error, OperationResultMessage.AnErrorHasOccurredInTheSoftware);
                authorizationFilterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                authorizationFilterContext.Result = new JsonResult(toastrResult);
            }
            else
            {
                throw exception;
            }
        }
    }
}
