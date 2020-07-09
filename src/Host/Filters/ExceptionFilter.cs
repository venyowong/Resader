using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Serilog;

namespace Resader.Host.Filters
{
    public class ExceptionFilter : IIncomingGrainCallFilter
    {
        public async Task Invoke(IIncomingGrainCallContext context)
        {
            try
            {
                await context.Invoke();
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
            }
        }
    }
}