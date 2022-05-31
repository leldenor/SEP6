using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Polly;
using SEP6.Serivces;
using SEP6.Services;
using SEP6.Settings;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntDesign();
builder.Services.AddSingleton<IMovieService, MovieService>();
builder.Services.AddSingleton(sp => StartupExtensions.GetMySqlConnectionString());
builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(DbExceptionFilterAttribute));
});
builder.Services.AddMvc(option => option.EnableEndpointRouting = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMvc(routes => {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
StartupExtensions.InitializeDatabase();
BuildWebHost(args).Build().Run();

private static AppSettings AppSettings { get; private set; }

static IWebHostBuilder BuildWebHost(string[] args)
{
    ReadAppSettings();

    return WebHost.CreateDefaultBuilder(args)
        .UseGoogleDiagnostics(
            AppSettings.GoogleCloudSettings.ProjectId,
            AppSettings.GoogleCloudSettings.ServiceName,
            AppSettings.GoogleCloudSettings.Version)
        .UseStartup<Startup>()
        .UsePortEnvironmentVariable();
}

static void ReadAppSettings()
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    AppSettings = new AppSettings();
    config.Bind(AppSettings);
}

app.Run();

static class StartupExtensions
{
    public static void OpenWithRetry(this DbConnection connection) =>
        // [START cloud_sql_mysql_dotnet_ado_backoff]
        Policy
            .Handle<MySqlException>()
            .WaitAndRetry(new[]
            {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
            })
            .Execute(() => connection.Open());
    public static void InitializeDatabase()
    {
        var connectionString = GetMySqlConnectionString();
        using (DbConnection connection = new MySqlConnection(connectionString.ConnectionString))
        {
            connection.OpenWithRetry();
            using (var createTableCommand = connection.CreateCommand())
            {

                //Fill this with MySql command
                createTableCommand.CommandText = @"
                        CREATE TABLE IF NOT EXISTS
                        
                        )";
                createTableCommand.ExecuteNonQuery();
            }
        }
    }
    public static MySqlConnectionStringBuilder GetMySqlConnectionString()
    {
        MySqlConnectionStringBuilder connectionString;
        if (Environment.GetEnvironmentVariable("INSTANCE_HOST") != null)
        {
            connectionString = SQLTCPConnectioncs.NewMysqlTCPConnectionString(); ;
        }
        else
        {
            connectionString = SQLUnixConnection.NewMysqlUnixSocketConnectionString();
        }
        // The values set here are for demonstration purposes only. You 
        // should set these values to what works best for your application.
        // [START cloud_sql_mysql_dotnet_ado_limit]
        // MaximumPoolSize sets maximum number of connections allowed in the pool.
        connectionString.MaximumPoolSize = 5;
        // MinimumPoolSize sets the minimum number of connections in the pool.
        connectionString.MinimumPoolSize = 0;
        // [END cloud_sql_mysql_dotnet_ado_limit]
        // [START cloud_sql_mysql_dotnet_ado_timeout]
        // ConnectionTimeout sets the time to wait (in seconds) while
        // trying to establish a connection before terminating the attempt.
        connectionString.ConnectionTimeout = 15;
        // [END cloud_sql_mysql_dotnet_ado_timeout]
        // [START cloud_sql_mysql_dotnet_ado_lifetime]
        // ConnectionLifeTime sets the lifetime of a pooled connection
        // (in seconds) that a connection lives before it is destroyed
        // and recreated. Connections that are returned to the pool are
        // destroyed if it's been more than the number of seconds
        // specified by ConnectionLifeTime since the connection was
        // created. The default value is zero (0) which means the
        // connection always returns to pool.
        connectionString.ConnectionLifeTime = 1800; // 30 minutes
                                                    // [END cloud_sql_mysql_dotnet_ado_lifetime]
        return connectionString;
    }
}

