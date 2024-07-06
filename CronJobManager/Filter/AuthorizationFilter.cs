using Hangfire.Dashboard;

namespace CronJobManager.Filter
{
    public class AuthorizationFilter: IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Allow all users to see the dashboard (not recommended for production)
            return true;

            // Implement your custom logic here
            // Example: Only allow local requests
            // var httpContext = context.GetHttpContext();
            // return httpContext.User.Identity.IsAuthenticated && httpContext.Connection.RemoteIpAddress.Equals(IPAddress.Loopback);
        }
    }
}
