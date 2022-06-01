using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Polly;
using SEP6.Authentication;
using SEP6.Serivces;
using SEP6.Services;
using SEP6.Settings;
using System.Data.Common;
using Google.Cloud.Diagnostics.AspNetCore;

namespace SEP6
{
    public class Program
    {
        public static AppSettings AppSettings { get; private set; }
        public static void Main(string[] args)
        {
            StartupExtensions.InitializeDatabase();
            BuildWebHost(args).Build().Run();
            //CreateHostBuilder(args).Build().Run();
        }


        /*public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });*/

        public static IWebHostBuilder BuildWebHost(string[] args)
        {
            ReadAppSettings();

            return WebHost.CreateDefaultBuilder(args)
                .UseGoogleDiagnostics(
                    AppSettings.googleCloudSettings.ProjectId,
                    AppSettings.googleCloudSettings.ServiceName,
                    AppSettings.googleCloudSettings.Version)
                .UseStartup<Startup>()
                .UsePortEnvironmentVariable();
        }

        private static void ReadAppSettings()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            AppSettings = new AppSettings();
            config.Bind(AppSettings);
        }
    }
    static class ProgramExtensions
    {
        public static IWebHostBuilder UsePortEnvironmentVariable(this IWebHostBuilder builder)
        {
            string port = Environment.GetEnvironmentVariable("PORT");
            if (string.IsNullOrEmpty(port))
            {
                builder.UseUrls("https://sep6-352012.appspot.com");
            }
            return builder;
        }
    }
}


