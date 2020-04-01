using System.Collections.Generic;

namespace aspnetcoreauth.Controllers
{
    public class ClientPermission
    {
        public ClientPermission()
        {

        }
        public ClientPermission(string clientNo, List<string> permissions)
        {
            ClientNo = clientNo;
            Permissions = permissions;
        }

        public string ClientNo { get; set; }
        public List<string> Permissions { get; set; }
    }

}