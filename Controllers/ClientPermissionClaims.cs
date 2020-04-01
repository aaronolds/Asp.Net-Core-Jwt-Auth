using System.Collections.Generic;

namespace aspnetcoreauth.Controllers
{
    public class ClientPermissionClaims
    {
        public ClientPermissionClaims()
        {
            ClientPermissions = new List<ClientPermission>();
        }
        public List<ClientPermission> ClientPermissions { get; set; }
    }

}