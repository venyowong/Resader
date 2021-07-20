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

            Parallel.ForEach(feeds, feed => AsyncHelper.RunSync( async () =>
            {
                await this.service.Fetch(feed.Url);

                var latestTime = this.rssService.GetFeedLatestTime(feed.Id);
                var articles = await this.dao.GetArticles(feed.Id, latestTime);
                if (articles.IsNullOrEmpty())
                {
                    return;
                }

                this.rssService.SaveArticles(feed.Id, articles.ToList());
            }));
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
                .WithCronSchedule("* */5 * * * ?")
                .ForJob("FetchJob", "Resader")
                .Build();

            yield return TriggerBuilder.Create()
                .WithIdentity("FetchJob_RightNow", "Resader")
                .StartNow()
                .ForJob("FetchJob", "Resader")
                .Build();
        }
    }
}
