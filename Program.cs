using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using Serilog;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using ThetaFTP.Shared.Models;

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
                ServerConfigModel? model = await ServerConfig.GetServerConfig();
                CertGenConfig.Result cert_result = await CertGenConfig.CertGen();

                if (cert_result == CertGenConfig.Result.None)
                {
                    if (model != null)
                    {
                        if (model.use_google_secrets == true)
                        {
                            GoogleSecretsManager googleSecretsManager = new GoogleSecretsManager();
                            Dictionary<GoogleSecretsManager.SecretType, string?> secrets = await googleSecretsManager.GetSecrets(model);

                            string? server_salt_secret_url = null;
                            secrets.TryGetValue(GoogleSecretsManager.SecretType.HashSalt, out server_salt_secret_url);

                            string? firebase_admin_token_secret_url = null;
                            secrets.TryGetValue(GoogleSecretsManager.SecretType.FirebaseAdminToken, out firebase_admin_token_secret_url);

                            string? mysql_user_password_secret_url = null;
                            secrets.TryGetValue(GoogleSecretsManager.SecretType.MySqlPassword, out mysql_user_password_secret_url);

                            string? smtp_password_secret_url = null;
                            secrets.TryGetValue(GoogleSecretsManager.SecretType.SmtpPassword, out smtp_password_secret_url);

                            string? custom_server_certificate_password_secret_url = null;
                            secrets.TryGetValue(GoogleSecretsManager.SecretType.SslCertificatePassword, out custom_server_certificate_password_secret_url);

                            model.server_salt_secret_url = server_salt_secret_url;
                            model.firebase_admin_token_secret_url = firebase_admin_token_secret_url;
                            model.mysql_user_password_secret_url = mysql_user_password_secret_url;
                            model.smtp_password_secret_url = smtp_password_secret_url;
                            model.custom_server_certificate_password_secret_url = custom_server_certificate_password_secret_url;
                        }

                        sha512 = new Sha512Hasher(model.server_salt);

                        configurations = model;

                        System.Timers.Timer server_utility_timer = new System.Timers.Timer();
                        server_utility_timer.Elapsed += Server_utility_timer_Elapsed;
                        server_utility_timer.Interval = 60000;
                        server_utility_timer.Start();

                        var builder = WebApplication.CreateBuilder(args);

                        builder.Services.AddHttpClient(HttpClientConfig, client => {
                            int timeout = 600;
                            if (model != null)
                                timeout = model.ConnectionTimeoutSeconds;
                            client.Timeout = TimeSpan.FromSeconds(timeout);

                        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                            {
                                if (model?.validate_ssl_certificates == true)
                                {
                                    if (policyErrors == System.Net.Security.SslPolicyErrors.None)
                                        return true;
                                    else
                                    {
                                        if (policyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
                                        {
                                            if (model.ensure_host_name_and_certificate_domain_name_match == false)
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
                        }).SetHandlerLifetime(TimeSpan.FromSeconds(model.ConnectionTimeoutSeconds));

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
                        if (model != null)
                            connection_timeout = model.ConnectionTimeoutSeconds;


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

                        if (model?.enforce_https == true)
                            builder.Services.AddHsts(option =>
                            {
                                option.MaxAge = TimeSpan.FromDays(double.MaxValue);
                                option.Preload = true;
                                option.IncludeSubDomains = true;
                            });

                        if (model?.DebugMode == false)
                        {
                            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
                            {
                                IPAddress? iPAddress = IPAddress.Loopback;

                                if (model != null)
                                    if (model.server_ip_address != null)
                                        iPAddress = IPAddress.Parse(model.server_ip_address);

                                serverOptions.Listen(iPAddress, 8000, listenOptions =>
                                {
                                    if (model != null && model.use_custom_ssl_certificate == true && model.custom_server_certificate_path != null)
                                        listenOptions.UseHttps(model.custom_server_certificate_path, model.custom_server_certificate_password);
                                    else
                                        listenOptions.UseHttps();

                                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                                });
                            });
                        }

                        var app = builder.Build();


                        if (!app.Environment.IsDevelopment())
                        {
                            app.UseExceptionHandler("/Error");

                            if (model?.enforce_https == true)
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
