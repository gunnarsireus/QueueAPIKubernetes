using CronJobManager.Services.Interfaces;
using System.Threading.Tasks;

namespace CronJobManager
{
    public class JobManager : IJobManager
    {
        private readonly IJobService _jobService;

        public JobManager(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task ToggleAirJob()
        {
            await _jobService.TriggerJobToggle("AirJob").ConfigureAwait(true);
        }

        public async Task ToggleBoatJob()
        {
            await _jobService.TriggerJobToggle("BoatJob").ConfigureAwait(true);
        }

        public async Task ToggleSunJob()
        {
            await _jobService.TriggerJobToggle("SunJob").ConfigureAwait(true);
        }

        public async Task ToggleTruckJob()
        {
            await _jobService.TriggerJobToggle("TruckJob").ConfigureAwait(true);
        }
    }
}