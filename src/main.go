package main

import (
	"github.com/gin-gonic/gin"
	"github.com/robfig/cron"
	"github.com/venyowong/resader/api"
	"github.com/venyowong/resader/db"
	"github.com/venyowong/resader/helper"
	"github.com/venyowong/resader/service"
)

func main() {
	helper.InitLog()
	db.Init()
	defer db.Close()

	c := cron.New()
	c.AddFunc("0 */5 * * * *", service.DoFetchJob)
	c.Start()

	r := gin.Default()
	r.Use(CORS)
	r.Use(Auth)
	r.Static("/html", "./assets/pc")
	r.Static("/mob", "./assets/mob")
	r.Static("/js", "./assets/js")
	r.POST("/user/login", api.UserLogin)
	r.POST("/auth/feed/subscribe", api.SubscribeFeed)
	r.POST("/auth/feed/unsubscribe", api.UnsubscribeFeed)
	r.GET("/auth/feeds", api.GetFeeds)
	r.GET("/auth/feed/articles", api.GetArticles)
	r.POST("/auth/article/read", api.ReadArticle)
	r.GET("/feeds", api.GetFeeds2)
	r.Run()
}
