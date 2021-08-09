using Resader.Api.Daos;
using Resader.Api.Extensions;
using Resader.Api.Factories;
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
        private RecommendService recommendService;
        private DbConnectionFactory dbConnectionFactory;
        private FetchService fetchService;

        public RssService(ICacheService cache, RecommendService recommendService,
            DbConnectionFactory dbConnectionFactory, FetchService fetchService)
        {
            this.cache = cache;
            this.dbConnectionFactory = dbConnectionFactory;
            this.recommendService = recommendService;
            this.fetchService = fetchService;
        }

        #region article
        public async Task<List<Article>> GetArticles(string feedId)
        {
            var key = Const.ArticlesInFeedCache + feedId;
            var map = this.cache.HashGetAll(key);
            if (map.Count > 0)
            {
                return map.Values.Select(x => x.ToObj<Article>())
                    .Where(x => x != default)
                    .ToList();
            }

            var dao = new RssDao(this.dbConnectionFactory);
            var articles = await dao.GetArticles(feedId);
            if (articles.IsNullOrEmpty())
            {
                articles = new List<Article>();
            }
            this.cache.HashSet(key, articles.ToDictionary(a => a.Id, a => a.ToJson()));
            return articles;
        }

        public async Task<bool> AddArticles(string feedId, List<Article> articles)
        {
            if (string.IsNullOrWhiteSpace(feedId) || articles.IsNullOrEmpty())
            {
                return false;
            }

            var articleList = await this.GetArticles(feedId);
            articles = articles.FindAll(a => !articleList.Any(x => x.Id == a.Id));
            var dao = new RssDao(this.dbConnectionFactory);
            if (!await dao.InsertArticle(articles.ToArray()))
            {
                return false;
            }

            this.cache.HashSet(Const.ArticlesInFeedCache + feedId, articles.ToDictionary(x => x.Id, x => x.ToJson()));
            this.cache.StringSet(Const.FeedLatestTimeCache + feedId, DateTime.Now.ToString("yyyy-MM-dd"));
            return true;
        }

        public async Task<bool> RefreshArticles(string feedId)
        {
            if (string.IsNullOrWhiteSpace(feedId))
            {
                return false;
            }

            var dao = new RssDao(this.dbConnectionFactory);
            var articles = await dao.GetArticles(feedId);
            if (!articles.IsNullOrEmpty())
            {
                this.cache.HashSet(Const.ArticlesInFeedCache + feedId, articles.ToDictionary(x => x.Id, x => x.ToJson()));
                this.cache.StringSet(Const.FeedLatestTimeCache + feedId, articles.Max(a => a.CreateTime).ToString("yyyy-MM-dd"));
            }
            return true;
        }

        public DateTime GetFeedLatestTime(string feedId)
        {
            DateTime.TryParse(this.cache.StringGet($"{Const.FeedLatestTimeCache}{feedId}"), out var latestTime);
            return latestTime;
        }
        #endregion

        #region record
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
        #endregion

        #region feed
        public List<Feed> GetFeeds(string userId)
        {
            var subscriptions = this.cache.HashGetAll(Const.SubscriptionCache + userId)
                .Select(x => x.Value.ToObj<Subscription>())
                .ToList();
            return this.GetFeeds(subscriptions.Select(x => x.FeedId).ToArray());
        }

        public Feed GetFeed(string feedId) =>
            this.cache.HashGet(Const.FeedsCache, feedId)?.ToObj<Feed>();

        public List<Feed> GetFeeds(string[] feedIds) =>
            this.cache.HashGet(Const.FeedsCache, feedIds)
                .Select(x => x.Value.ToObj<Feed>())
                .ToList();

        public List<Feed> GetFeeds() =>
            this.cache.HashGetAll(Const.FeedsCache)
                .Select(x => x.Value.ToObj<Feed>())
                .ToList();

        public void SaveFeeds(List<Feed> feeds)
        {
            if (feeds.IsNullOrEmpty())
            {
                return;
            }

            this.cache.HashSet(Const.FeedsCache, feeds.ToDictionary(r => r.Id, r => r.ToJson()));
        }

        public async Task<bool> UpdateFeed(Feed feed)
        {
            var dao = new RssDao(this.dbConnectionFactory);
            if (!await dao.UpdateFeed(feed))
            {
                return false;
            }

            var oldFeed = this.GetFeed(feed.Id);
            this.cache.HashSet(Const.FeedsCache, feed.Id, feed.ToJson());
            this.recommendService.UpdateFeedLabel(feed, oldFeed.Label);
            return true;
        }

        public async Task<bool> AddFeed(Feed feed)
        {
            var dao = new RssDao(this.dbConnectionFactory);
            if (!await dao.InsertFeed(feed))
            {
                return false;
            }

            this.cache.HashSet(Const.FeedsCache, feed.Id, feed.ToJson());
            return true;
        }

        public async Task<(Feed Feed, List<Article> Articles)> AddFeed(string url)
        {
            var feedId = url.Md5();
            var feed = this.GetFeed(feedId);
            if (feed == null)
            {
                var result = fetchService.Fetch(url, 30);
                if (result == default)
                {
                    return result;
                }

                if (await this.AddFeed(result.Feed))
                {
                    await this.AddArticles(feedId, result.Articles);
                }
                return result;
            }
            else
            {
                return (feed, (await this.GetArticles(feedId))
                        .OrderByDescending(a => a.CreateTime)
                        .Take(10)
                        .ToList());
            }
        }
        #endregion

        #region subscription
        public Subscription GetSubscription(string userId, string feedId) =>
            this.cache.HashGet(Const.SubscriptionCache + userId, feedId)?.ToObj<Subscription>();

        public void SaveSubscriptions(string userId, List<Subscription> subscriptions)
        {
            if (subscriptions.IsNullOrEmpty() || string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            this.cache.HashSet(Const.SubscriptionCache + userId, subscriptions.ToDictionary(r => r.FeedId, r => r.ToJson()));
        }

        public async Task<bool> AddSubscription(Subscription subscription)
        {
            var dao = new RssDao(this.dbConnectionFactory);
            if (subscription == null || !await dao.InsertSubscription(subscription))
            {
                return false;
            }

            this.cache.HashSet(Const.SubscriptionCache + subscription.UserId, subscription.FeedId, subscription.ToJson());
            return true;
        }

        public async Task<bool> DeleteSubscriptions(string userId, List<string> feeds)
        {
            var dao = new RssDao(this.dbConnectionFactory);
            if (await dao.DeleteSubscriptions(feeds, userId))
            {
                this.cache.HashDelete(Const.SubscriptionCache + userId, feeds.ToArray());
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
