package api

import (
	"strconv"

	"github.com/gin-gonic/gin"

	"github.com/venyowong/resader/db"
	. "github.com/venyowong/resader/helper"
	"github.com/venyowong/resader/service"
)

func SubscribeFeed(c *gin.Context) {
	url := c.PostForm("url")
	if url == "" {
		c.String(403, "rss 链接不能为空")
		return
	}
	jwt, e := c.Get("jwt")
	if !e {
		c.String(401, "token 不合法")
		return
	}
	userId := jwt.(JwtPayload).UserId

	feed := db.GetFeedById(Md5(url))
	if feed.Id == "" { // 新 feed
		var articles []db.Article
		feed, articles = service.Fetch(url)

		db.InsertFeed(feed)
		db.InsertArticles(articles...)
	}

	sub := db.GetSubscription(userId, feed.Id)
	if sub.FeedId == "" { // 未订阅
		sub = db.Subscription{
			UserId: userId,
			FeedId: feed.Id,
		}
		if !db.InsertSubscription(sub) {
			c.String(500, "订阅失败")
			return
		}
	}

	c.JSON(200, gin.H{
		"status": 0,
	})
}

func UnsubscribeFeed(c *gin.Context) {
	feedId := c.PostForm("feed")
	if feedId == "" {
		c.String(403, "feed id 不能为空")
		return
	}
	jwt, e := c.Get("jwt")
	if !e {
		c.String(401, "token 不合法")
		return
	}
	userId := jwt.(JwtPayload).UserId

	if !db.DeleteSubscription(userId, feedId) {
		c.String(500, "取消订阅失败")
		return
	}

	c.JSON(200, gin.H{
		"status": 0,
	})
}

func GetFeeds(c *gin.Context) {
	jwt, e := c.Get("jwt")
	if !e {
		c.String(401, "token 不合法")
		return
	}
	userId := jwt.(JwtPayload).UserId
	feeds := db.GetFeedsByUser(userId)
	Order(feeds, func(t db.Feed) float64 {
		return float64(t.CreateTime.Unix())
	}, true)
	c.JSON(200, gin.H{
		"status": 0,
		"data":   feeds,
	})
}

func GetFeeds2(c *gin.Context) {
	userId, _ := strconv.Atoi(c.Query("user"))
	feeds := db.GetFeedsByUser(userId)
	c.JSON(200, feeds)
}
