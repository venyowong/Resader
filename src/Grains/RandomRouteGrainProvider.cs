using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;

namespace Resader.Grains
{
    public class RandomRouteGrainProvider : IRouteGrainProvider
    {
        private readonly IClusterClient cluserClient;

        public RandomRouteGrainProvider(IClusterClient cluserClient)
        {
            this.cluserClient = cluserClient;
        }

        public ValueTask<IGrain> GetGrain(Type grainType)
        {
            if (typeof(IGrainWithStringKey).IsAssignableFrom(grainType))
            {
                return new ValueTask<IGrain>(this.cluserClient.GetGrain(grainType, Guid.NewGuid().ToString()));
            }
            
            return new ValueTask<IGrain>(this.cluserClient.GetGrain(grainType, Guid.NewGuid()));
        }
    }
}