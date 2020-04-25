using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Resader.Grains;
using Resader.Grains.Models;
using Resader.Host.Daos;

namespace Resader.Host.Grains
{
    public class FetcherGrain : Grain, IFetcherGrain
    {
        private RssFetcher rssFetcher;

        private IDbConnection connection;

        public FetcherGrain(RssFetcher rssFetcher, IDbConnection connection)
        {
            this.rssFetcher = rssFetcher;
            this.connection = connection;
        }

        public Task<Result> Fetch()
        {
            var feeds = this.connection.GetFeeds();
            if (feeds != null && feeds.Any())
            {
                var tasks = new List<Task>();
                foreach(var feed in feeds)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        this.rssFetcher.Fetch(feed.Url);
                    }));
                }
                Task.WaitAll(tasks.ToArray(), new TimeSpan(0, 1, 0));
            }

            return 0.ToResult(string.Empty);
        }
    }
}