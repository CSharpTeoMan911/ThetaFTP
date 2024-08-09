namespace ThetaFTP.Shared.Models
{
    public class ServerConfigModel
    {
        public string? smtps_email { get; set; } = "!!! REPLACE WITH SMTPS EMAIL ADDRESS !!!";
        public string? smtps_password { get; set; } = "!!! REPLACE WITH SMTPS EMAIL PASSWORD !!!";
        public string? smtps_server { get; set; } = "!!! REPLACE WITH SMTPS SERVER ADDRESS !!!";
        public int stmpts_port { get; set; }
        public bool two_step_auth { get; set; } = false;
    }
}
