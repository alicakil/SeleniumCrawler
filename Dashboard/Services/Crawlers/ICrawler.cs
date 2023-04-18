using Hangfire;

namespace Dashboard.Services.Crawlers
{
    internal interface ICrawler
    {
        [AutomaticRetry(Attempts = 2)]
        public void CrawlWebSite();
    }
}
