using System;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Microsoft.Extensions.Logging;
using CronJobManager.Services.Interfaces;
using CronJobManager.Services.Settings;

namespace CronJobManager.Services.Services
{
    public class JobService : IJobService
    {
        private readonly IRestClientFactory _restClientFactory;
        private IRestClient _restClient;
        private readonly ServiceSettings _serviceSettings;
        private readonly ILogger _logger;
        private const int TimeoutInMilliseconds = 10000; // 10 seconds

        public JobService(IRestClientFactory restClientFactory, ServiceSettings serviceSettings, ILoggerFactory loggerFactory)
        {
            _restClientFactory = restClientFactory;
            _serviceSettings = serviceSettings;
            _logger = loggerFactory.CreateLogger<JobService>();
        }

        public async Task TriggerJobToggle(string jobName)
        {
            _logger.LogInformation($"TriggerJobToggle {jobName}");
            try
            {
                var config = _serviceSettings.Resources.FirstOrDefault(x => x.Id == ServiceConstants.CAR_API_ID);
                var request = new RestRequest(ServiceConstants.RESOURCE_JOB_TOGGLE)
                {
                    Timeout = TimeoutInMilliseconds
                }
                .AddQueryParameter("jobName", jobName);

                _restClient = _restClientFactory.CreateClient(config.Url);
                var response = await _restClient.GetAsync(request);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Trigger 'TriggerJobToggle' was not successful. {response.StatusDescription} {response.Content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}