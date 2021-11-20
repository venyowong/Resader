using System;
using Quartz;
using Quartz.Spi;

namespace Resader.Api.Quartz;

public class CustomJobFactory : IJobFactory
{
    private IServiceProvider serviceProvider;

    public CustomJobFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        return this.serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
    }

    public void ReturnJob(IJob job)
    {
        var disposable = job as IDisposable;
        disposable?.Dispose();
    }
}
