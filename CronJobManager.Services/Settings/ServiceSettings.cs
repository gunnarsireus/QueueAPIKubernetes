using System.Collections.Generic;

namespace CronJobManager.Services.Settings
{
    public static class ServiceConstants
    {
        public static string CAR_API_ID = "car_api";

        public static string RESOURCE_JOB_TOGGLE = "jobstatus/toggle";

    }
    public class ServiceSettings
    {
        public List<Resource> Resources { get; set; }
    }

    public class Resource
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }

    public class JobSettings
    {
        public JobData ToggleAirJob { get; set; }
        public JobData ToggleBoatJob { get; set; }
        public JobData ToggleSunJob { get; set; }
        public JobData ToggleTruckJob { get; set; }
    }

    public class JobData
    {
        public string CronValue { get; set; }
        public bool Enabled { get; set; }
    }

}