namespace ThetaFTP.Shared.Models
{
    public class ServerConfigModel
    {
        public enum DatabaseService
        {
            MySql,
            Firebase
        }

        public bool is_reverse_proxy {  get; set; }
        public bool DebugMode { get; set; }
        public bool use_google_secrets { get; set; }
        public string? server_salt_secret_url { get; set; } = "!!! REPLACE WITH SECRET URL !!!";
        public string? server_salt_secret_version { get; set; } = "1";
        public string? firebase_admin_token_secret_url { get; set; } = "!!! REPLACE WITH SECRET URL !!!";
        public string? firebase_admin_token_secret_version { get; set; } = "1";
        public string? mysql_user_password_secret_url { get; set; } = "!!! REPLACE WITH SECRET URL !!!";
        public string? mysql_user_password_secret_version { get; set; } = "1";
        public string? smtp_password_secret_url { get; set; } = "!!! REPLACE WITH SECRET URL !!!";
        public string? smtp_password_secret_version { get; set; } = "1";
        public string? custom_server_certificate_password_secret_url { get; set; } = "!!! REPLACE WITH SECRET URL !!!";
        public string? custom_server_certificate_password_secret_version { get; set; } = "1";
        public string? server_ip_address { get; set; } = "127.0.0.1";
        public int server_port { get; set; } = 8000;
        public string? server_salt { get; set; } = "aaabbbccdddd";
        public bool use_secure_local_storage { get; set; } = true;
        public bool use_firebase { get; set; } = false;
        public string firebase_admin_token { get; set; } = "!!! REPLACE WITH FIREBASE DATABASE ADMIN TOKEN !!!";
        public string? firebase_database_url {  get; set; } = "!!! REPLACE WITH FIREBASE DATABASE URL !!!";
        public string? firebase_api_key { get; set; } = "!!! REPLACE WITH THE APP'S API KEY !!!";
        public string? firebase_auth_domain { get; set; } = "!!! REPLACE WITH THE APP'S AUTH DOMAIN !!!";
        public string? mysql_server_address { get; set; } = "127.0.0.1";
        public bool use_custom_ip_kestrel_config { get; set; }
        public int mysql_server_port { get; set; } = 3306;
        public string? mysql_user_id { get; set; } = "root";
        public string? mysql_user_password { get; set; } = "!!! REPLACE WITH MYSQL USER PASSWORD !!!";
        public string? mysql_database { get; set; } = "thetadrive";
        public bool twoStepAuth { get; set; } = false;
        public string? smtp_email { get; set; } = "!!! REPLACE WITH SMTPS EMAIL ADDRESS !!!";
        public string? smtp_password { get; set; } = "!!! REPLACE WITH SMTPS EMAIL PASSWORD !!!";
        public string? smtp_server { get; set; } = "smtp.gmail.com";
        public int smtp_port { get; set; } = 587;
        public bool smtp_use_ssl { get; set; } = false;
        public int ReadAndWriteOperationsPerSecond { get; set; } = 1200;
        public int ConnectionTimeoutSeconds { get; set; } = 600;
        public bool enforce_https { get; set; } = true;
        public int logs_expiration_days { get; set; } = 10;
        public bool validate_ssl_certificates { get; set; } = false;
        public bool validate_ssl_certificate_chain { get; set; } = false;
        public bool use_custom_ssl_certificate { get; set; }
        public bool ensure_host_name_and_certificate_domain_name_match { get; set; }
        public string? custom_server_certificate_path { get; set; } = "!!! REPLACE WITH THE PATH TO THE SERVER PRIVATE CERTIFICATE !!!";
        public string? custom_server_certificate_password{ get; set; } = "!!! REPLACE WITH THE PATH TO THE SERVER PRIVATE CERTIFICATE !!!";
        public long hsts_max_age_days { get; set; } = 365 * 2;
        public long? max_concurent_connections { get; set; } = null;
        public long? max_request_buffer_size { get; set; } = null;
        public long? max_response_buffer_size { get; set; } = null;
    }
}
