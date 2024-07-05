using RestSharp;

namespace CronJobManager.Services.Interfaces
{
    public interface IRestClientFactory
    {
        IRestClient CreateClient(string baseUrl);
    }

}
