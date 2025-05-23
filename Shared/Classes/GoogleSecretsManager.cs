﻿using Serilog;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class GoogleSecretsManager
    {
        public enum SecretType
        {
            HashSalt,
            FirebaseAdminToken,
            MySqlPassword,
            SmtpPassword,
            SslCertificatePassword,
            AesEncryptionKey
        }

        public async Task<Dictionary<SecretType, string?>> GetSecrets(ServerConfigModel model)
        {
            Dictionary<SecretType, string?> secrets = new Dictionary<SecretType, string?>()
            {
                {SecretType.HashSalt, null},
                {SecretType.FirebaseAdminToken, null},
                {SecretType.MySqlPassword, null},
                {SecretType.SmtpPassword, null},
                {SecretType.SslCertificatePassword, null},
                {SecretType.AesEncryptionKey, null}
            };

            Dictionary<SecretType, string?> secret_urls = new Dictionary<SecretType, string?>()
            {
                {SecretType.HashSalt, model.server_salt_secret_url},
                {SecretType.FirebaseAdminToken, model.firebase_admin_token_secret_url},
                {SecretType.MySqlPassword, model.mysql_user_password_secret_url},
                {SecretType.SmtpPassword, model.smtp_password_secret_url},
                {SecretType.SslCertificatePassword, model.custom_server_certificate_password_secret_url},
                {SecretType.AesEncryptionKey, model.aes_encryption_key_secret_url}
            };

            Dictionary<SecretType, string?> secret_versions = new Dictionary<SecretType, string?>()
            {
                {SecretType.HashSalt, model.server_salt_secret_version},
                {SecretType.FirebaseAdminToken, model.firebase_admin_token_secret_version},
                {SecretType.MySqlPassword, model.mysql_user_password_secret_version},
                {SecretType.SmtpPassword, model.smtp_password_secret_version},
                {SecretType.SslCertificatePassword, model.custom_server_certificate_password_secret_version},
                {SecretType.AesEncryptionKey, model.aes_encryption_key_version}
            };


            try
            {
                SecretManagerServiceClient client = SecretManagerServiceClient.Create();

                for (int i = 0; i < secrets.Keys.Count; i++)
                {
                    try
                    {
                        SecretType secretType = secrets.Keys.ElementAt(i);

                        string? version = String.Empty;
                        secret_versions.TryGetValue(secretType, out version);

                        string? url = String.Empty;
                        secret_urls.TryGetValue(secretType, out url);

                        AccessSecretVersionResponse result = await client.AccessSecretVersionAsync($"{url}/versions/{version}");
                        string secret_data = result.Payload.Data.ToStringUtf8();
                        secrets[secretType] = secret_data;
                    }
                    catch (Exception e)
                    {
                        Logging.Message(e, "Invalid secret URLs or secret versions", "Check the secrets URLs and secrets versions in the configuration file.", "GoogleSecretsManager", "GetSecrets", Logging.LogType.Error);
                    }
                }
            }
            catch(Exception e)
            {
                Logging.Message(e, "Invalid Google Cloud credentials or billing is not enabled credentials.", "Invalid Google Cloud credentials or billing is not enabled credentials. To log in use the command:\n 'gcloud auth application-default login'", "GoogleSecretsManager", "GetSecrets", Logging.LogType.Error);

                Console.Clear();
                Console.WriteLine("\n\n Invalid Google Cloud credentials or billing is not enabled. To log in use the command:\n 'gcloud auth application-default login'");
                throw new Exception(" Invalid Google Cloud credentials or billing is not enabled credentials. To log in use the command:\n 'gcloud auth application-default login'");
            }

            return secrets;
        }
    }
}
