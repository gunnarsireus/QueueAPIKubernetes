using CronJobManager.Services.Interfaces;
using CronJobManager.Services.Models;
using CronJobManager.Services.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.IO;
using System.Reflection;
using CronJobManager.Services.Settings;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;

namespace CronJobManager
{
    public static class Program
    {

        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddSingleton(configuration.GetSection("Resources").Get<ServiceSettings>());
                    services.AddSingleton(configuration.GetSection("Jobs").Get<JobSettings>());
                    services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 }));
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                        // Add other logging providers as necessary
                    });

                    var connectionString = configuration.GetConnectionString("CronJobManager");
                    services.AddHangfire(c => c.UseSqlServerStorage(connectionString));
                    services.AddHangfireServer();
                    services.AddSingleton<IRestClientFactory, RestClientFactory>();
                    services.AddTransient<IJobService, JobService>();
                    services.AddTransient<IJobManager, JobManager>();
                    services.AddHostedService<CronJobManagerService>();

                    services.AddControllers();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure(app =>
                    {
                        var defaultDateCulture = "en-US";
                        var ci = new CultureInfo(defaultDateCulture);

                        app.UseRequestLocalization(new RequestLocalizationOptions
                        {
                            DefaultRequestCulture = new RequestCulture(ci),
                            SupportedCultures = [ci],
                            SupportedUICultures = [ci]
                        });

                        app.UseRouting();
                        app.UseHangfireDashboard(); // Add this line to use the Hangfire Dashboard on localhost:5000/hangfire

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHangfireDashboard();
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}
