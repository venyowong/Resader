using System.Collections.Generic;
using Quartz;

namespace Resader.Api.Quartz;

public interface IScheduledJob
{
    /// <summary>
    /// 必须调用 JobBuilder.StoreDurably
    /// </summary>
    /// <returns></returns>
    IJobDetail GetJobDetail();

    /// <summary>
    /// 不同的触发规则需要返回不同的 Trigger
    /// </summary>
    /// <returns></returns>
    IEnumerable<ITrigger> GetTriggers();
}
