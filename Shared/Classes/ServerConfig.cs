﻿using ThetaFTP.Shared.Models;
using ThetaFTP.Shared.Formatters;
using System.IO;
using System.Text;
using Serilog;

namespace ThetaFTP.Shared.Classes
{
    public class ServerConfig:Shared
    {
        private static string json_file = "server_settings.json";
        public static async Task GetServerConfig()
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
                catch (Exception e)
                {
                    Log.Error(e, "Error reading server configurations");
                }
                finally
                {
                    await fs.DisposeAsync();
                }
            }
            catch { }

            ServerConfigModel? model = await JsonFormatter.JsonDeserialiser<ServerConfigModel?>(serialised_json);

            if (model == null)
            {
                await CreateServerConfig(new ServerConfigModel());
                Console.WriteLine(new StringBuilder("\n\n\n\tGenerated app config file at: ").Append(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append(json_file).Append("\n\n\n"));
            }
            else
            {
                credentials = new ServerCredentials(model);
                configurations = new ServerConfigurations(model);
            }
        }

        private static async Task CreateServerConfig(ServerConfigModel model)
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
            catch (Exception e)
            {
                Log.Error(e, "Error creating server configuration file");
            }
            finally
            {
                await fileStream.DisposeAsync();
            }
        }
    }
}
