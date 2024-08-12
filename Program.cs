using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ThetaFTP.Data;
using ThetaFTP.Shared;
using ThetaFTP.Shared.Classes;

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
            server_utility_timer.Interval = 100;
            server_utility_timer.Start();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
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
            await Shared.Shared.database_auth_validation.Delete(null);
        }
    }
}
