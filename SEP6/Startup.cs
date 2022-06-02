
using Microsoft.AspNetCore.Components.Authorization;
using MySql.Data.MySqlClient;
using Polly;
using SEP6.Authentication;
using SEP6.Serivces;
using SEP6.Services;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SEP6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddAntDesign();
            services.AddSingleton<IMovieService, MovieService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddScoped<AuthenticationStateProvider, AuthProvider>();
            services.AddSingleton(sp => StartupExtensions.GetMySqlConnectionString());
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(DbExceptionFilterAttribute));
            });
            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });*/
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
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
}
