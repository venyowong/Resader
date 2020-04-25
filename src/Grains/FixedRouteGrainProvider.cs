using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;

namespace Resader.Grains
{
    public class FixedRouteGrainProvider : IRouteGrainProvider
    {
        private readonly IClusterClient cluserClient;

        private static readonly Guid _fixedGuid = Guid.NewGuid();

        public FixedRouteGrainProvider(IClusterClient cluserClient)
        {
            this.cluserClient = cluserClient;
        }

        public ValueTask<IGrain> GetGrain(Type grainType)
        {
            if (typeof(IGrainWithStringKey).IsAssignableFrom(grainType))
            {
                return new ValueTask<IGrain>(this.cluserClient.GetGrain(grainType, _fixedGuid.ToString()));
            }
            
            return new ValueTask<IGrain>(this.cluserClient.GetGrain(grainType, _fixedGuid));
        }
    }
}