using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Options;
using Resader.Daos;
using Resader.Extensions;
using Resader.Factories;
using Resader.Models;
using Resader.Models.Response;
using Serilog;

namespace Resader.Services
{
    public class FetchService
    {
        private DbConnectionFactory connectionFactory;
        private Configuration configuration;
        
        public HttpClient Client{get;set;}

        public FetchService(DbConnectionFactory connectionFactory, IOptions<Configuration> config, HttpClient httpClient)
        {
            this.connectionFactory = connectionFactory;
            this.configuration = config.Value;
            this.Client = httpClient;
        }

        public FeedOverview Fetch(string feed, int timeout)
        {
            var time = DateTime.Now;
            var task = this.Fetch(feed);
            SpinWait.SpinUntil(() => task.IsCompletedSuccessfully || (DateTime.Now - time).TotalSeconds > timeout);
            if (task.IsCompletedSuccessfully)
            {
                return task.Result;
            }
            else
            {
                return null;
            }
        }
        
        public async Task<FeedOverview> Fetch(string feed)
        {
            try
            {
                var feedId = feed.Md5();
                SyndicationFeed sf = null;
                string title = null;
                CodeHollow.FeedReader.Feed cfeed = null;
                try 
                {
                    var html = this.Client.GetStringAsync(feed).Result;
                    cfeed = CodeHollow.FeedReader.FeedReader.ReadFromString(html);
                    title = cfeed?.Title;
                }
                catch
                {
                    sf = SyndicationFeed.Load(XmlReader.Create(feed));
                    title = sf?.Title?.Text;
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    Log.Warning($"The title of feed({feed}) is null or white space.");
                    return null;
                }

                if (sf != null)
                {
                    return new FeedOverview
                    {
                        Title = title,
                        Articles = (await this.ParseArticles(sf, feedId))
                            .Select(a => a.ToResponseModel())
                            .ToList()
                    };
                }
                else
                {
                    return new FeedOverview
                    {
                        Title = title,
                        Articles = (await this.ParseArticles(cfeed, feedId))
                            .Select(a => a.ToResponseModel())
                            .ToList()
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"failed to add {feed}");
            }

            return null;
        }

        public async Task<List<Article>> ParseArticles(SyndicationFeed sf, string feedId)
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

                using (var conn = await this.connectionFactory.CreateDbConnection("MySql", "resader"))
                {
                    var rssDao = new RssDao(conn);
                    if ((await rssDao.GetArticle(articleId)) == null)
                    {
                        await rssDao.InsertArticle(article);
                    }
                    else
                    {
                        await rssDao.UpdateArticle(article);
                    }
                }
            }

            return articles;
        }

        public async Task<List<Article>> ParseArticles(CodeHollow.FeedReader.Feed feed, string feedId)
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

                using (var conn = await this.connectionFactory.CreateDbConnection("MySql", "resader"))
                {
                    var rssDao = new RssDao(conn);
                    if ((await rssDao.GetArticle(articleId)) == null)
                    {
                        await rssDao.InsertArticle(article);
                    }
                    else
                    {
                        await rssDao.UpdateArticle(article);
                    }
                }
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