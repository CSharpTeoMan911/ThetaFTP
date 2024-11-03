using System.Collections.Generic;

namespace ThetaFTP.Shared.Models
{
    public class ServerConfigurations
    {
        private static bool _initialized = false;
        public ServerConfigurations(ServerConfigModel model) 
        {
            if (_initialized == false)
            {
                use_secure_local_storage = model.use_secure_local_storage;
                use_firebase = model.use_firebase;
                firebase_database_url = model.firebase_database_url;
                firebase_api_key = model.firebase_api_key;
                firebase_auth_domain = model.firebase_auth_domain;
                mysql_server_address = model.mysql_server_address;
                mysql_server_port = model.mysql_server_port;
                mysql_database = model.mysql_database;
                smtp_server = model.smtp_server;
                smtp_port = model.smtp_port;
                smtp_use_ssl = model.smtp_use_ssl;
                two_step_auth = model.two_step_auth;
                validate_ssl_certificates = model.validate_ssl_certificates;
                ReadAndWriteOperationsPerSecond = model.ReadAndWriteOperationsPerSecond;
                ConnectionTimeoutSeconds = model.ConnectionTimeoutSeconds;
                http_addresses = model.http_addresses;
                enforce_https = model.enforce_https;
                min_worker_threads = model.min_worker_threads;
                min_input_output_threads = model.min_input_output_threads;
                logs_expiration_days = model.logs_expiration_days;
                _initialized = true;
            }
        }

        public readonly bool use_secure_local_storage = true;
        public readonly bool use_firebase = true;
        public readonly string? firebase_database_url = "!!! REPLACE WITH FIREBASE DATABASE URL !!!";
        public readonly string? firebase_api_key = "!!! REPLACE WITH THE APP'S API KEY !!!";
        public readonly string? firebase_auth_domain = "!!! REPLACE WITH THE APP'S AUTH DOMAIN !!!";
        public readonly string? mysql_server_address = "!!! REPLACE WITH MYSQL SERVER'S IP ADDRESS !!!";
        public readonly int mysql_server_port = 3306;
        public readonly string? mysql_database = "!!! REPLACE WITH MYSQL DATABASE NAME!!!";
        public readonly string? smtp_server = "!!! REPLACE WITH SMTPS SERVER ADDRESS !!!";
        public readonly int smtp_port;
        public readonly bool smtp_use_ssl = false;
        public readonly bool two_step_auth = false;
        public readonly bool validate_ssl_certificates = false;
        public readonly int ReadAndWriteOperationsPerSecond = 1200;
        public readonly int ConnectionTimeoutSeconds = 600;
        public readonly List<string> http_addresses = new List<string>()
        {
            "https://localhost:7040",
            "http://localhost:5219"
        };
        public readonly bool enforce_https;
        public readonly int min_worker_threads = 100;
        public readonly int min_input_output_threads = 100;
        public readonly int logs_expiration_days = 10;
    }
}
