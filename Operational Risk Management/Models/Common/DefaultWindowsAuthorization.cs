using Operational_Risk_Management.Models.Context;
using Operational_Risk_Management.Models.Enums;
using Operational_Risk_Management.Models.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Operational_Risk_Management.Models.Common
{
    public class DefaultWindowsAuthorization : IAuthorizationRequirement
    {
        public string? Role { get; set; }
        public DefaultWindowsAuthorization(string? role)
        {
            Role = role;
        }
    }

    public class DefaultWindowsAuthorizationHandler : AuthorizationHandler<DefaultWindowsAuthorization>
    {
        private readonly ApplicationDBContext _dbContext;

        public DefaultWindowsAuthorizationHandler(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DefaultWindowsAuthorization requirement)
        {
            var userName = context.User.Identity?.Name?.Substring(4);
            //userName = "rahyou";
            var user = _dbContext.ApplicationUsers.FirstOrDefault(a => a.UserName == userName);

            if (user != null && !user.Disabled)
            {
                var claims = new ClaimsIdentity(userName);
                claims.AddClaim(new Claim("FullName", user.FullName.ToString()));
                claims.AddClaim(new Claim("UserName", user.UserName.ToString()));
                claims.AddClaim(new Claim("Id", user.Id.ToString()));
                claims.AddClaim(new Claim(UserRoles.Admin.ToString(), user.IsAdmin.ToString().ToLower()));
                claims.AddClaim(new Claim(UserRoles.KriSubmissionAccess.ToString(), user.KriSubmissionAccess.ToString().ToLower()));
                context.User.AddIdentity(claims);
            }
            //check default auth
            if (user != null && requirement.Role == UserRoles.Default.ToString())
            {
                context.Succeed(requirement);
            }
            //check role based
            if (user != null && (
                (requirement.Role==UserRoles.Admin.ToString() && context.User.IsAdmin()) ||
                (requirement.Role==UserRoles.KriSubmissionAccess.ToString() && context.User.Has_KriSubmission_Access())
                )) {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
