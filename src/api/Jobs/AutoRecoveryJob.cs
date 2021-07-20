using Microsoft.Extensions.Options;
using Quartz;
using Resader.Api.Daos;
using Resader.Api.Quartz;
using Resader.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Jobs
{
    public class AutoRecoveryJob : IJob, IScheduledJob
    {
        private Configuration config;
        private UserDao userDao;
        private RssDao rssDao;
        private RssService rssService;

        public AutoRecoveryJob(IOptions<Configuration> config, UserDao userDao, 
            RssDao rssDao, RssService rssService)
        {
            this.config = config.Value;
            this.userDao = userDao;
            this.rssDao = rssDao;
            this.rssService = rssService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (!this.config.AutoRecovery)
            {
                return;
            }

            var users = await this.userDao.GetUsers();
            foreach (var u in users)
            {
                var records = await this.rssDao.GetReadRecords(u.Id);
                this.rssService.SaveReadRecords(u.Id, records.ToList());

                var browseRecords = await this.rssDao.GetFeedBrowseRecords(u.Id);
                this.rssService.SaveFeedBrowseRecords(u.Id, browseRecords.ToList());
            }
        }

        public IJobDetail GetJobDetail()
        {
            return JobBuilder.Create<AutoRecoveryJob>()
                .WithIdentity("AutoRecoveryJob", "Resader")
                .StoreDurably()
                .Build();
        }

        public IEnumerable<ITrigger> GetTriggers()
        {
            yield return TriggerBuilder.Create()
                .WithIdentity("AutoRecoveryJob_RightNow", "Resader")
                .StartNow()
                .ForJob("AutoRecoveryJob", "Resader")
                .Build();
        }
    }
}
