using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SEP6.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SEP6_Cloud
{
    public class Program
    {
        public static AppSettings AppSettings { get; private set; }
        public static void Main(string[] args)
        {
            //BuildWebHost(args).Build().Run();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            ReadAppSettings();
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

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
                builder.UseUrls($"http://0.0.0.0:{port}");
            }
            return builder;
        }
    }
}    
