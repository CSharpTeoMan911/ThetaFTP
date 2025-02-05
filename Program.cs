using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using Serilog;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

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
                CertGenConfig.Result cert_result = await CertGenConfig.CertGen();

                if (configurations != null)
                {
                    if (cert_result == CertGenConfig.Result.None)
                        ThreadPool.SetMinThreads(configurations.min_worker_threads, configurations.min_input_output_threads);
                    else
                    {
                        Thread.Sleep(7000);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Thread.Sleep(7000);
                    Environment.Exit(0);
                }

                System.Timers.Timer server_utility_timer = new System.Timers.Timer();
                server_utility_timer.Elapsed += Server_utility_timer_Elapsed;
                server_utility_timer.Interval = 60000;
                server_utility_timer.Start();

                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddHttpClient(HttpClientConfig, client => {
                    int timeout = 600;
                    if (configurations != null)
                        timeout = configurations.ConnectionTimeoutSeconds;
                    client.Timeout = TimeSpan.FromSeconds(timeout);

                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        if (configurations.validate_ssl_certificates == true)
                        {
                            if (policyErrors == System.Net.Security.SslPolicyErrors.None)
                                return true;
                            else
                            {
                                if (policyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
                                {
                                    if (configurations.ensure_host_name_and_certificate_domain_name_match == false)
                                        return true;
                                    else
                                        return false;
                                }
                                else
                                    return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
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


                builder.WebHost.ConfigureKestrel((context, serverOptions) =>
                {
                    IPAddress? iPAddress = IPAddress.Loopback;

                    if (configurations != null)
                        if (configurations.server_ip_address != null && configurations.server_ip_address != "!!! REPLACE THE DESIRED SERVER IP ADDRESS !!!")
                            iPAddress = IPAddress.Parse(configurations.server_ip_address);

                    serverOptions.Listen(iPAddress, 8000, listenOptions =>
                    {
                        if (configurations != null && configurations.use_custom_ssl_certificate == true && configurations.custom_server_certificate_path != null)
                            listenOptions.UseHttps(configurations.custom_server_certificate_path, configurations.custom_server_certificate_password);
                        else
                            listenOptions.UseHttps();

                        listenOptions.UseHttps();
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                    });
                });

                var app = builder.Build();


                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");

                    if (configurations?.enforce_https == true)
                        app.UseHsts();
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
                Environment.Exit(1);
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
