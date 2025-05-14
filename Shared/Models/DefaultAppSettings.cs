using System.ComponentModel;
using System.Diagnostics.Tracing;

namespace ThetaFTP.Shared.Models
{
    public class DefaultAppSettings
    {
        public class LogLevel
        {
            public string Default { get; set; } = "None";
            [Description("Microsoft.AspNetCore")]
            public string Microsoft_AspNetCore = "None";
        }

        public class Logging_Info
        {
            public LogLevel LogLevel { get; set; } = new LogLevel();
        }

        public class Forwarded_Headers
        {
            public string ForwardedHeaders { get; set; } = "XForwardedFor, XForwardedProto";
        }

        public class Kestrel_Info
        {
            public Kestrel_Endpoints Endpoints { get; set; } = new Kestrel_Endpoints();
        }

        public class Kestrel_Endpoints
        {
            public Kestrel_Address Address { get; set; } = new Kestrel_Address();
        }

        public class Kestrel_Address
        {
            public string Url { get; set; } = "https://localhost:8000";
        }

        public Logging_Info Logging { get; set; } = new Logging_Info();
        public string AllowedHosts { get; set; } = "*";
        public Kestrel_Info Kestrel { get; set; } = new Kestrel_Info();
        public Forwarded_Headers ForwardedHeaders { get; set; } = new Forwarded_Headers();
        public ServerConfigModel ServerConfigModel { get; set; } = new ServerConfigModel();
    }
}
