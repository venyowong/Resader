using Microsoft.AspNetCore.Mvc;
using Resader.Api.Attributes;
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
using System.Text;

namespace Resader.Api.Controllers;

[ApiController]
[Route("/RSS")]
public class RssController : BaseController
{
    private RssService service;

    public RssController(RssService service)
    {
        this.service = service;
    }

    /// <summary>
    /// 获取在 EndTime 时间点前的文章
    /// <para>EndTime 为空则返回最新文章</para>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [JwtValidation]
    [HttpGet("Articles")]
    public async Task<Result<List<ArticleResponse>>> GetArticles([FromQuery][Required] GetArticlesRequest request)
    {
        // 记录 endtime 为空或当前时间一分钟内的请求记录，用于判断 feed 是否有新文章更新
        DateTime.TryParse(request.EndTime, out DateTime end);
        if (end == default || (end >= DateTime.Now.AddMinutes(-1) && end <= DateTime.Now.AddMinutes(1)))
        {
            this.service.SaveFeedBrowseRecord(this.GetUserId(), request.FeedId);
        }

        var articles = (await this.service.GetArticles(request.FeedId))
            .Where(a => end != default ? a.Published < end : true)
            .OrderByDescending(a => a.CreateTime)
            .ThenByDescending(a => a.Title)
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
        if (await this.service.DeleteSubscriptions(this.GetUserId(), feeds))
        {
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
    public Result<List<FeedResponse>> GetFeeds()
    {
        var feeds = this.service.GetFeeds(this.GetUserId());
        return Result.Success(feeds.OrderBy(f => f.CreateTime).Select(f =>
        {
            var browseRecord = this.service.GetFeedBrowseRecord(this.GetUserId(), f.Id);
            var lastSeen = browseRecord?.UpdateTime ?? DateTime.MinValue;
            var articles = this.service.GetArticles(f.Id).Result;
            var newArticleCount = articles.Count(a => a.CreateTime > lastSeen);
            return new FeedResponse
            {
                Id = f.Id,
                Title = f.Title,
                Url = f.Url,
                Active = newArticleCount > 0,
                NewArticleCount = newArticleCount
            };
        })
        .ToList());
    }

    [HttpGet("opml.xml")]
    public FileContentResult GetOpml([FromQuery][Required] string userId)
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
        foreach (var feed in this.service.GetFeeds(userId))
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

        return this.File(Encoding.UTF8.GetBytes(opml.ToString()), "application/xml");
    }

    [HttpGet("RecommendFeeds")]
    public List<RecommendedFeed> RecommendFeeds(string label, [FromServices] RecommendService recommendService,
        [FromServices] RssService rssService, [FromServices] ICacheService cache)
    {
        return cache.GetWithInit(Const.RecommendedFeedsCache + label, () =>
        {
            var feeds = recommendService.GetLabeledFeeds(label)
                .FindAll(f => f.Recommend);
            return feeds.Select(f => new RecommendedFeed
            {
                Feed = f,
                Article = rssService.GetArticles(f.Id).Result.OrderByDescending(f => f.CreateTime).FirstOrDefault()
            })
            .Where(f => f.Article != null)
            .OrderByDescending(x => x.Article?.CreateTime)
            .ToList();
        }, new TimeSpan(0, 5, 0));
    }

    private async Task<FeedOverview> SubscribeFeed(string feed, string userId)
    {
        var fetchResult = await this.service.AddFeed(feed);
        if (fetchResult == default)
        {
            return null;
        }

        var feedId = fetchResult.Feed.Id;
        var subscription = this.service.GetSubscription(userId, feedId);
        if (subscription == null)
        {
            subscription = new Subscription
            {
                UserId = userId,
                FeedId = feedId
            };
            await this.service.AddSubscription(subscription);
        }

        return new FeedOverview
        {
            Id = feedId,
            Title = fetchResult.Feed.Title,
            Articles = fetchResult.Articles.Select(a => a.ToResponseModel()).ToList()
        };
    }
}
