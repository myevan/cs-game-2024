using System.ComponentModel.DataAnnotations;

namespace RpcServer.Application
{
    public class AppSettings
    {
        public class ConnectionKeys
        {
            [Required]
            public string Auth { get; set; } = string.Empty;
        }

        public AppSettings()
        {

        }
    }
}
