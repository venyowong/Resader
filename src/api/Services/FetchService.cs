using Resader.Common.Extensions;
using Resader.Common.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Resader.Api.Services;

public class FetchService
{
    public HttpClient Client { get; set; }

    public FetchService(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", "resader");
        httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        this.Client = httpClient;
    }

    public (Feed Feed, List<Article> Articles) Fetch(string feed, int timeout)
    {
        var time = DateTime.Now;
        var task = Task.Run(() => this.Fetch(feed));
        SpinWait.SpinUntil(() => task.IsCompletedSuccessfully || (DateTime.Now - time).TotalSeconds > timeout);
        if (task.IsCompletedSuccessfully)
        {
            return task.Result;
        }
        else
        {
            return default;
        }
    }

    public (Feed Feed, List<Article> Articles) Fetch(string feed)
    {
        try
        {
            var feedId = feed.Md5();
            var feedEntity = new Feed
            {
                Id = feed.Md5(),
                Url = feed
            };

            var xml = this.Client.GetStringAsync(feed).Result;
            var cfeed = CodeHollow.FeedReader.FeedReader.ReadFromString(xml);
            feedEntity.Title = cfeed?.Title;
            feedEntity.Description = cfeed?.Description;
            feedEntity.Image = cfeed?.ImageUrl;

            if (string.IsNullOrWhiteSpace(feedEntity.Title))
            {
                Log.Warning($"The title of feed({feed}) is null or white space.");
                return default;
            }

            return (feedEntity, this.ParseArticles(cfeed, feedId));
        }
        catch (Exception e)
        {
            Log.Error(e, $"failed to add {feed}");
        }

        return default;
    }

    public List<Article> ParseArticles(CodeHollow.FeedReader.Feed feed, string feedId)
    {
        if (feed == null || feed.Items == null || !feed.Items.Any())
        {
            Log.Warning("Feed is empty, so no articles.");
            return null;
        }

        var articles = new List<Article>();
        foreach (var item in feed.Items)
        {
            var articleUrl = item?.Link;
            var articleTitle = item?.Title;
            if (string.IsNullOrWhiteSpace(articleUrl) || string.IsNullOrWhiteSpace(articleTitle))
            {
                continue;
            }

            var articleId = feedId + articleUrl.Md5();
            var content = this.Simplify(item.Content);
            var article = new Article
            {
                Id = articleId,
                Url = articleUrl,
                FeedId = feedId,
                Title = articleTitle,
                Summary = content,
                Published = item.PublishingDate?.ToLocalTime() ?? DateTime.Now,
                Updated = item.PublishingDate?.ToLocalTime() ?? DateTime.Now,
                Keyword = string.Join(',', item.Categories),
                Content = content,
                Contributors = item.Author,
                Authors = item.Author
            };
            articles.Add(article);
        }

        return articles;
    }

    private string Simplify(string input, int length = 500)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        if (input.Length <= length)
        {
            return input;
        }

        return input.Substring(0, length - 3) + "...";
    }
}
