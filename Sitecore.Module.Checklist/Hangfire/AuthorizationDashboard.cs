using Hangfire.Dashboard;
using Microsoft.Owin;

namespace Sitecore.Module.Checklist.Hangfire
{
    public class AuthorizationDashboard : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var owinContext = new OwinContext(context.GetOwinEnvironment());

            return Context.User.IsAuthenticated;
        }
    }
}
