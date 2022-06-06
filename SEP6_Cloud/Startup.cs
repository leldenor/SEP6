
using Microsoft.AspNetCore.Components.Authorization;
using Polly;
using SEP6.Services;
using SEP6_Cloud.Authentication;
using SEP6_Cloud.Serivces;
using System.Data.Common;
using System.Data.SqlClient;

namespace SEP6_Cloud
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
            services.AddScoped<AuthenticationStateProvider, AuthProvider>();
            services.AddSingleton<AccountService>();
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
                .Handle<SqlException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                })
                .Execute(() => connection.Open());
        // [END cloud_sql_mysql_dotnet_ado_backoff]

        public static SqlConnectionStringBuilder GetMySqlConnectionString()
        {
            SqlConnection myConnection = new SqlConnection();
            SqlConnectionStringBuilder myBuilder = new SqlConnectionStringBuilder();

            myBuilder.UserID = "root";

            myBuilder.Password = "1357908642";

            myBuilder.InitialCatalog = "egg6";

            myBuilder.DataSource = "34.118.87.40";

            myBuilder.ConnectTimeout = 30;

            myConnection.ConnectionString = myBuilder.ConnectionString;

            return myBuilder;
        }
    }
}
