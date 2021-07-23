using Microsoft.AspNetCore.Mvc;
using Resader.Api.Attributes;
using Resader.Api.Daos;
using Resader.Common.Extensions;
using Resader.Api.Services;
using Resader.Common;
using Resader.Common.Api.Request;
using Resader.Common.Api.Response;
using Resader.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OPMLCore.NET;

namespace Resader.Api.Controllers
{
    [ApiController]
    [Route("/RSS")]
    public class RssController : BaseController
    {
        private RssDao dao;
        private FetchService fetchService;
        private RssService service;

        public RssController(RssDao dao, FetchService fetchService, RssService service)
        {
            this.dao = dao;
            this.fetchService = fetchService;
            this.service = service;
        }

        [JwtValidation]
        [HttpGet("Articles")]
        public Result<List<ArticleResponse>> GetArticles([FromQuery][Required] GetArticlesRequest request)
        {
            // 记录 endtime 为空或当前时间一分钟内的请求记录，用于判断 feed 是否有新文章更新
            DateTime.TryParse(request.EndTime, out DateTime end);
            if (end == default || (end >= DateTime.Now.AddMinutes(-1) && end <= DateTime.Now.AddMinutes(1)))
            {
                this.service.SaveFeedBrowseRecord(this.GetUserId(), request.FeedId);
            }

            var articles = this.service.GetArticles(request.FeedId)
                .Where(a => end != default ? a.Published < end : true)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .Select(a => a.ToResponseModel())
                .ToList();
            var records = this.service.GetReadRecords(this.GetUserId(), articles.Select(a => a.Id).ToList());
            articles.ForEach(a => a.Read = records.Any(r => r.ArticleId == a.Id));
            return Result.Success(articles);
        }

        [JwtValidation]
        [HttpPost("Subscribe")]
        public Result<List<FeedOverview>> Subscribe([Required][FromBody] List<string> feeds)
        {
            var tasks = feeds.Select(feed => this.SubscribeFeed(feed, this.GetUserId())).ToArray();
            Task.WaitAll(tasks);
            return Result.Success(tasks.Select(t => t.Result)
                .Where(f => f != null)
                .ToList());
        }

        [JwtValidation]
        [HttpPost("Unsubscribe")]
        public async Task<Result> Unsubscribe([Required][FromBody] List<string> feeds)
        {
            if (await this.dao.DeleteSubscriptions(feeds, this.GetUserId()))
            {
                this.service.ClearFeedCache(this.GetUserId());
                return Result.Success();
            }
            else
            {
                return Result.Fail(2);
            }
        }

        [JwtValidation]
        [HttpPost("Read")]
        public Result Read([Required][FromBody] List<string> articles)
        {
            this.service.SaveReadRecords(this.GetUserId(), articles);
            return Result.Success();
        }

        [JwtValidation]
        [HttpGet("Feeds")]
        public async Task<Result<List<FeedResponse>>> GetFeeds()
        {
            var feeds = await this.service.GetFeeds(this.GetUserId());
            return Result.Success(feeds.Select(f =>
            {
                var browseRecord = this.service.GetFeedBrowseRecord(this.GetUserId(), f.Id);
                var active = true;
                if (browseRecord != null  && browseRecord.UpdateTime >= this.service.GetFeedLatestTime(f.Id))
                {
                    active = false;
                }
                return new FeedResponse
                {
                    Id = f.Id,
                    Title = f.Title,
                    Url = f.Url,
                    Active = active
                };
            })
            .ToList());
        }

        [HttpGet("opml.xml")]
        public async Task<string> GetOpml([FromQuery][Required] string userId)
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
            foreach (var feed in await this.service.GetFeeds(userId))
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

        private async Task<FeedOverview> SubscribeFeed(string feed, string userId)
        {
            (Feed Feed, List<Article> Articles) fetchResult;

            #region 插入 feed
            var feedId = feed.Md5();
            var feedEntity = await this.dao.GetFeed(feedId);
            if (feedEntity == null)
            {
                fetchResult = this.fetchService.Fetch(feed, 30);
                if (fetchResult == default)
                {
                    return null;
                }

                feedEntity = fetchResult.Feed;
                await this.dao.InsertFeed(feedEntity);

                this.service.SaveArticles(feedId, fetchResult.Articles);
            }
            else
            {
                fetchResult = (feedEntity, this.service.GetArticles(feedId)
                    .OrderByDescending(a => a.CreateTime)
                    .Take(10)
                    .ToList());
            }
            #endregion

            #region 订阅
            var subscription = await this.dao.GetSubscription(userId, feedId);
            if (subscription == null)
            {
                this.service.ClearFeedCache(userId);

                subscription = new Subscription
                {
                    UserId = userId,
                    FeedId = feedId
                };
                await this.dao.InsertSubscription(subscription);
            }
            #endregion

            return new FeedOverview
            {
                Id = feedId,
                Title = fetchResult.Feed.Title,
                Articles = fetchResult.Articles.Select(a => a.ToResponseModel()).ToList()
            };
        }
    }
}
