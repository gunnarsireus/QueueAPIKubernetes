using CronJobManager.Services.Interfaces;
using RestSharp;

namespace CronJobManager.Services.Models
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient CreateClient(string baseUrl)
        {
            var options = new RestClientOptions(baseUrl);
            // Configure your options here
            return new RestClient(options);
        }
    }

}
