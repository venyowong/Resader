using Microsoft.Extensions.Options;
using Resader.Api.Extensions;
using Resader.Api.Factories;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Daos;
public class RssDao
{
    private DbConnectionFactory connectionFactory;
    private Configuration config;

    static RssDao()
    {
        Utility.MakeDapperMapping(typeof(Article), typeof(Feed), typeof(Subscription), typeof(ReadRecord), typeof(FeedBrowseRecord));
    }

    public RssDao(DbConnectionFactory connectionFactory, IOptions<Configuration> config)
    {
        this.connectionFactory = connectionFactory;
        this.config = config.Value;
    }

    #region article

    public async Task<List<Article>> GetArticles(string feedId)
    {
        if (string.IsNullOrWhiteSpace(feedId))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        if (config.ArticleMonths > 0)
        {
            return (await connection.QueryWithPolly<Article>($@"SELECT * FROM article WHERE feed_id=@FeedId AND 
                    DATEDIFF(create_time, DATE_ADD(NOW(), INTERVAL {-config.ArticleMonths} MONTH))>=0", new { FeedId = feedId })).ToList();
        }
        else
        {
            return (await connection.QueryWithPolly<Article>("SELECT * FROM article WHERE feed_id=@FeedId", new { FeedId = feedId })).ToList();
        }
    }

    public async Task<bool> InsertArticle(params Article[] articles)
    {
        if (articles.IsNullOrEmpty())
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("INSERT INTO article(id, url, feed_id, title, summary, published, updated, keyword, content, contributors, " +
            "authors, copyright, create_time, update_time) VALUES(@Id, @Url, @FeedId, @Title, @Summary, @Published, @Updated, @Keyword, @Content, " +
            "@Contributors, @Authors, @Copyright, now(), now())", articles);
    }

    public async Task<bool> UpdateArticle(Article article)
    {
        if (article == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("UPDATE article SET title=@Title, summary=@Summary, published=@Published, updated=@Updated, " +
            "keyword=@Keyword, content=@Content, contributors=@Contributors, authors=@Authors, copyright=@Copyright, update_time=now() WHERE id=@Id", article);
    }
    #endregion

    #region feed
    public async Task<bool> InsertFeed(Feed feed)
    {
        if (feed == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly(@"INSERT INTO feed(id, url, title, description, image , label, create_time, update_time) 
                VALUES(@Id, @Url, @Title, @Description, @Image, @Label, now(), now())", feed);
    }

    public async Task<IEnumerable<Feed>> GetFeeds()
    {
        using var connection = await connectionFactory.Create();
        return await connection.QueryWithPolly<Feed>("SELECT * FROM feed");
    }

    public async Task<bool> UpdateFeed(Feed feed)
    {
        if (feed == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("UPDATE feed SET title=@Title, update_time=now(), description=@Description, image=@Image, label=@Label WHERE  id=@Id", feed);
    }
    #endregion

    #region subscription
    public async Task<IEnumerable<Subscription>> GetSubscriptions(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        return await connection.QueryWithPolly<Subscription>("SELECT * FROM subscription WHERE user_id=@userId", new { userId });
    }

    public async Task<bool> InsertSubscription(Subscription subscription)
    {
        if (subscription == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("INSERT INTO subscription(user_id, feed_id, create_time, update_time) VALUES(@UserId, @FeedId, now(), now())", subscription);
    }

    public async Task<bool> DeleteSubscriptions(List<string> feeds, string userId)
    {
        if (feeds.IsNullOrEmpty() || string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("DELETE FROM subscription WHERE user_id=@UserId AND feed_id in @Feeds", new
        {
            UserId = userId,
            Feeds = feeds
        });
    }
    #endregion

    #region record
    public async Task<ReadRecord> GetReadRecord(string userId, string articleId)
    {
        if (string.IsNullOrWhiteSpace(articleId) || string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        return await connection.QueryFirstOrDefaultWithPolly<ReadRecord>("SELECT * FROM readrecord WHERE user_id=@userId AND article_id=@articleId", new
        {
            userId,
            articleId
        });
    }

    public async Task<IEnumerable<ReadRecord>> GetReadRecords(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        return await connection.QueryWithPolly<ReadRecord>("SELECT * FROM readrecord WHERE user_id=@userId", new { userId });
    }

    public async Task<bool> InsertReadRecord(ReadRecord record)
    {
        if (record == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("INSERT INTO readrecord(article_id, user_id, create_time, update_time) VALUES(@ArticleId, @UserId, now(), now())", record);
    }

    public async Task<bool> UpdateReadRecord(ReadRecord record)
    {
        if (record == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("UPDATE readrecord SET update_time=now() WHERE user_id=@UserId AND article_id=@ArticleId", record);
    }

    public async Task<FeedBrowseRecord> GetFeedBrowseRecord(string userId, string feedId)
    {
        if (string.IsNullOrWhiteSpace(feedId) || string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        return await connection.QueryFirstOrDefaultWithPolly<FeedBrowseRecord>("SELECT * FROM feed_browse_record WHERE user_id=@userId AND feed_id=@feedId", new
        {
            userId,
            feedId
        });
    }

    public async Task<IEnumerable<FeedBrowseRecord>> GetFeedBrowseRecords(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        return await connection.QueryWithPolly<FeedBrowseRecord>("SELECT * FROM feed_browse_record WHERE user_id=@userId", new { userId });
    }

    public async Task<bool> InsertFeedBrowseRecord(FeedBrowseRecord record)
    {
        if (record == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("INSERT INTO feed_browse_record(feed_id, user_id, create_time, update_time) VALUES(@FeedId, @UserId, now(), now())", record);
    }

    public async Task<bool> UpdateFeedBrowseRecord(FeedBrowseRecord record)
    {
        if (record == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("UPDATE feed_browse_record SET update_time=now() WHERE user_id=@UserId AND feed_id=@FeedId", record);
    }
    #endregion
}
