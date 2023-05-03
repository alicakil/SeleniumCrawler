using Dashboard.Models;
using Dashboard.Repository;
using Dashboard.Services.Crawlers;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.Server;

namespace Dashboard.Services
{
    public class HangfireService
    {
        [AutomaticRetry(Attempts = 1)]
        [JobDisplayName("My Job")]
        public void ExecuteJob()
        {
			// Initialize ---
			Console.Write("Job Executing...");
			Context c = new Context();
			CurrentUser currentUser = new CurrentUser();

			currentUser.Id = 4;
			currentUser.Name = "Crawler";
			UnitOfWork u = new UnitOfWork(c, currentUser);
			// --------------

			Console.Write("Instagram Crawler Initialize");
			Instagram i = new Instagram();
			i.CrawlWebSite();
		}
    }

    public class HangfireAuthenticationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContent = context.GetHttpContext();

            return httpContent.User.Identity.IsAuthenticated;

		}
    }
}
