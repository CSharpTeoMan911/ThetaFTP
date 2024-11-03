namespace ThetaFTP.Shared.Models
{
    public class ServerCredentials
    {
        private static bool _initialized = false;
        public ServerCredentials(ServerConfigModel model)
        {
            if (_initialized == false)
            {
                firebase_admin_token = model.firebase_admin_token;
                firebase_database_url = model.firebase_database_url;
                firebase_api_key = model.firebase_api_key;
                firebase_auth_domain = model.firebase_auth_domain;
                mysql_user_id = model.mysql_user_id;
                mysql_user_password = model.mysql_user_password;
                smtp_email = model.smtp_email;
                smtp_password = model.smtp_password;
                _initialized = true;
            }
        }
        public readonly string firebase_admin_token = "!!! REPLACE WITH FIREBASE DATABASE ADMIN TOKEN !!!";
        public readonly string? firebase_database_url = "!!! REPLACE WITH FIREBASE DATABASE URL !!!";
        public readonly string? firebase_api_key = "!!! REPLACE WITH THE APP'S API KEY !!!";
        public readonly string? firebase_auth_domain = "!!! REPLACE WITH THE APP'S AUTH DOMAIN !!!";
        public readonly string? mysql_user_id = "!!! REPLACE WITH MYSQL USER ID !!!";
        public readonly string? mysql_user_password = "!!! REPLACE WITH MYSQL USER PASSWORD !!!";
        public readonly string? smtp_email = "!!! REPLACE WITH SMTPS EMAIL ADDRESS !!!";
        public readonly string? smtp_password = "!!! REPLACE WITH SMTPS EMAIL PASSWORD !!!";
    }
}
