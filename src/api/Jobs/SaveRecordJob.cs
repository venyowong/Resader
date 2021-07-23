using Quartz;
using Resader.Api.Daos;
using Resader.Api.Quartz;
using Resader.Api.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class SaveRecordJob : IJob, IScheduledJob
    {
        private UserDao userDao;
        private RssDao rssDao;
        private RssService rssService;

        public SaveRecordJob(UserDao userDao, RssDao rssDao, RssService rssService)
        {
            this.userDao = userDao;
            this.rssDao = rssDao;
            this.rssService = rssService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var timeStr = string.Empty;
            if (File.Exists("SaveRecordTime.txt"))
            {
                timeStr = File.ReadAllText("SaveRecordTime.txt");
            }
            DateTime.TryParse(timeStr, out var time);
            var users = await this.userDao.GetUsers();
            foreach (var u in users)
            {
                #region 阅读记录
                var readRecords = this.rssService.GetReadRecords(u.Id);
                foreach (var r in readRecords.Where(x => time == default ? true : x.CreateTime >= time.AddMinutes(-10)))
                {
                    if ((await this.rssDao.GetReadRecord(r.UserId, r.ArticleId)) == null)
                    {
                        await this.rssDao.InsertReadRecord(r);
                    }
                    else
                    {
                        await this.rssDao.UpdateReadRecord(r);
                    }
                }
                #endregion

                #region feed 浏览记录
                var browseRecords = this.rssService.GetFeedBrowseRecords(u.Id);
                foreach (var r in browseRecords.Where(x => time == default ? true : x.CreateTime >= time.AddMinutes(-10)))
                {
                    if ((await this.rssDao.GetFeedBrowseRecord(r.UserId, r.FeedId)) == null)
                    {
                        await this.rssDao.InsertFeedBrowseRecord(r);
                    }
                    else
                    {
                        await this.rssDao.UpdateFeedBrowseRecord(r);
                    }
                }
                #endregion
            }

            File.WriteAllText("SaveRecordTime.txt", DateTime.Now.ToString());
        }

        public IJobDetail GetJobDetail()
        {
            return JobBuilder.Create<SaveRecordJob>()
                .WithIdentity("SaveReadRecordJob", "Resader")
                .StoreDurably()
                .Build();
        }

        public IEnumerable<ITrigger> GetTriggers()
        {
            yield return TriggerBuilder.Create()
                .WithIdentity("SaveReadRecordJob_Trigger1", "Resader")
                .WithCronSchedule("0 */5 * * * ?")
                .ForJob("SaveReadRecordJob", "Resader")
                .Build();

            yield return TriggerBuilder.Create()
                .WithIdentity("SaveReadRecordJob_RightNow", "Resader")
                .StartNow()
                .ForJob("SaveReadRecordJob", "Resader")
                .Build();
        }
    }
}
