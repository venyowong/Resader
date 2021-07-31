using Resader.Api.Extensions;
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
        private IDbConnection connection;

        static RecommendDao()
        {
            Utility.MakeDapperMapping(typeof(Recommend));
        }

        public RecommendDao(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<List<Recommend>> GetRecommends()
        {
            return (await this.connection.QueryWithPolly<Recommend>("SELECT * FROM recommend"))?.ToList();
        }

        public Task<bool> InsertRecommend(string label, string feedId) => 
            this.connection.ExecuteWithPolly("INSERT INTO recommend(label, feed_id, create_time, update_time) VALUES(@label, @feedId, now(), now())", new { label, feedId });

        public Task<bool> DeleteRecommend(string label, string feedId) =>
            this.connection.ExecuteWithPolly("DELETE FROM recommend WHERE label=@label AND feed_id=@feedId", new { label, feedId });
    }
}
