using Microsoft.AspNetCore.Authorization;

namespace aspnetcoreauth.AuthFilter
{
    public class HasAuthorizePermissionRequirement : IAuthorizationRequirement
    {
        public string Permissions { get; private set; }

        public HasAuthorizePermissionRequirement(string permissions)
        {
            Permissions = permissions;
        }
    }
}