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
        public bool smtp_use_ssl { get; set; } = false;
        public bool two_step_auth { get; set; } = false;
        public bool validate_ssl_certificates { get; set; } = false;
        public int ReadAndWriteOperationsPerSecond { get; set; } = 1250;
        public int ConnectionTimeoutSeconds { get; set; } = 600;
        public List<string> http_addresses { get; set; } = new List<string>()
        {
            "https://localhost:7040",
            "http://localhost:5219"
        };
        public bool enforce_https { get; set;}
        public int min_worker_threads { get; set; } = 100;
        public int min_input_output_threads { get; set; } = 100;
        public int logs_expiration_days { get; set; } = 10;
    }
}
