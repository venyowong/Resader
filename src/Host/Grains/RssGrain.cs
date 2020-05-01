using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OPMLCore.NET;
using Orleans;
using Orleans.Http.Abstractions;
using Resader.Grains;
using Resader.Grains.Models;
using Resader.Host.Daos;
using Resader.Host.Models;

namespace Resader.Host.Grains
{
    public class RssGrain : Grain, IRssGrain
    {
        private IDbConnection connection;

        private RssFetcher fetcher;

        private ILogger<RssGrain> logger;

        public RssGrain(IDbConnection connection, RssFetcher fetcher, ILogger<RssGrain> logger)
        {
            this.connection = connection;
            this.fetcher = fetcher;
            this.logger = logger;
        }

        public Task<Result<List<Resader.Grains.Models.Article>>> GetArticles([FromQuery] string feedId, [FromQuery] int page, 
            [FromQuery] int pageCount, [FromQuery] string endTime)
        {
            if (page < 0 || pageCount <= 0 || string.IsNullOrWhiteSpace(feedId))
            {
                return default(List<Resader.Grains.Models.Article>).ToResult(1);
            }

            DateTime.TryParse(endTime, out DateTime end);
            var articles = this.connection.GetArticles(feedId, page * pageCount, pageCount, end)
                ?.Select(a => a.ToResponseModel()).ToList();
            if (articles != null && articles.Any())
            {
                var readRecords = this.connection.GetReadRecords(this.GetPrimaryKeyString(), articles?.Select(a => a.Id));
                var readArticles = readRecords?.Select(r => r.ArticleId);
                foreach(var article in articles)
                {
                    article.Read = readArticles?.Contains(article.Id) ?? false;
                }
            }
            return articles?.ToResult();
        }

        public Task<Result<List<Resader.Grains.Models.Feed>>> GetFeeds()
        {
            return this.connection.GetFeeds(this.GetPrimaryKeyString())
                ?.Select(a => a.ToResponseModel())
                .ToList()
                .ToResult();
        }

        public Task<string> GetOpml()
        {
            var opml = new Opml();
            opml.Encoding = "UTF-8";
            opml.Version = "2.0";
            var head = new Head();
            head.Title = "Resader Feeds";
            head.DateCreated = DateTime.Now;
            head.DateModified = DateTime.Now;
            opml.Head = head;

            Body body = new Body();
            foreach (var feed in this.connection.GetFeeds(this.GetPrimaryKeyString()))
            {
                body.Outlines.Add(new Outline
                {
                    Text = feed.Title,
                    Title = feed.Title,
                    Created = DateTime.Now,
                    HTMLUrl = feed.Url,
                    Type = "rss",
                    XMLUrl = feed.Url
                });
            }
            opml.Body = body;

            return Task.FromResult(opml.ToString());
        }

        public Task<Result> Read([FromBody] List<string> articles)
        {
            if (articles == null || !articles.Any())
            {
                return 1.ToResult(string.Empty);
            }

            var readRecords = this.connection.GetReadRecords(this.GetPrimaryKeyString(), articles);
            if (readRecords != null)
            {
                articles = articles.Where(id => !readRecords.Any(record => record.ArticleId == id)).ToList();
            }

            if (this.connection.InsertReadRecords(articles.Select(article => new ReadRecord
            {
                ArticleId = article,
                UserId = this.GetPrimaryKeyString()
            })) > 0)
            {
                return 0.ToResult(string.Empty);
            }
            else
            {
                return 2.ToResult(string.Empty);
            }
        }

        public Task<Result<List<FeedOverview>>> Subscribe([FromBody] List<string> feeds)
        {
            if (feeds == null || !feeds.Any())
            {
                return default(List<FeedOverview>).ToResult(1);
            }

            return feeds.AsParallel()
                .Select(feed => this.SubscribeFeed(feed, this.GetPrimaryKeyString()))
                .Where(f => f != null)
                .ToList()
                .ToResult();
        }

        public Task<Result> Unsubscribe([FromBody] List<string> feeds)
        {
            if (feeds == null || !feeds.Any())
            {
                return 1.ToResult(string.Empty);
            }

            if (this.connection.DeleteSubscriptions(feeds, this.GetPrimaryKeyString()) > 0)
            {
                return 0.ToResult(string.Empty);
            }
            else
            {
                return 2.ToResult(string.Empty);
            }
        }

        public Task<Result<List<string>>> GetActiveFeeds()
        {
            return this.connection.GetSubscriptions(this.GetPrimaryKeyString())
                ?.ToDictionary(item => item.FeedId, item => this.connection.GetLastestArticle(item.FeedId))
                .Where(item => 
                {
                    if (item.Value == null)
                    {
                        return false;
                    }

                    if (this.connection.GetReadRecords(this.GetPrimaryKeyString(), new string[]{item.Value.Id})?.Any() ?? false)
                    {
                        return false;
                    }

                    return true;
                })
                .Select(item => item.Key)
                .ToList()
                .ToResult();
        }

        private FeedOverview SubscribeFeed(string feed, string userId)
        {
            var result = this.fetcher.Fetch(feed, 5);
            if (result == default)
            {
                return result;
            }

            #region 插入 feed
            var feedId = feed.GetMd5Hash();
            result.Id = feedId;
            var feedEntity = this.connection.GetFeed(feedId);
            if (feedEntity == null)
            {
                feedEntity = new Resader.Host.Models.Feed
                {
                    Id = feedId,
                    Url = feed,
                    Title = result.Title
                };
                this.connection.InsertFeed(feedEntity);
            }
            #endregion

            #region 订阅
            var subscription = this.connection.GetSubscription(userId, feedId);
            if (subscription == null)
            {
                subscription = new Subscription
                {
                    UserId = userId,
                    FeedId = feedId
                };
                this.connection.InsertSubscription(subscription);
            }
            #endregion

            return result;
        }
    }
}