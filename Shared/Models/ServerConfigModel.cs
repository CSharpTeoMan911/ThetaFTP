namespace ThetaFTP.Shared.Models
{
    public class ServerConfigModel
    {
        public string? mysql_server_address { get; set; } = "!!! REPLACE WITH MYSQL SERVER'S IP ADDRESS !!!";
        public string? mysql_user_id { get; set; } = "!!! REPLACE WITH MYSQL USER ID !!!";
        public string? mysql_user_password { get; set; } = "!!! REPLACE WITH MYSQL USER PASSWORD !!!";
        public string? mysql_database { get; set; } = "!!! REPLACE WITH MYSQL DATABASE NAME!!!";
        public string? smtp_email { get; set; } = "!!! REPLACE WITH SMTPS EMAIL ADDRESS !!!";
        public string? smtp_password { get; set; } = "!!! REPLACE WITH SMTPS EMAIL PASSWORD !!!";
        public string? smtp_server { get; set; } = "!!! REPLACE WITH SMTPS SERVER ADDRESS !!!";
        public int smtp_port { get; set; }
        public bool smtp_use_ssl { get; set; }
        public bool two_step_auth { get; set; } = false;
    }
}
