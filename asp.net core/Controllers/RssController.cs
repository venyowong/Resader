using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPMLCore.NET;
using Resader.Daos;
using Resader.Extensions;
using Resader.Models;
using Resader.Models.Request;
using Resader.Models.Response;

namespace Resader.Controllers
{
    [ApiController]
    [Route("/RSS")]
    public class RssController : Controller
    {
        private IDbConnection connection;

        private RssFetcher rssFetcher;

        public RssController(IDbConnection connection, RssFetcher rssFetcher)
        {
            this.connection = connection;
            this.rssFetcher = rssFetcher;
        }

        [Authorize]
        [HttpGet("Articles")]
        public async Task<Result<List<ArticleResponse>>> GetArticles([Required] GetArticlesRequest request)
        {
            DateTime.TryParse(request.EndTime, out DateTime end);
            var articles = (await this.connection.GetArticles(request.FeedId, request.Page * request.PageCount, 
                request.PageCount, end))?.Select(a => a.ToResponseModel()).ToList();
            if (articles != null && articles.Any())
            {
                var readRecords = await this.connection.GetReadRecords(request.UserId, articles?.Select(a => a.Id));
                var readArticles = readRecords?.Select(r => r.ArticleId);
                foreach(var article in articles)
                {
                    article.Read = readArticles?.Contains(article.Id) ?? false;
                }
            }
            return articles?.ToResult();
        }

        [Authorize]
        [HttpGet("Feeds")]
        public async Task<Result<List<Feed>>> GetFeeds([Required] string userId)
        {
            return (await this.connection.GetFeeds(userId))?.ToList().ToResult();
        }

        [Authorize]
        [HttpGet("opml.xml")]
        public async Task<string> GetOpml([Required] string userId)
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
            foreach (var feed in (await this.connection.GetFeeds(userId)))
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

            return opml.ToString();
        }

        [Authorize]
        [HttpPost("Read")]
        public async Task<Result> Read([Required] ReadRequest request)
        {
            var readRecords = await this.connection.GetReadRecords(request.UserId, request.Articles);
            if (readRecords != null)
            {
                request.Articles = request.Articles.Where(id => !readRecords.Any(record => record.ArticleId == id)).ToList();
            }

            if (await this.connection.InsertReadRecords(request.Articles.Select(article => new ReadRecord
            {
                ArticleId = article,
                UserId = request.UserId
            })))
            {
                return Result.CreateSuccessResult();
            }
            else
            {
                return Result.CreateFailureResult(2);
            }
        }

        [Authorize]
        [HttpPost("Subscribe")]
        public Result<List<FeedOverview>> Subscribe([Required] SubscribeRequest request)
        {
            var tasks = request.Feeds.Select(feed => this.SubscribeFeed(feed, request.UserId)).ToArray();
            Task.WaitAll(tasks);
            return tasks.Select(t => t.Result)
                .Where(f => f != null)
                .ToList()
                .ToResult();
        }

        [Authorize]
        [HttpPost("Unsubscribe")]
        public async Task<Result> Unsubscribe([Required] UnsubscribeRequest request)
        {
            if (await this.connection.DeleteSubscriptions(request.Feeds, request.UserId))
            {
                return Result.CreateSuccessResult();
            }
            else
            {
                return Result.CreateFailureResult(2);
            }
        }

        [Authorize]
        [HttpGet("ActiveFeeds")]
        public async Task<Result<List<string>>> GetActiveFeeds([Required] string userId)
        {
            var subscriptions = await this.connection.GetSubscriptions(userId);
            if (subscriptions == null || !subscriptions.Any())
            {
                return null;
            }

            var dic = new Dictionary<string, Article>();
            foreach (var item in subscriptions)
            {
                var article = await this.connection.GetLastestArticle(item.FeedId);
                if (article != null)
                {
                    dic.Add(item.FeedId, article);
                }
            }

            var result = new List<string>();
            foreach (var pair in dic)
            {
                var records = await this.connection.GetReadRecords(userId, new string[] { pair.Value.Id });
                if (records == null || !records.Any())
                {
                    result.Add(pair.Key);
                }
            }
            return result.ToResult();
        }

        private async Task<FeedOverview> SubscribeFeed(string feed, string userId)
        {
            FeedOverview result = null;

            #region 插入 feed
            var feedId = feed.GetMd5Hash();
            var feedEntity = await this.connection.GetFeed(feedId);
            if (feedEntity == null)
            {
                result = this.rssFetcher.Fetch(feed, 30);
                if (result == default)
                {
                    return result;
                }

                feedEntity = new Feed
                {
                    Id = feedId,
                    Url = feed,
                    Title = result.Title
                };
                await this.connection.InsertFeed(feedEntity);
            }
            else
            {
                result = new FeedOverview
                {
                    Title = feedEntity.Title,
                    Articles = (await this.connection.GetArticles(feedId, 0, 10, DateTime.Now))
                        ?.Select(a => a.ToResponseModel())
                        .ToList()
                };
            }
            result.Id = feedId;
            #endregion

            #region 订阅
            var subscription = await this.connection.GetSubscription(userId, feedId);
            if (subscription == null)
            {
                subscription = new Subscription
                {
                    UserId = userId,
                    FeedId = feedId
                };
                await this.connection.InsertSubscription(subscription);
            }
            #endregion

            return result;
        }
    }
}