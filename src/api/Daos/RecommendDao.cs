using Resader.Api.Extensions;
using Resader.Api.Factories;
using Resader.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Daos
{
    public class RecommendDao
    {
        private DbConnectionFactory connectionFactory;

        static RecommendDao()
        {
            Utility.MakeDapperMapping(typeof(Recommend));
        }

        public RecommendDao(DbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<List<Recommend>> GetRecommends()
        {
            using var connection = await connectionFactory.Create();
            return (await connection.QueryWithPolly<Recommend>("SELECT * FROM recommend"))?.ToList();
        }

        public async Task<bool> InsertRecommend(string label, string feedId)
        {
            using var connection = await connectionFactory.Create();
            return await connection.ExecuteWithPolly("INSERT INTO recommend(label, feed_id, create_time, update_time) VALUES(@label, @feedId, now(), now())", new { label, feedId });
        }

        public async Task<bool> DeleteRecommend(string label, string feedId)
        {
            using var connection = await connectionFactory.Create();
            return await connection.ExecuteWithPolly("DELETE FROM recommend WHERE label=@label AND feed_id=@feedId", new { label, feedId });
        }
    }
}
