using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using aspnetcoreauth.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace aspnetcoreauth.AuthFilter
{
    public class HasAuthorizationPermissionHandler : AuthorizationHandler<HasAuthorizePermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HasAuthorizationPermissionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasAuthorizePermissionRequirement requirement)
        {
            if (string.IsNullOrEmpty(requirement.Permissions) || !(context.User is { }))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // token must have the following claim
            if (!context.User.HasClaim(c => c.Type == "clients"))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            //TODO:  Now that we got the end point data we need to make sure that the client number is in the list of client numbers the user can access
            // then we need to look at the key for the different modules the user wants to access.  The permissions.  It WeatherForcast, ect 
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            string clientNo = routeData?.Values["clientno"]?.ToString();
            var userClients = context.User.Claims.FirstOrDefault(x => x.Type == "clients");
            if (!(userClients is { }))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // make sure the user has access to the client in the URL
            string[] clients = userClients.Value.Split(",");
            if (!clients.Any(x => x == clientNo))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // make sure the the user has permissions to the controller
            var clientpermisionString = context.User.Claims.FirstOrDefault(x => x.Type == "clientpermisions");
            if (!(clientpermisionString is { }))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            var clientPermissions = JsonSerializer.Deserialize<ClientPermissionClaims>(clientpermisionString.Value);
            var requiredPermissions = requirement.Permissions.Split(",");

            // I don't like the client permissions, client permissions
            var clientPermission = clientPermissions.ClientPermissions.Where(x => x.ClientNo == clientNo).FirstOrDefault();
            if (!clientPermission.Permissions.Any(x => requirement.Permissions.Contains(x)))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}


// var userName = context.User.Identity.Name;
// var assignedPermissionsForUser = MockData.UserPermissions.Where(x => x.Key == userName).Select(x => x.Value).ToList();


// var requiredPermissions = requirement.Permissions.Split(",");
// foreach (var x in requiredPermissions)
// {
//     if (assignedPermissionsForUser.Contains(x))
//     {
//         //context.Succeed authorize's the user. 
//         //Note: If multiple authorize attributes are available, if you want the user to be authorized in both the all the attributes, then dont set the context.success here. Just return task completed
//         //setting context.succeed here will not take the control next attribute, it will be marked as authorized in all lower level attributes.
//         context.Succeed(requirement);
//         return Task.CompletedTask;
//     }
// }

//Setting user as not authorized
//context.Fail();