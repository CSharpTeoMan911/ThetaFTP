using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using ThetaFTP.Data;
using ThetaFTP.Shared;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;

namespace ThetaFTP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            _= Main_OP(args).Result;
        }

        private static async Task<bool> Main_OP(string[] args)
        {
            Shared.Shared.config = await ServerConfig.GetServerConfig();

            System.Timers.Timer server_utility_timer = new System.Timers.Timer();
            server_utility_timer.Elapsed += Server_utility_timer_Elapsed;
            server_utility_timer.Interval = 600000;
            server_utility_timer.Start();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddMvc();
            builder.Services.AddControllers(options =>
            {
                options.InputFormatters.Add(new StreamFormatter());
                options.InputFormatters.Add(new JsonTextInputFormatter());
                options.OutputFormatters.Add(new StreamOutputFormatter());
            });
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });


            List<string>? urls = builder.Configuration[WebHostDefaults.ServerUrlsKey]?.Split(';').ToList();
            List<string>? config_urls = new List<string>();

            Shared.Shared.config?.http_addresses?.ForEach(element =>
            {
                if (urls?.Contains(element) == false)
                    config_urls.Add(element);
            });

            if (config_urls.Count > 0)
                builder.WebHost.UseUrls(config_urls.ToArray());



            //////////////////////////////////////////////
            //              !!! TO DO !!!               //
            //////////////////////////////////////////////
            //                                          //
            // COMFIGURE SERVER'S KESTER SERVICE LIMITS //
            //                                          //
            //////////////////////////////////////////////
            ///

            int connection_timeout = 600;
            if (Shared.Shared.config != null)
                connection_timeout = Shared.Shared.config.ConnectionTimeoutSeconds;


            builder.WebHost.ConfigureKestrel(c =>
            {
                c.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(connection_timeout);
                c.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(connection_timeout);
                c.Limits.MinResponseDataRate = null;
                c.Limits.MaxConcurrentConnections = null;
                c.Limits.MinResponseDataRate = null;
                c.Limits.MinRequestBodyDataRate = null;
                c.Limits.MaxRequestBodySize = null;
                c.Limits.MaxConcurrentConnections = null;
                c.Limits.MaxConcurrentUpgradedConnections = null;
            });
            //////////////////////////////////////////////
            //              !!! TO DO !!!               //
            //////////////////////////////////////////////
            //                                          //
            // COMFIGURE SERVER'S KESTER SERVICE LIMITS //
            //                                          //
            //////////////////////////////////////////////






            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
                });
            }


            app.MapControllers();
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();

            return true;
        }

        private static async void Server_utility_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            await Shared.Shared.databaseServerFunctions.DeleteDatabaseCache();
        }
    }
}
