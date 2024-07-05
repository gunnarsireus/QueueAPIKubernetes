using System.Threading.Tasks;

namespace CronJobManager
{
    public interface IJobManager
    {
        Task ToggleAirJob();

        Task ToggleBoatJob();

        Task ToggleSunJob();

        Task ToggleTruckJob();
    }
}
