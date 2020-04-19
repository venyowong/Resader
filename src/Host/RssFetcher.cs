using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Resader.Grains.Models;
using Resader.Host.Daos;
using Resader.Host.Models;

namespace Resader.Host
{
    public class RssFetcher : IInvocable
    {
        private ILogger<RssFetcher> logger;

        private IDbConnection connection;

        private Configuration configuration;

        public RssFetcher(ILogger<RssFetcher> logger, IDbConnection connection, IOptions<Configuration> config)
        {
            this.logger = logger;
            this.connection = connection;
            this.configuration = config.Value;
        }

        public FeedOverview Fetch(string feed, int seconds)
        {
            var time = DateTime.Now;
            var task = Task.Run(() => this.Fetch(feed));
            SpinWait.SpinUntil(() => task.IsCompletedSuccessfully || (DateTime.Now - time).TotalSeconds > seconds);
            if (task.IsCompletedSuccessfully)
            {
                return task.Result;
            }
            else
            {
                return null;
            }
        }
        
        public FeedOverview Fetch(string feed)
        {
            try
            {
                var feedId = feed.GetMd5Hash();
                SyndicationFeed sf = null;
                string title = null;
                CodeHollow.FeedReader.Feed cfeed = null;
                try 
                {
                    sf = SyndicationFeed.Load(XmlReader.Create(feed));
                    title = sf?.Title?.Text;
                }
                catch
                {
                    cfeed = CodeHollow.FeedReader.FeedReader.ReadAsync(feed).Result;
                    title = cfeed?.Title;
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    this.logger.LogWarning($"The title of feed({feed}) is null or white space.");
                    return null;
                }

                if (sf != null)
                {
                    return new FeedOverview
                    {
                        Title = title,
                        Articles = this.ParseArticles(sf, feedId)
                            .Select(a => a.ToResponseModel())
                            .ToList()
                    };
                }
                else
                {
                    return new FeedOverview
                    {
                        Title = title,
                        Articles = ParseArticles(cfeed, feedId)
                            .Select(a => a.ToResponseModel())
                            .ToList()
                    };
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, $"failed to add {feed}");
            }

            return null;
        }

        public Task Invoke()
        {
            this.logger.LogInformation("fetching rss...");
            
            var feeds = this.connection.GetFeeds();
            if (feeds != null && feeds.Any())
            {
                var tasks = new List<Task>();
                foreach(var feed in feeds)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        this.Fetch(feed.Url);
                    }));
                }
                Task.WaitAll(tasks.ToArray(), new TimeSpan(0, 1, 0));
            }

            return Task.CompletedTask;
        }

        public List<Resader.Host.Models.Article> ParseArticles(SyndicationFeed sf, string feedId)
        {
            if (sf == null || sf.Items == null || !sf.Items.Any())
            {
                this.logger.LogWarning("SyndicationFeed is empty, so no articles.");
                return null;
            }

            var articles = new List<Resader.Host.Models.Article>();
            foreach (var item in sf.Items)
            {
                var articleUrl = item?.Links?.FirstOrDefault()?.Uri?.AbsoluteUri;
                var articleTitle = item?.Title?.Text;
                if (string.IsNullOrWhiteSpace(articleUrl) || string.IsNullOrWhiteSpace(articleTitle))
                {
                    continue;
                }

                var articleId = feedId + articleUrl.GetMd5Hash();
                var content = string.Empty;
                if (item.Content is TextSyndicationContent textContent)
                {
                    content = textContent.Text;
                }
                var article = new Resader.Host.Models.Article
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

                try
                {
                    using (var conn = new MySqlConnection(this.configuration?.MySql?.ConnectionString))
                    {
                        if (conn.GetArticle(articleId) == null)
                        {
                            conn.InsertArticle(article);
                        }
                        else
                        {
                            conn.UpdateArticle(article);
                        }
                    }
                }
                catch(Exception e)
                {
                    this.logger.LogError(e, $"error occured when insert {articleUrl}");
                }
            }

            return articles;
        }

        public List<Resader.Host.Models.Article> ParseArticles(CodeHollow.FeedReader.Feed feed, string feedId)
        {
            if (feed == null || feed.Items == null || !feed.Items.Any())
            {
                this.logger.LogWarning("Feed is empty, so no articles.");
                return null;
            }

            var articles = new List<Resader.Host.Models.Article>();
            foreach (var item in feed.Items)
            {
                var articleUrl = item?.Link;
                var articleTitle = item?.Title;
                if (string.IsNullOrWhiteSpace(articleUrl) || string.IsNullOrWhiteSpace(articleTitle))
                {
                    continue;
                }

                var articleId = feedId + articleUrl.GetMd5Hash();
                var content = this.Simplify(item.Content);
                var article = new Resader.Host.Models.Article
                {
                    Id = articleId,
                    Url = articleUrl,
                    FeedId = feedId,
                    Title = articleTitle,
                    Summary = content,
                    Published = item.PublishingDate ?? DateTime.Now,
                    Updated = item.PublishingDate ?? DateTime.Now,
                    Keyword = string.Join(',', item.Categories),
                    Content = content,
                    Contributors = item.Author,
                    Authors = item.Author
                };
                articles.Add(article);

                try
                {
                    using (var conn = new MySqlConnection(this.configuration?.MySql?.ConnectionString))
                    {
                        if (conn.GetArticle(articleId) == null)
                        {
                            conn.InsertArticle(article);
                        }
                        else
                        {
                            conn.UpdateArticle(article);
                        }
                    }
                }
                catch(Exception e)
                {
                    this.logger.LogError(e, $"error occured when insert {articleUrl}");
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