using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Resader.Factories
{
    public class RedisConnectionFactory
    {
        private Configuration config;
        private ConnectionMultiplexer connectionMultiplexer;

        public RedisConnectionFactory(IOptions<Configuration> config)
        {
            this.config = config.Value;
            if (!string.IsNullOrWhiteSpace(this.config.Redis?.ConnectionString))
            {
                connectionMultiplexer = ConnectionMultiplexer.Connect(this.config.Redis.ConnectionString);
            }
        }

        public IDatabase Create() => this.connectionMultiplexer?.GetDatabase(this.config.Redis.DefaultDb);
    }
}