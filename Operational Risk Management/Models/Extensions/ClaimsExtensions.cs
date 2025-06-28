using Operational_Risk_Management.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class ClaimsExtensions
    {

        public static string? GetFullName(this ClaimsPrincipal claims)
        {

            return claims.FindFirstValue("FullName");
        }
        public static string? GetUserName(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue("UserName");
        }
        public static string? GetUserId(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue("Id");
        }
        public static bool IsAdmin(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(UserRoles.Admin.ToString()) == "true";
        }
        public static bool Has_KriSubmission_Access(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(UserRoles.KriSubmissionAccess.ToString()) == "true";
        }
        //public static bool Has_DatabaseVA_Access(this ClaimsPrincipal claims)
        //{
        //    return claims.FindFirstValue(UserRoles.DatabaseVA_Access.ToString()) == "true";
        //}
        //public static bool Has_ReAssign_Access(this ClaimsPrincipal claims)
        //{
        //    return claims.FindFirstValue(UserRoles.ReAssign_Access.ToString()) == "true";
        //}

    }
}
