using ThetaFTP.Shared.Models;
using ThetaFTP.Shared.Formatters;
using System.IO;
using System.Text;

namespace ThetaFTP.Shared.Classes
{
    public class ServerConfig
    {
        private static string json_file = "server_settings.json";
        public static async Task<ServerConfigModel?> GetServerConfig()
        {
            ServerConfigModel? model = await JsonFormatter.JsonDeserialiser<ServerConfigModel>(json_file);
            if (model == null)
                model = await CreateServerConfig(new ServerConfigModel());
            return model;
        }

        private static async Task<ServerConfigModel> CreateServerConfig(ServerConfigModel model)
        {
            FileStream fileStream = File.OpenWrite(json_file);
            try
            {
                string? serialised_model = await JsonFormatter.JsonSerialiser(model);
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
