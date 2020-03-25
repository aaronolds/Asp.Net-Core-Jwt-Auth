using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace aspnetcoreauth.AuthFilter
{
    public class HasAuthorizationPermissionHandler : AuthorizationHandler<HasAuthorizePermissionRequirement>
    {
        public HasAuthorizationPermissionHandler()
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasAuthorizePermissionRequirement requirement)
        {
            if (string.IsNullOrEmpty(requirement.Permissions))
            {
                context.Fail();
            }

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}