package api

import (
	"strconv"

	"github.com/gin-gonic/gin"
	"github.com/venyowong/resader/db"
)

func GetArticles(c *gin.Context) {
	feedId := c.Query("feed")
	if feedId == "" {
		c.String(403, "feed id 不能为空")
		return
	}
	userId, v := getUserId(c)
	if !v {
		return
	}

	db.InsertFeedBrowseRecord(userId, feedId)
	onlyunread := c.Query("onlyunread")
	read := 2
	if onlyunread == "true" {
		read = 1
	}
	page, _ := strconv.Atoi(c.Query("page"))
	perPage, _ := strconv.Atoi(c.Query("perPage"))

	articles := db.GetArticles(userId, feedId, read)
	l := len(articles)
	b := (page - 1) * perPage
	e := b + perPage
	if e > l {
		e = l
	}
	c.JSON(200, gin.H{
		"status": 0,
		"data": gin.H{
			"total": l,
			"data":  articles[b:e],
		},
	})
}

func ReadArticle(c *gin.Context) {
	articleId := c.PostForm("article")
	if articleId == "" {
		c.String(403, "文章 id 不能为空")
		return
	}
	userId, v := getUserId(c)
	if !v {
		return
	}

	db.InsertReadRecord(db.ReadRecord{
		UserId:    userId,
		ArticleId: articleId,
	})
	c.Status(200)
}
