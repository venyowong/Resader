using Microsoft.Extensions.Options;
using Quartz;
using Resader.Api.Daos;
using Resader.Api.Quartz;
using Resader.Api.Services;
using Resader.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Jobs;
public class AutoRecoveryJob : IJob, IScheduledJob
{
    private Configuration config;
    private UserDao userDao;
    private RssDao rssDao;
    private RssService rssService;
    private RecommendService recommendService;

    public AutoRecoveryJob(IOptions<Configuration> config, UserDao userDao,
        RssDao rssDao, RssService rssService, RecommendService recommendService)
    {
        this.config = config.Value;
        this.userDao = userDao;
        this.rssDao = rssDao;
        this.rssService = rssService;
        this.recommendService = recommendService;
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

            var subscriptions = await this.rssDao.GetSubscriptions(u.Id);
            this.rssService.SaveSubscriptions(u.Id, subscriptions.ToList());
        }

        var feeds = (await this.rssDao.GetFeeds())?.ToList();
        if (!feeds.IsNullOrEmpty())
        {
            this.rssService.SaveFeeds(feeds);
            await this.recommendService.RefreshLabels(feeds);

            // 此处将所有文章加载到缓存中，因此数据量越大服务启动时间越长
            feeds.AsParallel().ForAll(feed =>
            {
                this.rssService.RefreshArticles(feed.Id).Wait();
            });
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
