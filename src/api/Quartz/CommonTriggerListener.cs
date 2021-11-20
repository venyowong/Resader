using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Serilog;

namespace Resader.Api.Quartz;
public class CommonTriggerListener : ITriggerListener
{
    public string Name
    {
        get
        {
            return "CommonTriggerListener";
        }
    }

    public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
    {
        Log.Information(string.Format("Trigger: {0} Job: {1} TriggerComplete Runtime: {2}", trigger.Key, trigger.JobKey, context.JobRunTime));
        return Task.CompletedTask;
    }

    public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        Log.Information(string.Format("Trigger: {0} Job: {1} TriggerFired", trigger.Key, trigger.JobKey));
        return Task.CompletedTask;
    }

    public void TriggerMisfired(ITrigger trigger)
    {
    }

    public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        Log.Logger.Information(string.Format("Trigger: {0} Job: {1} TriggerMisfired", trigger.Key, trigger.JobKey));
        return Task.CompletedTask;
    }

    public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}
