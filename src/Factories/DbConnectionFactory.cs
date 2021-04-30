using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Polly;
using Serilog;

namespace Resader.Factories
{
    public class DbConnectionFactory
    {
        private IConfiguration config;

        private IAsyncPolicy<MySqlConnection> mySqlConnectionPolicy = PollyPolicies.GetDbConnectionPolicy<MySqlConnection>();

        private IAsyncPolicy<SqlConnection> sqlConnectionPolicy = PollyPolicies.GetDbConnectionPolicy<SqlConnection>();

        public DbConnectionFactory(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<IDbConnection> CreateDbConnection(string clientType, string connectionName)
        {
            var key = $"{clientType}:ConnectionStrings:{connectionName}";
            var connectionString = this.config[key];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Log.Warning($"无法获取到 {clientType} {connectionName} 对应的连接字符串，key: {key}");
                return null;
            }

            switch (clientType.ToLower())
            {
                case "mysql":
                    return await this.CreateMySqlConnection(connectionString);
                case "sqlserver":
                    return await this.CreateSqlServerConnection(connectionString);
            }

            Log.Warning($"无法识别的数据库类型: {clientType}");
            return null;
        }

        public async Task<IDbConnection> CreateMySqlConnection(string connectionString)
        {
            return await this.mySqlConnectionPolicy.ExecuteAsync(async () =>
            {
                var conn = new MySqlConnection(connectionString);
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                return conn;
            });
        }

        public async Task<IDbConnection> CreateSqlServerConnection(string connectionString)
        {
            return await this.sqlConnectionPolicy.ExecuteAsync(async () =>
            {
                var conn = new SqlConnection(connectionString);
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }
                return conn;
            });
        }
    }
}