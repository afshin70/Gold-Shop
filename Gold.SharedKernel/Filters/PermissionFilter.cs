using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gold.SharedKernel.Filters
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string permissionName) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permissionName };
        }
    }

    public class PermissionFilter : IAuthorizationFilter
    {
        readonly string _permissionName;
        //private IUserResolverService _userResolverService;
       // private IOrganizationResolverService _organizationResolverService;

        public PermissionFilter(string permissionName/*,IUserResolverService userResolverService,IOrganizationResolverService organizationResolverService*/)
        {
            _permissionName = permissionName;
            //_userResolverService = userResolverService;
            //_organizationResolverService = organizationResolverService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //_userResolverService = (IUserResolverService)context.HttpContext.RequestServices.GetService(typeof(IUserResolverService));
            //_organizationResolverService = (IOrganizationResolverService)context.HttpContext.RequestServices.GetService(typeof(IOrganizationResolverService));
            //var orgId = _organizationResolverService.OrganizationId;
            //var userId = _userResolverService.UserId;
            // get user role by orgid
            // get permisions of role in org

            // check user has permission
            // if no access , context.Result = new ForbidResult();

            // var hasPermission = .....
            // if (!hasPermission)
            // {
            //     context.Result = new ForbidResult();
            // }


        }
    }
}