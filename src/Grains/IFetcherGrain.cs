using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;
using Resader.Grains.Models;

namespace Resader.Grains
{
    public interface IFetcherGrain : IGrainWithStringKey
    {
        [HttpPost("fetcher/fetch", routeGrainProviderPolicy: nameof(FixedRouteGrainProvider))]
        Task<Result> Fetch();
    }
}