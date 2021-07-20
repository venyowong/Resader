using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Polly;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Factories
{
    public class DbConnectionFactory
    {
        private Configuration config;
        private IAsyncPolicy<MySqlConnection> mySqlConnectionPolicy = PollyPolicies.GetDbConnectionPolicy<MySqlConnection>();

        public DbConnectionFactory(IOptions<Configuration> config)
        {
            this.config = config.Value;
        }

        public async Task<IDbConnection> Create()
        {
            return await this.mySqlConnectionPolicy.ExecuteAsync(async () =>
            {
                var conn = new MySqlConnection(this.config.MySql?.ConnectionString);
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                return conn;
            });
        }
    }
}
