using CronJobManager.Services.Settings;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CronJobManager
{
    public sealed class CronJobManagerService : IHostedService
    {
        private readonly IJobManager _jobManager;
        private readonly JobSettings _jobSettings;
        private readonly ILogger<CronJobManagerService> _logger;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly Task _completedTask = Task.CompletedTask;
        private BackgroundJobServer _server;

        private const string ToggleAirJob = "ToggleAirJobTrigger";
        private const string ToggleTruckJob = "ToggleBoatJobTrigger";
        private const string ToggleSunJob = "ToggleSunJobTrigger";
        private const string ToggleBoatJob = "ToggleTruckJobTrigger";

        public CronJobManagerService(ILogger<CronJobManagerService> logger, IRecurringJobManager recurringJobManager, JobSettings jobSettings, IJobManager jobManager)
        {
            _logger = logger;
            _recurringJobManager = recurringJobManager;
            _jobSettings = jobSettings;
            _jobManager = jobManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _server = new BackgroundJobServer(new BackgroundJobServerOptions()
            {
                ServerTimeout = TimeSpan.FromMinutes(10)
            });

            _logger.LogInformation($"Starting CronJobManagerService");

            AddOrRemoveJob(ToggleAirJob, _jobSettings.ToggleAirJob.CronValue, () => _jobManager.ToggleAirJob(), _jobSettings.ToggleSunJob.Enabled);
            AddOrRemoveJob(ToggleBoatJob, _jobSettings.ToggleBoatJob.CronValue, () => _jobManager.ToggleBoatJob(), _jobSettings.ToggleBoatJob.Enabled);
            AddOrRemoveJob(ToggleSunJob, _jobSettings.ToggleSunJob.CronValue, () => _jobManager.ToggleSunJob(), _jobSettings.ToggleSunJob.Enabled);
            AddOrRemoveJob(ToggleTruckJob, _jobSettings.ToggleTruckJob.CronValue, () => _jobManager.ToggleTruckJob(), _jobSettings.ToggleBoatJob.Enabled);

            return _completedTask;
        }

        private void AddOrRemoveJob(string jobId, string cronValue, Expression<Func<Task>> task, bool enabled)
        {
            if (enabled)
            {
                _recurringJobManager.AddOrUpdate(jobId, task, cronValue);
            }
            else
            {
                _recurringJobManager.RemoveIfExists(jobId);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping CronJobManagerService");
            _server.Dispose();

            return _completedTask;
        }
    }
}