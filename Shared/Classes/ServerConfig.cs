using ThetaFTP.Shared.Models;
using ThetaFTP.Shared.Formatters;
using System.IO;
using System.Text;
using Serilog;

namespace ThetaFTP.Shared.Classes
{
    public class ServerConfig:Shared
    {
        private static string json_file = "server_settings.json";
        public static async Task<ServerConfigModel?> GetServerConfig()
        {
            string? serialised_json = null;

            try
            {
                using (FileStream fs = File.OpenRead(json_file))
                {
                    byte[] json_binary = new byte[fs.Length];
                    await fs.ReadAsync(json_binary, 0, json_binary.Length);
                    serialised_json = Encoding.UTF8.GetString(json_binary);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error reading server configurations");
            }

            ServerConfigModel? model = await JsonFormatter.JsonDeserialiser<ServerConfigModel?>(serialised_json);

            if (model == null)
            {
                string path = new StringBuilder(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append(json_file).ToString();
                await CreateServerConfig(new ServerConfigModel(), path);
                Console.WriteLine(new StringBuilder("\n\n\n Generated app config file at: \n ").Append(path).Append("\n\n\n"));
            }

            return model;
        }

        private static async Task CreateServerConfig(ServerConfigModel model, string path)
        {
            try
            {
                using (FileStream fileStream = File.OpenWrite(path))
                {
                    string? serialised_model = await JsonFormatter.JsonSerialiser(model);
                    if (serialised_model != null)
                    {
                        byte[] model_binary = Encoding.UTF8.GetBytes(serialised_model);
                        await fileStream.WriteAsync(model_binary, 0, model_binary.Length);
                        await fileStream.FlushAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error creating server configuration file");
            }
        }
    }
}
