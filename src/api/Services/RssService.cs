using Resader.Api.Daos;
using Resader.Api.Extensions;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Services
{
    public class RssService
    {
        private ICacheService cache;
        private RssDao dao;

        public RssService(ICacheService cache, RssDao dao)
        {
            this.cache = cache;
            this.dao = dao;
        }

        public List<Article> GetArticles(string feedId)
        {
            var map = this.cache.HashGetAll(Const.ArticlesInFeedCache + feedId);
            return map.Values.Select(x => x.ToObj<Article>())
                .Where(x => x != default)
                .ToList();
        }

        public void SaveArticles(string feedId, List<Article> articles)
        {
            this.cache.HashSet(Const.ArticlesInFeedCache + feedId, articles.ToDictionary(x => x.Id, x => x.ToJson()));
            this.cache.StringSet(Const.FeedLatestTimeCache + feedId, articles.Max(x => x.CreateTime).ToString("yyyy-MM-dd"));
        }

        public DateTime GetFeedLatestTime(string feedId)
        {
            DateTime.TryParse(this.cache.StringGet($"{Const.FeedLatestTimeCache}{feedId}"), out var latestTime);
            return latestTime;
        }

        public void SaveReadRecords(string userId, List<string> articles)
        {
            if (articles.IsNullOrEmpty() || string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            this.SaveReadRecords(userId, articles.Select(a => new ReadRecord
            {
                ArticleId = a,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                UserId = userId
            }).ToList());
        }

        public void SaveReadRecords(string userId, List<ReadRecord> records)
        {
            if (records.IsNullOrEmpty() || string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            this.cache.HashSet(Const.ReadRecordCache + userId, records.ToDictionary(r => r.ArticleId, r => r.ToJson()));
        }

        public List<ReadRecord> GetReadRecords(string userId)
        {
            return this.cache.HashGetAll(Const.ReadRecordCache + userId)
                .Values
                .Select(x => x.ToObj<ReadRecord>())
                .ToList();
        }

        public List<ReadRecord> GetReadRecords(string userId, List<string> articles)
        {
            return this.cache.HashGet(Const.ReadRecordCache + userId, articles.ToArray())
                .Select(x => x.Value.ToObj<ReadRecord>())
                .ToList();
        }

        public async Task<List<Feed>> GetFeeds(string userId)
        {
            var key = Const.FeedCache + userId;
            var str = this.cache.StringGet(key);
            if (!string.IsNullOrWhiteSpace(str))
            {
                return str.GZipDecompress().ToObj<List<Feed>>();
            }

            var feeds = (await this.dao.GetFeeds(userId)).ToList();
            this.cache.StringSet(key, feeds.ToJson().GZipCompress());
            return feeds;
        }

        public void ClearFeedCache(string userId)
        {
            this.cache.DeleteKey(Const.FeedCache + userId);
        }

        public void SaveFeedBrowseRecord(string userId, string feedId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(feedId))
            {
                return;
            }

            this.cache.HashSet(Const.FeedLatestBrowseTimeCache + userId, feedId, new FeedBrowseRecord
            {
                UserId = userId,
                FeedId = feedId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            }.ToJson());
        }

        public void SaveFeedBrowseRecords(string userId, List<FeedBrowseRecord> records)
        {
            if (records.IsNullOrEmpty() || string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            this.cache.HashSet(Const.FeedLatestBrowseTimeCache + userId, records.ToDictionary(r => r.FeedId, r => r.ToJson()));
        }

        public FeedBrowseRecord GetFeedBrowseRecord(string userId, string feedId)
        {
            var json = this.cache.HashGet(Const.FeedLatestBrowseTimeCache + userId, feedId);
            return json.ToObj<FeedBrowseRecord>();
        }

        public List<FeedBrowseRecord> GetFeedBrowseRecords(string userId)
        {
            return this.cache.HashGetAll(Const.FeedLatestBrowseTimeCache + userId)
                .Values
                .Select(x => x.ToObj<FeedBrowseRecord>())
                .ToList();
        }
    }
}
