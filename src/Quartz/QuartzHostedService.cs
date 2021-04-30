using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using Serilog;

namespace Resader.Quartz
{
    public class QuartzHostedService : IHostedService
    {
        private StdSchedulerFactory schedulerFactory;
        private IScheduler scheduler;
        private IEnumerable<IScheduledJob> jobs;
        private IJobFactory jobFactory;

        public QuartzHostedService(IEnumerable<IScheduledJob> jobs, IJobFactory jobFactory)
        {
            this.schedulerFactory = new StdSchedulerFactory();
            this.jobs = jobs;
            this.jobFactory = jobFactory;
            Log.Debug("QuartzHostedService inited.");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.scheduler = await this.schedulerFactory.GetScheduler();
            this.scheduler.JobFactory = this.jobFactory;
            this.scheduler.ListenerManager.AddTriggerListener(new CommonTriggerListener(), GroupMatcher<TriggerKey>.AnyGroup());
            await this.scheduler.Start();
            Log.Debug("QuartzHostedService.Scheduler started.");

            if (this.jobs == null || !this.jobs.Any())
            {
                Log.Warning("QuartzHostedService no jobs found.");
                return;
            }

            foreach (var job in this.jobs)
            {
                var jobDetail = job.GetJobDetail();
                if (jobDetail != null)
                {
                    await this.scheduler.AddJob(jobDetail, true);
                    Log.Debug(string.Format("Add job({0}) into the scheduler.", jobDetail.Key));

                    var triggers = job.GetTriggers();
                    if (triggers != null)
                    {
                        foreach (var trigger in triggers)
                        {
                            await this.scheduler.ScheduleJob(trigger);
                            Log.Debug(string.Format("Schedule the job({0}) with trigger({1})", trigger.JobKey, GetTriggerDesc(trigger)));
                        }
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.scheduler.Shutdown();
        }

        private string GetTriggerDesc(ITrigger trigger)
        {
            if (trigger == null)
            {
                return string.Empty;
            }

            if (trigger is CronTriggerImpl)
            {
                var cronTrigger = trigger as CronTriggerImpl;
                return string.Format("Key: {0} Cron: {1}", cronTrigger.Key, cronTrigger.CronExpressionString);
            }
            else if (trigger is SimpleTriggerImpl)
            {
                var simpleTrigger = trigger as SimpleTriggerImpl;
                return string.Format("Key: {0} Interval: {1} Count: {2}", simpleTrigger.Key, simpleTrigger.RepeatInterval, simpleTrigger.RepeatCount);
            }
            else
            {
                return string.Format("Key: {0}", trigger.Key);
            }
        }
    }
}