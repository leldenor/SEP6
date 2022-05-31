using SEP6.Settings;
using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Common;
using System.IO;
using MySQL.Data.MySqlClient;

namespace SEP6 {

    // this is Program.cs code, must run on start

    public class MoveThisCode {

        public static AppSettings AppSettings { get; set; }

        public static void Main(string[] args) {
            StartupExtensions.InitializeDatabase();
            BuildWebHost(args).Build().Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args) {
            ReadAppSettings();

            return WebHost.CreateDefaultBuilder(args).UseGoogleDiagnostics(
                AppSettings.GoogleCloudSettings.ProjectId,
                AppSettings.GoogleCloudSettings.ServiceName,
                AppSettings.GoogleCloudSettings.Version)
                .UseStartup<Startup>().UsePortEnviromentVariable();
        }

        private static void ReadAppSettings() {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            AppSettings = new AppSettings();
            config.Bind(AppSettings);
        }

        static class ProgramExtensions {
            public static IWebHostBuilder UsePortEnviromentalVariable(this IWebHostBuilder builder) {
                string port = UsePortEnviromentalVariable().GetEnviromentVariable("PORT");
                if (!string.IsNullOrEmpty(port)) {
                    builder.UseUrls($"http://0.0.0.0:{port}");
                }
                return builder;
            }
        }
    }
}
