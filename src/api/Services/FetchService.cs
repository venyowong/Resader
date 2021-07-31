using Microsoft.Extensions.Options;
using Resader.Api.Daos;
using Resader.Common.Extensions;
using Resader.Api.Factories;
using Resader.Common.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Resader.Api.Services
{
    public class FetchService
    {
        public HttpClient Client { get; set; }

        public FetchService(HttpClient httpClient)
        {
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
                SyndicationFeed sf = null;
                CodeHollow.FeedReader.Feed cfeed = null;
                var feedEntity = new Feed
                {
                    Url = feed
                };
                try
                {
                    var html = this.Client.GetStringAsync(feed).Result;
                    cfeed = CodeHollow.FeedReader.FeedReader.ReadFromString(html);
                    feedEntity.Title = cfeed?.Title;
                    feedEntity.Description = cfeed?.Description;
                    feedEntity.Image = cfeed?.ImageUrl;
                }
                catch
                {
                    sf = SyndicationFeed.Load(XmlReader.Create(feed));
                    feedEntity.Title = sf?.Title?.Text;
                    feedEntity.Description = sf?.Description?.Text;
                    feedEntity.Image = sf?.ImageUrl?.ToString();
                }
                if (string.IsNullOrWhiteSpace(feedEntity.Title))
                {
                    Log.Warning($"The title of feed({feed}) is null or white space.");
                    return default;
                }

                List<Article> articles = null;
                if (sf != null)
                {
                    articles = this.ParseArticles(sf, feedId);
                }
                else
                {
                    articles = this.ParseArticles(cfeed, feedId);
                }
                return (feedEntity, articles);
            }
            catch (Exception e)
            {
                Log.Error(e, $"failed to add {feed}");
            }

            return default;
        }

        public List<Article> ParseArticles(SyndicationFeed sf, string feedId)
        {
            if (sf == null || sf.Items.IsNullOrEmpty())
            {
                Log.Warning("SyndicationFeed is empty, so no articles.");
                return null;
            }

            var articles = new List<Article>();
            foreach (var item in sf.Items)
            {
                var articleUrl = item?.Links?.FirstOrDefault()?.Uri?.AbsoluteUri;
                var articleTitle = item?.Title?.Text;
                if (string.IsNullOrWhiteSpace(articleUrl) || string.IsNullOrWhiteSpace(articleTitle))
                {
                    continue;
                }

                var articleId = feedId + articleUrl.Md5();
                var content = string.Empty;
                if (item.Content is TextSyndicationContent textContent)
                {
                    content = textContent.Text;
                }
                var article = new Article
                {
                    Id = articleId,
                    Url = articleUrl,
                    FeedId = feedId,
                    Title = articleTitle,
                    Summary = Simplify(item.Summary?.Text),
                    Published = item.PublishDate.LocalDateTime,
                    Updated = item.LastUpdatedTime.LocalDateTime,
                    Keyword = string.Join(',', item.Categories?.Select(c => c?.Name)),
                    Content = Simplify(content),
                    Contributors = string.Join(',', item.Contributors?.Select(c => c?.Name)),
                    Authors = string.Join(',', item.Authors?.Select(c => c?.Name)),
                    Copyright = item.Copyright?.Text
                };
                articles.Add(article);
            }

            return articles;
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
}
