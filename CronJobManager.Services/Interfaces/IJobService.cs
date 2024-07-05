using System.Threading.Tasks;

namespace CronJobManager.Services.Interfaces
{
    public interface IJobService
    {
        Task TriggerJobToggle(string jobType);
    }
}
