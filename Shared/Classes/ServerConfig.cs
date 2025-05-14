using ThetaFTP.Shared.Models;
using ThetaFTP.Shared.Formatters;
using System.IO;
using System.Text;
using Serilog;

namespace ThetaFTP.Shared.Classes
{
    public class ServerConfig:Shared
    {
        private static readonly string development_app_settings_json_file = "appsettings.Development.json";
        private static readonly string app_settings_json_file = "appsettings.json";


        public static async Task<DefaultAppSettings?> GetServerConfig(bool isDebug)
        {
            string? serialised_json = null;

            try
            {
                
                using (FileStream fs = File.OpenRead(isDebug == true ? development_app_settings_json_file : app_settings_json_file))
                {
                    byte[] json_binary = new byte[fs.Length];
                    await fs.ReadAsync(json_binary, 0, json_binary.Length);
                    serialised_json = Encoding.UTF8.GetString(json_binary);
                }
                
            }
            catch (Exception e)
            {
                Logging.Message(e, "Error reading server configurations", "Json deserialisation error", "JsonFormatter", "JsonSerialiser", Logging.LogType.Error);
            }

            DefaultAppSettings? model = await JsonFormatter.JsonDeserialiser<DefaultAppSettings?>(serialised_json);

            if (model == null)
            {
                string path = new StringBuilder(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append(isDebug == true ? development_app_settings_json_file : app_settings_json_file).ToString();
                await CreateServerConfig(new DefaultAppSettings(), path, isDebug);
                Console.WriteLine(new StringBuilder("\n\n\n Generated app config file at: \n ").Append(path).Append("\n\n\n"));
            }

            return model;
        }

        private static async Task CreateServerConfig(DefaultAppSettings model, string path, bool isDebug)
        {
            try
            {
                model.Logging.LogLevel.Default = isDebug == false ? "None" : "Information";
                model.Logging.LogLevel.Microsoft_AspNetCore = isDebug == false ? "None" : "Warning";

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
                Logging.Message(e, "Error creating server configurations", "Json serialisation error. Check if the app has permissions to write into the app's directory", "JsonFormatter", "JsonSerialiser", Logging.LogType.Error);
            }
        }
    }
}
