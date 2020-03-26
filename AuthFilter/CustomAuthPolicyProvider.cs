using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace aspnetcoreauth.AuthFilter
{
    public class CustomAuthPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public CustomAuthPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            //Initializing default policy provider.
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        //This method will be called by the asp.net core pipeline only when Authorize Attribute has Policy Property set
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            try
            {
                //All custom policies created by us will have " : " as delimiter to identify policy name and values
                //Any delimiter or character can be choosen, and it is upto user choice

                var policy = policyName.Split(":").FirstOrDefault(); //Name for policy and values are set in A2AuthorizePermission Attribute
                var attributeValue = policyName.Split(":").LastOrDefault();

                if (policy != null)
                {
                    //Dynamically building the AuthorizationPolicy and adding the respective requirement based on the policy names which we define in Authroize Attribute.
                    var policyBuilder = new AuthorizationPolicyBuilder();

                    if (policy == "dmps.clients")
                    {
                        //Authorize Hanlders are created based on Authroize Requirement type.
                        //Adding the object of HasAuthorizePermissionRequirement will invoke the HasAuthorizationPermissionHandler
                        
                        //options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
                        policyBuilder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(new HasAuthorizePermissionRequirement(attributeValue));
                        return Task.FromResult(policyBuilder.Build());
                    }
                }
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }
            catch (Exception)
            {
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }
        }
    }
}