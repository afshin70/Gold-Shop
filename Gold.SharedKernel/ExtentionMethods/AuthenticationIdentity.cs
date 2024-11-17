using Gold.SharedKernel.ConstData;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.ExtentionMethods
{
    public static class AuthenticationIdentity
    {
        public static string GetName(this ClaimsPrincipal user)
        {
            string value = string.Empty;
            var claim = user.FindFirst(GoldClaimType.Name);
            if (claim != null)
                value = claim.Value;
            return value;
        }

        public static string GetSecurityStamp(this ClaimsPrincipal user)
        {
            string value = string.Empty;
            var claim = user.FindFirst(GoldClaimType.SecurityStamp);
            if (claim != null)
                value = claim.Value;
            return value;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            string value = string.Empty;
            var claim = user.FindFirst(GoldClaimType.UserName);
            if (claim != null)
                value = claim.Value;
            return value;
        }
		public static string GetUserMobile(this ClaimsPrincipal user)
		{
			string value = string.Empty;
			var claim = user.FindFirst(GoldClaimType.Mobile);
			if (claim != null)
				value = claim.Value;
			return value;
		}
		public static string GetUserId(this ClaimsPrincipal user)
        {
            string value = string.Empty;
            var claim = user.FindFirst(GoldClaimType.UserId);
            if (claim != null)
                value = claim.Value;
            return value;
        }
        public static int GetUserIdAsInt(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(GoldClaimType.UserId);
            if (claim != null)
                if (int.TryParse(claim.Value, out int value))
                    return value;
            return 0;
        }
        public static UserType? GetUserType(this ClaimsPrincipal user)
        {
            UserType? value = null;
            var claim = user.FindFirst(GoldClaimType.UserType);
            if (claim != null)
            {
                if (Enum.TryParse<UserType>(claim.Value, out UserType userType))
                    value = userType;
            }
            return value;
        }

        public static string GetUserTypeFaName(this ClaimsPrincipal user)
        {
            UserType? value = null;
            var claim = user.FindFirst(GoldClaimType.UserType);
            if (claim != null)
            {
                if (Enum.TryParse<UserType>(claim.Value, out UserType userType))
                    value = userType;
            }
           
            return value==null?string.Empty:value.GetDisplayName();
        }
        public static List<ManagerPermission> GetManagerPermissions(this ClaimsPrincipal user)
        {
            List<ManagerPermission> list = new List<ManagerPermission>();

            var managerPermissionClaims = user.FindAll(GoldClaimType.ManagerPermission);
            if (managerPermissionClaims != null)
            {
                foreach (Claim claim in managerPermissionClaims)
                {
                    if (Enum.TryParse<ManagerPermission>(claim.Value, out ManagerPermission permission))
                        list.Add(permission);
                }
            }
            return list;
        }

        public static List<byte> GetManagerPermissionIds(this ClaimsPrincipal user)
        {
            List<byte> list = new List<byte>();

            var managerPermissionClaims = user.FindAll(GoldClaimType.ManagerPermission);
            if (managerPermissionClaims != null)
            {
                foreach (Claim claim in managerPermissionClaims)
                {
                    if (byte.TryParse(claim.Value, out byte adminMenuId))
                        list.Add(adminMenuId);
                }
            }
            return list;
        }

        public static bool IsInPermission(this ClaimsPrincipal user, ManagerPermission permission)
        {
            var userType = GetUserType(user);
            bool isInManagerPermission = false;
            if (userType == UserType.Admin)
            {
                return isInManagerPermission = true;
            }
            else if (userType == UserType.Manager)
            {
                var managerPermissions = GetManagerPermissions(user);
                isInManagerPermission = managerPermissions.Any(x => x == permission);
            }
            return isInManagerPermission;
        }
    }
}
