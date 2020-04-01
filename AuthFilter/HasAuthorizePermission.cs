using Microsoft.AspNetCore.Authorization;

namespace aspnetcoreauth.AuthFilter
{
    public class HasAuthorizeAttribute : AuthorizeAttribute
    {
        private string _permissions;

        public HasAuthorizeAttribute() : base()
        {
        }

        public string Permissions
        {
            get
            {
                return _permissions;
            }
            set
            {
                _permissions = value;

                //The Policy property should be set for the GetPolicyAsync() Method in CustomAuthPolicyProvider to be invoked
                //Appending value "CustomAuthPermissionPolicy" is to identify which handler to be invoked from Custom Auth Policy Provider.
                //value "CustomAuthPermissionPolicy" is user defined, it can be any value and any delimiter based on user requirement.
                Policy = "clients:" + _permissions;
            }
        }
    }
}