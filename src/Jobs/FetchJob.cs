using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Resader.Daos;
using Resader.Extensions;
using Resader.Quartz;
using Resader.Services;

namespace Resader.Jobs
{
    public class FetchJob : IJob, IScheduledJob
    {
        private FetchService service;
        private RssDao dao;

        public FetchJob(RssDao dao, FetchService service)
        {
            this.dao = dao;
            this.service = service;
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            var feeds = await this.dao.GetFeeds();
            if (feeds.IsNullOrEmpty())
            {
                return;
            }

            var tasks = feeds.Select(feed => this.service.Fetch(feed.Url)).ToArray();
            Task.WaitAll(tasks);
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