using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Resader.Extensions;
using Resader.Models;

namespace Resader.Daos
{
    public class RssDao
    {
        private IDbConnection connection;

        public RssDao(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Article> GetArticle(string articleId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(articleId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Article>("SELECT * FROM article WHERE id=@Id", new { Id = articleId });
        }

        public async Task<IEnumerable<Article>> GetArticles(string feedId, int skip, int take, DateTime endTime)
        {
            if (connection == null || string.IsNullOrWhiteSpace(feedId))
            {
                return null;
            }

            var sql = string.Empty;
            if (endTime != default)
            {
                sql = "SELECT * FROM article WHERE feed_id=@FeedId AND published<@EndTime ORDER BY published DESC LIMIT @Skip,@Take";
            }
            else
            {
                sql = "SELECT * FROM article WHERE feed_id=@FeedId ORDER BY published DESC LIMIT @Skip,@Take";
            }
            return await connection.QueryWithPolly<Article>(sql, new
            {
                FeedId = feedId,
                Skip = skip,
                Take = take,
                EndTime = endTime
            });
        }

        public async Task<bool> InsertArticle(Article article)
        {
            if (connection == null || article == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("INSERT INTO article(id, url, feed_id, title, summary, published, updated, created, keyword, content, contributors, " +
                "authors, copyright, create_time, update_time) VALUES(@Id, @Url, @FeedId, @Title, @Summary, @Published, @Updated, now(), @Keyword, @Content, " +
                "@Contributors, @Authors, @Copyright, now(), now())", article);
        }

        public async Task<bool> UpdateArticle(Article article)
        {
            if (connection == null || article == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("UPDATE article SET title=@Title, summary=@Summary, published=@Published, updated=@Updated, " +
                "keyword=@Keyword, content=@Content, contributors=@Contributors, authors=@Authors, copyright=@Copyright, update_time=now() WHERE id=@Id", article);
        }

        public async Task<IEnumerable<Feed>> GetFeeds(string userId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryWithPolly<Feed>("SELECT * FROM feed WHERE id IN (SELECT feed_id FROM subscription WHERE user_id=@UserId)",
                new { UserId = userId });
        }

        public async Task<Feed> GetFeed(string feedId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(feedId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Feed>("SELECT * FROM feed WHERE id=@Id", new { Id = feedId });
        }

        public async Task<bool> InsertFeed(Feed feed)
        {
            if (connection == null || feed == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("INSERT INTO feed(id, url, title, create_time, update_time) VALUES(@Id, @Url, @Title, now(), now())", feed);
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptions(string userId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryWithPolly<Subscription>("SELECT * FROM subscription WHERE user_id=@userId", new { userId });
        }

        public async Task<Subscription> GetSubscription(string userId, string feedId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(feedId) || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Subscription>(
                "SELECT * FROM subscription WHERE user_id=@UserId AND feed_id=@FeedId", new { FeedId = feedId, UserId = userId });
        }

        public async Task<bool> InsertSubscription(Subscription subscription)
        {
            if (connection == null || subscription == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("INSERT INTO subscription(user_id, feed_id, create_time, update_time) VALUES(@UserId, @FeedId, now(), now())", subscription);
        }

        public async Task<bool> DeleteSubscriptions(List<string> feeds, string userId)
        {
            if (connection == null || feeds.IsNullOrEmpty() || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            return await connection.ExecuteWithPolly("DELETE FROM subscription WHERE user_id=@UserId AND feed_id in @Feeds", new
            {
                UserId = userId,
                Feeds = feeds
            });
        }

        public async Task<IEnumerable<ReadRecord>> GetReadRecords(string userId, IEnumerable<string> articles)
        {
            if (connection == null || articles.IsNullOrEmpty() || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryWithPolly<ReadRecord>("SELECT * FROM readrecord where user_id=@userId AND article_id in @articles;", new
            {
                userId,
                articles
            });
        }

        public async Task<bool> InsertReadRecords(IEnumerable<ReadRecord> records)
        {
            if (connection == null || records.IsNullOrEmpty())
            {
                return false;
            }

            try
            {
                return await connection.ExecuteWithPolly("INSERT INTO readrecord(article_id, user_id, create_time, update_time) VALUES(@ArticleId, @UserId, now(), now())", records);
            }
            catch
            {
                return false;
            }
        }
    
        public async Task<IEnumerable<Feed>> GetFeeds()
        {
            if (connection == null)
            {
                return null;
            }
            
            return await connection.QueryWithPolly<Feed>("SELECT * FROM feed");
        }

        public async Task<Article> GetLastestArticle(string feedId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(feedId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Article>("select * from article a where a.feed_id=@feedId order by a.published desc limit 1;", new {feedId});
        }
    }
}