using Quartz;
using Resader.Api.Daos;
using Resader.Common.Extensions;
using Resader.Api.Helpers;
using Resader.Api.Quartz;
using Resader.Api.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Jobs
{
    public class FetchJob : IJob, IScheduledJob
    {
        private FetchService service;
        private RssDao dao;
        private RssService rssService;

        public FetchJob(RssDao dao, FetchService service, RssService rssService)
        {
            this.dao = dao;
            this.service = service;
            this.rssService = rssService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var feeds = await this.dao.GetFeeds();
            if (feeds.IsNullOrEmpty())
            {
                return;
            }

            feeds.AsParallel().ForAll(feed =>
            {
                var result = this.service.Fetch(feed.Url);
                if (result.Feed != null && !result.Articles.IsNullOrEmpty())
                {
                    this.rssService.AddArticles(result.Feed.Id, result.Articles).Wait();
                }
            });
        }

        public IJobDetail GetJobDetail()
        {
            return JobBuilder.Create<FetchJob>()
                .WithIdentity("FetchJob", "Resader")
                .StoreDurably()
                .Build();
        }

        public IEnumerable<ITrigger> GetTriggers()
        {
            yield return TriggerBuilder.Create()
                .WithIdentity("FetchJob_Trigger1", "Resader")
                .WithCronSchedule("0 */1 * * * ?")
                .ForJob("FetchJob", "Resader")
                .Build();
        }
    }
}
