package main

import (
	"log"
	"net/http"
	"strings"

	"github.com/gin-gonic/gin"
	"github.com/leemcloughlin/logfile"
	"github.com/robfig/cron"
	"github.com/venyowong/resader/api"
	"github.com/venyowong/resader/db"
	"github.com/venyowong/resader/service"
)

func cors(c *gin.Context) {

	// First, we add the headers with need to enable CORS
	// Make sure to adjust these headers to your needs
	c.Header("Access-Control-Allow-Origin", "*")
	c.Header("Access-Control-Allow-Methods", "*")
	c.Header("Access-Control-Allow-Headers", "*")

	// Second, we handle the OPTIONS problem
	if c.Request.Method != "OPTIONS" {

		c.Next()

	} else {

		// Everytime we receive an OPTIONS request,
		// we just return an HTTP 200 Status Code
		// Like this, Angular can now do the real
		// request using any other method than OPTIONS
		c.AbortWithStatus(http.StatusOK)
	}
}

func auth(c *gin.Context) {
	if !strings.Contains(c.Request.URL.Path, "/auth/") {
		c.Next()
		return
	}

	token := c.Request.Header.Get("token")
	if token == "" {
		c.AbortWithStatus(403)
		return
	}

	pl := api.VerifyToken(token)
	if pl.Mail == "" {
		c.AbortWithStatus(401)
		return
	}

	c.Set("jwt", pl)
	c.Next()
}

func main() {
	logFile, err := logfile.New(
		&logfile.LogFile{
			FileName: "log.txt",
			MaxSize:  500 * 1024, // 500K duh!
			Flags:    logfile.FileOnly | logfile.OverWriteOnStart})
	if err != nil {
		log.Fatalf("Failed to create logFile: %s\n", err)
	}

	log.SetOutput(logFile)

	db.Init()
	defer db.Close()

	c := cron.New()
	c.AddFunc("0 */5 * * * *", func() {
		log.Println("开始抓取 RSS")
		feeds := db.GetFeeds()
		for _, feed := range feeds {
			_, articles := service.Fetch(feed.Url)
			if articles == nil {
				log.Printf("未获取到文章 %s", feed)
				continue
			}

			db.InsertArticles(articles...)
		}
	})
	c.Start()

	gin.SetMode(gin.ReleaseMode)
	r := gin.Default()
	r.Use(cors)
	r.Use(auth)
	r.Static("/html", "./assets/pc")
	r.Static("/mob", "./assets/mob")
	r.Static("/js", "./assets/js")
	r.POST("/user/login", api.Login)
	r.POST("/auth/feed/subscribe", api.SubscribeFeed)
	r.POST("/auth/feed/unsubscribe", api.UnsubscribeFeed)
	r.GET("/auth/feeds", api.GetFeeds)
	r.GET("/auth/feed/articles", api.GetArticles)
	r.POST("/auth/article/read", api.ReadArticle)
	r.GET("/feeds", api.GetFeeds2)
	r.Run()
}
