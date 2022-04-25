package api

import (
	"strconv"

	"github.com/gin-gonic/gin"

	"github.com/venyowong/resader/db"
	"github.com/venyowong/resader/helper"
	"github.com/venyowong/resader/service"
)

func SubscribeFeed(c *gin.Context) {
	url := c.PostForm("url")
	if url == "" {
		c.String(403, "rss 链接不能为空")
		return
	}
	userId, v := getUserId(c)
	if !v {
		return
	}

	if !service.SubscribeFeed(userId, url) {
		c.String(500, "订阅失败")
		return
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
	userId, v := getUserId(c)
	if !v {
		return
	}

	if !db.DeleteSubscription(userId, feedId) {
		c.String(500, "取消订阅失败")
		return
	}

	c.JSON(200, gin.H{
		"status": 0,
	})
}

func GetFeeds(c *gin.Context) {
	userId, v := getUserId(c)
	if !v {
		return
	}

	feeds := db.GetFeedsByUser(userId)
	helper.Order(feeds, func(t db.Feed) float64 {
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
