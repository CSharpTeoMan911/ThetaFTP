using ThetaFTP.Shared.Models;
using ThetaFTP.Shared.Formatters;
using System.IO;
using System.Text;

namespace ThetaFTP.Shared.Classes
{
    public class ServerConfig
    {
        private static string instructions = "\n\n\t\t\t [ MySQL OPTIONS ]" +
                                                 "\n\n\t\t(mandatory) mysql_server_address -> The MySQL server's IP address. To get the IP address via command-line follow the instructions at this link: https://serverfault.com/a/788170" +
                                                 "\n\n\t\t(mandatory) mysql_user_id -> The username used to log into MySQL." +
                                                 "\n\n\t\t(mandatory) mysql_user_password -> The password of the account used to log into MySQL." +
                                                 "\n\n\t\t(mandatory) mysql_database -> The name of the database model used by the server application, which comes packaged with the application under the name Theta_FTP.sql" +
            "                                     \n\n\n\t\t\t [ SMTPS OPTIONS ]" +
            "                                     \n\n\t\t(optional) smtp_email -> The email address used to perform SMTP operations." +
            "                                     \n\n\t\t(optional) smtp_password -> The password used by the email address." +
            "                                     \n\n\t\t(optional) smtp_server -> The name of the server used for SMTP operations (e.g. \"smtp_server\": \"smtp.gmail.com\": )." +
            "                                     \n\n\t\t(optional) smtp_port -> Numerical value that specifies the port of the SMTP server through which the operation is done." +
            "                                     \n\n\t\t(optional) smtp_use_ssl -> Boolean value (true or false) that enables SSL encrypted SMTP operations." +
            "                                     \n\n\t\t(optional) two_step_auth -> Boolean value (true or false) that enables 2-step authentication via SMTP. ( WARNING: This option will make all SMTPS variables mandatory )" +
            "                                     \n\n\n\t\t\t [ SERVER OPTIONS ]" +
            "                                     \n\n\t\t(optional) validate_ssl_certificates ->  Boolean value (true or false) that enables certificate validation. Enable this if you have a valid SSL certificate from a valid Certificate Authority." +
            "                                     \n\n\t\t(mandatory) ReadAndWriteOperationsPerSecond -> Numerical value that specifies the number of read and write operations the server can do each second." +
            "                                     \n\n\t\t(mandatory) ConnectionTimeoutSeconds -> Numerical value that specifies the number of seconds the server will keep any client connection open." +
            "                                     \n\n\t\t(mandatory) http_addresses -> List value that holds the HTTP addresses to be used to host the server. The server will take the first address and if it cannot use it, it will use the second one, and so on. (e.g. \"http_addresses\": [https://most_significant_address.com, [http://second_most_significant_address.com])." +
            "                                     \n\n\t\t(optional) enforce_https -> Boolean value (true or false) that specifies if the server should force the clients to use HTTPS connections, if possible." +
            "                                     \n\n\n";


        private static string json_file = "server_settings.json";
        public static async Task<ServerConfigModel?> GetServerConfig()
        {
            string? serialised_json = null;

            try
            {
                FileStream fs = File.OpenRead(json_file);
                try
                {
                    byte[] json_binary = new byte[fs.Length];
                    await fs.ReadAsync(json_binary, 0, json_binary.Length);
                    serialised_json = Encoding.UTF8.GetString(json_binary);
                }
                finally
                {
                    await fs.DisposeAsync();
                }
            }
            catch { }


            ServerConfigModel? model = await JsonFormatter.JsonDeserialiser<ServerConfigModel>(serialised_json);

            if (model == null)
                model = await CreateServerConfig(new ServerConfigModel());
            return model;
        }

        private static async Task<ServerConfigModel> CreateServerConfig(ServerConfigModel model)
        {
            FileStream fileStream = File.OpenWrite(json_file);
            try
            {

                string? serialised_model = await JsonFormatter.JsonSerialiser(model, instructions);
                if (serialised_model != null)
                {
                    byte[] model_binary = Encoding.UTF8.GetBytes(serialised_model);
                    await fileStream.WriteAsync(model_binary, 0, model_binary.Length);
                    await fileStream.FlushAsync();
                }
            }
            finally
            {
                await fileStream.DisposeAsync();
            }

            return model;
        }
    }
}
