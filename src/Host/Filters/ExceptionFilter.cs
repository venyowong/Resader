using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Resader.Host.Filters
{
    public class ExceptionFilter : IIncomingGrainCallFilter
    {
        private ILogger<ExceptionFilter> logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                await context.Invoke();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, string.Empty);
            }
        }
    }
}