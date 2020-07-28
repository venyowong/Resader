using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Resader.Extensions;
using Resader.Models;

namespace Resader.Daos
{
    public static class RssDao
    {
        public static async Task<Article> GetArticle(this IDbConnection connection, string articleId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(articleId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Article>("SELECT * FROM article WHERE id=@Id", new { Id = articleId });
        }

        public static async Task<IEnumerable<Article>> GetArticles(this IDbConnection connection, string feedId, 
            int skip, int take, DateTime endTime)
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

        public static async Task<bool> InsertArticle(this IDbConnection connection, Article article)
        {
            if (connection == null || article == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("INSERT INTO article(id, url, feed_id, title, summary, published, updated, created, keyword, content, contributors, " +
                "authors, copyright, create_time, update_time) VALUES(@Id, @Url, @FeedId, @Title, @Summary, @Published, @Updated, now(), @Keyword, @Content, " +
                "@Contributors, @Authors, @Copyright, now(), now())", article);
        }

        public static async Task<bool> UpdateArticle(this IDbConnection connection, Article article)
        {
            if (connection == null || article == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("UPDATE article SET title=@Title, summary=@Summary, published=@Published, updated=@Updated, " +
                "keyword=@Keyword, content=@Content, contributors=@Contributors, authors=@Authors, copyright=@Copyright, update_time=now() WHERE id=@Id", article);
        }

        public static async Task<IEnumerable<Feed>> GetFeeds(this IDbConnection connection, string userId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryWithPolly<Feed>("SELECT * FROM feed WHERE id IN (SELECT feed_id FROM subscription WHERE user_id=@UserId)",
                new { UserId = userId });
        }

        public static async Task<Feed> GetFeed(this IDbConnection connection, string feedId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(feedId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Feed>("SELECT * FROM feed WHERE id=@Id", new { Id = feedId });
        }

        public static async Task<bool> InsertFeed(this IDbConnection connection, Feed feed)
        {
            if (connection == null || feed == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("INSERT INTO feed(id, url, title, create_time, update_time) VALUES(@Id, @Url, @Title, now(), now())", feed);
        }

        public static async Task<IEnumerable<Subscription>> GetSubscriptions(this IDbConnection connection, string userId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryWithPolly<Subscription>("SELECT * FROM subscription WHERE user_id=@userId", new { userId });
        }

        public static async Task<Subscription> GetSubscription(this IDbConnection connection, string userId, string feedId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(feedId) || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Subscription>(
                "SELECT * FROM subscription WHERE user_id=@UserId AND feed_id=@FeedId", new { FeedId = feedId, UserId = userId });
        }

        public static async Task<bool> InsertSubscription(this IDbConnection connection, Subscription subscription)
        {
            if (connection == null || subscription == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("INSERT INTO subscription(user_id, feed_id, create_time, update_time) VALUES(@UserId, @FeedId, now(), now())", subscription);
        }

        public static async Task<bool> DeleteSubscriptions(this IDbConnection connection, List<string> feeds, string userId)
        {
            if (connection == null || feeds == null || !feeds.Any() || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            return await connection.ExecuteWithPolly("DELETE FROM subscription WHERE user_id=@UserId AND feed_id in @Feeds", new
            {
                UserId = userId,
                Feeds = feeds
            });
        }

        public static async Task<IEnumerable<ReadRecord>> GetReadRecords(this IDbConnection connection, string userId, IEnumerable<string> articles)
        {
            if (connection == null || articles == null || !articles.Any() || string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await connection.QueryWithPolly<ReadRecord>("SELECT * FROM readrecord where user_id=@userId AND article_id in @articles;", new
            {
                userId,
                articles
            });
        }

        public static async Task<bool> InsertReadRecords(this IDbConnection connection, IEnumerable<ReadRecord> records)
        {
            if (connection == null || records == null || !records.Any())
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
    
        public static async Task<IEnumerable<Feed>> GetFeeds(this IDbConnection connection)
        {
            if (connection == null)
            {
                return null;
            }
            
            return await connection.QueryWithPolly<Feed>("SELECT * FROM feed");
        }

        public static async Task<Article> GetLastestArticle(this IDbConnection connection, string feedId)
        {
            if (connection == null || string.IsNullOrWhiteSpace(feedId))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<Article>("select * from article a where a.feed_id=@feedId order by a.published desc limit 1;", new {feedId});
        }
    }
}