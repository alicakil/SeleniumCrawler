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
        public void ExecuteJob(PerformContext context)
        {
            try
            {
                // Initialize ---
                //Console.Write("Execute Job calisiyor");
                //Context c = new Context();
                //CurrentUser currentUser = new CurrentUser();

                //currentUser.Id = 4;
                //currentUser.Name = "Crawler";
                //UnitOfWork u = new UnitOfWork(c, currentUser);
                //// --------------

                //Console.Write("Instagram Crawler Initialize");
                //Instagram i = new Instagram(u);
                //i.CrawlWebSite();


                JobStorage
                .Current
                .GetConnection()
                .SetJobParameter(context.BackgroundJob.Id, "resultMessage", "Instagram Crawl Completed");
            }
            catch (Exception ex)
            {
                JobStorage
               .Current
               .GetConnection()
               .SetJobParameter(context.BackgroundJob.Id, "resultMessage", ex.Message);
            }

        }
    }

    public class HangfireAuthenticationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContent = context.GetHttpContext();

            if (!httpContent.User.Identity.IsAuthenticated)
                return false;

            var Role = httpContent.User.FindFirst("Role")?.Value;

            if (Role == "Admin")
                return httpContent.User.Identity.IsAuthenticated;

            return false;
        }
    }
}
