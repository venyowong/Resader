package service

import (
	"log"
	"regexp"
	"strings"

	"github.com/mmcdole/gofeed"
	"github.com/venyowong/resader/db"
	"github.com/venyowong/resader/helper"
)

var imgReg, _ = regexp.Compile("<img src=\"([^\"]+)\"")

func DoFetchJob() {
	log.Println("开始抓取 RSS")
	feeds := db.GetFeeds()
	for _, feed := range feeds {
		_, articles := Fetch(feed.Url)
		if articles == nil {
			log.Printf("未获取到文章 %s", feed)
			continue
		}

		db.InsertArticles(articles...)
	}
	log.Println("抓取 RSS 结束")
}

func Fetch(url string) (db.Feed, []db.Article) {
	parser := gofeed.NewParser()
	feed, err := parser.ParseURL(url)
	if err != nil {
		log.Printf("%s 解析失败 %s", url, err)
		return db.Feed{}, nil
	}

	feedId := helper.Md5(url)
	f := db.Feed{
		Id:          feedId,
		Url:         url,
		Title:       feed.Title,
		Description: feed.Description,
	}
	if feed.Image != nil {
		f.Image = feed.Image.URL
	}
	articles := []db.Article{}
	for _, item := range feed.Items {
		article := db.Article{
			Id:        helper.Md5(item.Link),
			Url:       item.Link,
			FeedId:    feedId,
			Title:     item.Title,
			Published: item.Published,
			Content:   item.Content,
			Authors: strings.Join(helper.Select(item.Authors, func(t *gofeed.Person) string {
				return t.Name
			}), ","),
		}
		if article.Content == "" {
			article.Content = item.Description
		}
		if item.Image != nil {
			article.Image = item.Image.URL
		}
		if article.Image == "" {
			ms := imgReg.FindStringSubmatch(item.Content)
			if ms != nil {
				article.Image = ms[1]
			}
		}
		if item.ITunesExt != nil {
			article.Summary = item.ITunesExt.Summary
			article.Keyword = item.ITunesExt.Keywords
		}
		if item.DublinCoreExt != nil {
			article.Contributors = strings.Join(item.DublinCoreExt.Contributor, ",")
		}
		articles = append(articles, article)
	}
	return f, articles
}

func SubscribeFeed(userId int, url string) bool {
	feed := db.GetFeedById(helper.Md5(url))
	if feed.Id == "" { // 新 feed
		var articles []db.Article
		feed, articles = Fetch(url)

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
			return false
		}
	}

	return true
}
