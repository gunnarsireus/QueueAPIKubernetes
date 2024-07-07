using CronJobManager.Filter;
using CronJobManager.Services.Interfaces;
using CronJobManager.Services.Models;
using CronJobManager.Services.Services;
using CronJobManager.Services.Settings;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CronJobManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Ensure the database is created
            EnsureDatabase(configuration);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        IConfiguration configuration = context.Configuration;

                        services.AddSingleton(configuration.GetSection("Resources").Get<ServiceSettings>());
                        services.AddSingleton(configuration.GetSection("Jobs").Get<JobSettings>());
                        services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 }));
                        services.AddLogging(builder =>
                        {
                            builder.AddConsole();
                            // Add other logging providers as necessary
                        });

                        var connectionString = configuration.GetConnectionString("CronJobManagerDbConnection");
                        services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
                        services.AddHangfireServer();
                        services.AddSingleton<IRestClientFactory, RestClientFactory>();
                        services.AddTransient<IJobService, JobService>();
                        services.AddTransient<IJobManager, JobManager>();
                        services.AddHostedService<CronJobManagerService>();

                        services.AddControllers();
                    })
                    .Configure((context, app) =>
                    {
                        var defaultDateCulture = "en-US";
                        var ci = new CultureInfo(defaultDateCulture);

                        var localizationOptions = new RequestLocalizationOptions
                        {
                            DefaultRequestCulture = new RequestCulture(ci),
                            SupportedCultures = new[] { ci },
                            SupportedUICultures = new[] { ci }
                        };

                        app.UseRequestLocalization(localizationOptions);

                        app.UseHangfireDashboard("/hangfire", new DashboardOptions
                        {
                            Authorization = new[] { new AuthorizationFilter() }
                        });

                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHangfireDashboard();
                            endpoints.MapControllers();
                        });
                    });
                });

        private static void EnsureDatabase(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CronJobManagerDbConnection");

            // Connection string for master database to check and create the database
            var masterConnectionString = connectionString.Replace("Database=CronJobManagerDb;", "Database=master;");

            using (var connection = new SqlConnection(masterConnectionString))
            {
                try
                {
                    connection.Open();

                    var commandText = @"
                        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CronJobManagerDb')
                        BEGIN
                            CREATE DATABASE [CronJobManagerDb]
                        END";

                    using (var command = new SqlCommand(commandText, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Database 'CronJobManagerDb' ensured.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the database: {ex.Message}");
                }
            }
        }
    }
}
