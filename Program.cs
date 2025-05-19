using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using ThetaFTP.Shared.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Server.Kestrel;

namespace ThetaFTP
{
    public class Program:Shared.Shared
    {
        public static void Main(string[] args)
        {
            Logging.Init();
            _ = Main_OP(args).Result;
        }

        private static async Task<bool> Main_OP(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                ServerConfigModel? model = (await ServerConfig.GetServerConfig(builder.Environment.IsDevelopment()))?.ServerConfigModel;
                CertGenConfig.Result cert_result = await CertGenConfig.CertGen();
                FileEncryptionKeyGen.Result key_result = await FileEncryptionKeyGen.KeyGen();

                if (cert_result == CertGenConfig.Result.None)
                {
                    if (key_result == FileEncryptionKeyGen.Result.None)
                    {
                        if (model != null)
                        {
                            if (model.use_google_secrets == true)
                            {
                                GoogleSecretsManager googleSecretsManager = new GoogleSecretsManager();
                                Dictionary<GoogleSecretsManager.SecretType, string?> secrets = await googleSecretsManager.GetSecrets(model);

                                string? server_salt_secret = null;
                                secrets.TryGetValue(GoogleSecretsManager.SecretType.HashSalt, out server_salt_secret);

                                string? firebase_admin_token_secret = null;
                                secrets.TryGetValue(GoogleSecretsManager.SecretType.FirebaseAdminToken, out firebase_admin_token_secret);

                                string? mysql_user_password_secret = null;
                                secrets.TryGetValue(GoogleSecretsManager.SecretType.MySqlPassword, out mysql_user_password_secret);

                                string? smtp_password_secret = null;
                                secrets.TryGetValue(GoogleSecretsManager.SecretType.SmtpPassword, out smtp_password_secret);

                                string? custom_server_certificate_password_secret = null;
                                secrets.TryGetValue(GoogleSecretsManager.SecretType.SslCertificatePassword, out custom_server_certificate_password_secret);

                                string? aes_encryption_key_secret = null;
                                secrets.TryGetValue(GoogleSecretsManager.SecretType.AesEncryptionKey, out aes_encryption_key_secret);

                                model.server_salt = server_salt_secret;
                                model.firebase_admin_token = firebase_admin_token_secret;
                                model.mysql_user_password = mysql_user_password_secret;
                                model.smtp_password = smtp_password_secret;
                                model.custom_server_certificate_password = custom_server_certificate_password_secret;

                                if (model.use_file_encryption == true)
                                {
                                    if (await AesKeyLoadup.LoadAesKeyFromValue(aes_encryption_key_secret) == false)
                                    {
                                        Console.WriteLine("Invalid Google Cloud credentials or corrupt AES key.\nCheck if Google Secrets API url is valid\nor use the command:\n 'gcloud auth application-default login'");
                                        throw new Exception("Corrupted AES key");
                                    }
                                }
                            }
                            else
                            {
                                if (model.use_file_encryption == true)
                                {
                                    if (await AesKeyLoadup.LoadAesKeyFromFile(model?.aes_encryption_key_location) == false)
                                    {
                                        Console.WriteLine("Corrupt AES key. Check if the specified path to the key is valid.");
                                        throw new Exception("Corrupted AES key");
                                    }
                                }
                            }

                            sha512 = new Sha512Hasher(model?.server_salt);

                            configurations = model;

                            System.Timers.Timer server_utility_timer = new System.Timers.Timer();
                            server_utility_timer.Elapsed += Server_utility_timer_Elapsed;
                            server_utility_timer.Interval = 60000;
                            server_utility_timer.Start();



                            builder.Services.AddRateLimiter((options) => {
                                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext => 
                                RateLimitPartition.GetSlidingWindowLimiter(partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                                factory: partition => new SlidingWindowRateLimiterOptions
                                {
                                    PermitLimit = model != null ? model.maximum_number_of_requests_per_minute : 10000,
                                    Window = TimeSpan.FromMinutes(1),
                                    SegmentsPerWindow = 4,
                                    QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
                                    QueueLimit = model != null ? model.maximum_number_of_queued_requests : 10000,
                                    AutoReplenishment = true
                                }));


                                options.OnRejected = async (context, cancellationToken) =>
                                {
                                    Logging.Message("Rate limit exceeded", $"The API rate limit was exceeded for an IP address: {context.HttpContext.Connection.RemoteIpAddress}", "Rate limit exceeded", "Program", "Main_OP", Logging.LogType.Error);

                                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                                    context.HttpContext.Response.Headers["Retry-After"] = "60";
                                    await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);
                                };
                            });


                            builder.Services.AddServerSideBlazor(option =>
                            {
                                option.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(2);
                                option.MaxBufferedUnacknowledgedRenderBatches = 15;
                            });

                            builder.Services.AddRazorComponents()
                                            .AddInteractiveServerComponents()
                                            .AddHubOptions(options =>
                                            {
                                                options.MaximumReceiveMessageSize = 10 * 1024 * 1024; 
                                            });

                            builder.Services.AddHttpClient(HttpClientConfig, client => {
                                int timeout = 600;
                                if (model != null)
                                    timeout = model.ConnectionTimeoutSeconds;
                                client.Timeout = TimeSpan.FromSeconds(timeout);

                            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                            {
                                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) =>
                                {
                                    if (model?.validate_ssl_certificates == true)
                                    {
                                        if (policyErrors == System.Net.Security.SslPolicyErrors.None)
                                            return true;
                                        else
                                        {
                                            Logging.Message("SSL certificate validation failed", $"SSL policyErrors: {policyErrors.ToString()}", "The public server certificate has some SSL policy errors. Check if the DN and CN match the server's web address", "Program", "Main_OP", Logging.LogType.Error);

                                            if (policyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors && certChain != null)
                                                if (model.validate_ssl_certificate_chain == true)
                                                {
                                                    foreach (X509ChainStatus status in certChain.ChainStatus)
                                                    {
                                                        Logging.Message("SSL certificate validation failed", $"SSL chain policyErrors: {status.Status}", "The public server certificate has some SSL policy errors. Check if the DN and CN match the server's web address", "Program", "Main_OP", Logging.LogType.Error);
                                                    }

                                                    return false;
                                                }
                                                else
                                                    return true;

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
                            }).SetHandlerLifetime(TimeSpan.FromSeconds(model != null ? model.ConnectionTimeoutSeconds : 600));

                            builder.Services.AddRazorPages();
                            builder.Services.AddServerSideBlazor();
                            builder.Services.AddMvc();
                            builder.Services.AddControllers(options =>
                            {
                                options.InputFormatters.Add(new StreamFormatter());
                                options.InputFormatters.Add(new JsonTextInputFormatter());
                                options.OutputFormatters.Add(new StreamOutputFormatter());
                                options.OutputFormatters.Add(new GzipStreamOutputFormatter());
                            });

                            int connection_timeout = 600;
                            if (model != null)
                                connection_timeout = model.ConnectionTimeoutSeconds;


                            if (model?.enforce_https == true)
                                builder.Services.AddHsts(option =>
                                {
                                    option.MaxAge = TimeSpan.FromDays(model.hsts_max_age_days);
                                    option.Preload = true;
                                    option.IncludeSubDomains = true;
                                });


                            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
                            {
                                if (model != null)
                                {
                                    IConfigurationSection kestrel_config = context.Configuration.GetSection("Kestrel");

                                    KestrelConfigurationLoader https = serverOptions.Configure(kestrel_config).Endpoint("Address", listenOptions => {
                                        if (model != null && model.use_custom_ssl_certificate == true && model.custom_server_certificate_path != null)
                                        {
                                            listenOptions.ListenOptions.UseHttps(model.custom_server_certificate_path, model.custom_server_certificate_password);
                                        }

                                        listenOptions.ListenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                                    });

                                    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(connection_timeout);
                                    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(connection_timeout);
                                    serverOptions.Limits.MaxRequestBodySize = model.max_request_buffer_size;
                                    serverOptions.Limits.MaxResponseBufferSize = model.max_response_buffer_size;
                                    serverOptions.Limits.MaxRequestBufferSize = model.max_request_buffer_size > 10 * 1024 * 1024 * 2 ? model.max_request_buffer_size : 10 * 1024 * 1024 * 2;
                                    serverOptions.Limits.MaxConcurrentConnections = model.max_concurent_connections;
                                }
                            });

                            var app = builder.Build();

                            app.Use(async (context, next) =>
                            {
                                context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");   
                                context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
                                await next.Invoke();
                            });


                            if (model?.is_reverse_proxy == true)
                                app.UseForwardedHeaders();

                            if (!app.Environment.IsDevelopment())
                            {
                                app.UseExceptionHandler("/Error");

                                if (model?.enforce_https == true)
                                    app.UseHsts();
                            }

                            app.UseRateLimiter();

                            app.MapControllers();
                            app.UseHttpsRedirection();

                            app.UseStaticFiles();

                            app.UseRouting();

                            app.MapBlazorHub();
                            app.MapFallbackToPage("/_Host");

                            app.MapRazorPages().RequireRateLimiting("sliding_window");
                            app.MapDefaultControllerRoute().RequireRateLimiting("sliding_window");

                            app.UseCors();

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
                else
                {
                    Thread.Sleep(7000);
                    Environment.Exit(0);
                }

            }
            catch(Exception e)
            {
                Logging.Message(e, "Fatal error", "", "Program", "Main_OP", Logging.LogType.Fatal);
                Environment.Exit(1);
            }
            finally
            {
                Logging.FlushLogs();
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
