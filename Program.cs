using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using Serilog;

namespace ThetaFTP
{
    public class Program:Shared.Shared
    {
        public static void Main(string[] args)
        {
            _= Main_OP(args).Result;
        }

        private static async Task<bool> Main_OP(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("ServerErrorLogs.txt",
            rollingInterval: RollingInterval.Infinite,
            rollOnFileSizeLimit: true)
            .CreateLogger();

            try
            {
                await ServerConfig.GetServerConfig();

                if (configurations != null)
                    ThreadPool.SetMinThreads(configurations.min_worker_threads, configurations.min_input_output_threads);
                else
                    Environment.Exit(0);

                System.Timers.Timer server_utility_timer = new System.Timers.Timer();
                server_utility_timer.Elapsed += Server_utility_timer_Elapsed;
                server_utility_timer.Interval = 60000;
                server_utility_timer.Start();

                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddHttpClient(Shared.Shared.HttpClientConfig, client => {
                    int timeout = 600;
                    if (configurations != null)
                        timeout = configurations.ConnectionTimeoutSeconds;
                    client.Timeout = TimeSpan.FromSeconds(timeout);

                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        if (configurations != null)
                            return !configurations.validate_ssl_certificates;
                        return true;
                    }
                }).SetHandlerLifetime(TimeSpan.FromSeconds(configurations.ConnectionTimeoutSeconds));

                builder.Services.AddRazorPages();
                builder.Services.AddServerSideBlazor();
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

                if (configurations != null)
                {
                    List<string>? addresses = configurations.http_addresses.Distinct().ToList();
                    List<string>? urls = builder.Configuration[WebHostDefaults.ServerUrlsKey]?.Split(';').ToList();
                    List<string>? config_urls = new List<string>();

                    addresses?.ForEach(element =>
                    {
                        if (urls?.Contains(element) == false)
                            config_urls.Add(element);
                    });

                    if (config_urls.Count > 0)
                        builder.WebHost.UseUrls(config_urls.ToArray());
                }


                //////////////////////////////////////////////
                //              !!! TO DO !!!               //
                //////////////////////////////////////////////
                //                                          //
                // COMFIGURE SERVER'S KESTER SERVICE LIMITS //
                //                                          //
                //////////////////////////////////////////////
                ///

                int connection_timeout = 600;
                if (configurations != null)
                    connection_timeout = configurations.ConnectionTimeoutSeconds;


                builder.WebHost.ConfigureKestrel(c =>
                {
                    c.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(connection_timeout);
                    c.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(connection_timeout);
                    c.Limits.MinResponseDataRate = null;
                    c.Limits.MaxConcurrentConnections = null;
                    c.Limits.MinRequestBodyDataRate = null;
                    c.Limits.MaxRequestBodySize = null;
                    c.Limits.MaxConcurrentUpgradedConnections = null;
                });
                //////////////////////////////////////////////
                //              !!! TO DO !!!               //
                //////////////////////////////////////////////
                //                                          //
                // COMFIGURE SERVER'S KESTER SERVICE LIMITS //
                //                                          //
                //////////////////////////////////////////////

                if (configurations?.enforce_https == true)
                    builder.Services.AddHsts(option =>
                    {
                        option.MaxAge = TimeSpan.FromDays(double.MaxValue);
                        option.Preload = true;
                        option.IncludeSubDomains = true;
                    });




                var app = builder.Build();


                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");

                    if (configurations?.enforce_https == true)
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
            }
            catch(Exception E)
            {
                Log.Fatal(E, "Fatal error");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }

            return true;
        }

        private static async void Server_utility_timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (configurations?.use_firebase == true)
                await fireabseDatabaseServerFunctions.DeleteDatabaseCache();
            else
                await databaseServerFunctions.DeleteDatabaseCache();
        }
    }
}
