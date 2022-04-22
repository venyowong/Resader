package service

import (
	"log"
	"regexp"
	"strings"

	"github.com/mmcdole/gofeed"

	. "github.com/venyowong/resader/db"
	. "github.com/venyowong/resader/helper"
)

var imgReg, _ = regexp.Compile("<img src=\"([^\"]+)\"")

func Fetch(url string) (Feed, []Article) {
	parser := gofeed.NewParser()
	feed, err := parser.ParseURL(url)
	if err != nil {
		log.Printf("%s 解析失败 %s", url, err)
		return Feed{}, nil
	}

	feedId := Md5(url)
	f := Feed{
		Id:          feedId,
		Url:         url,
		Title:       feed.Title,
		Description: feed.Description,
	}
	if feed.Image != nil {
		f.Image = feed.Image.URL
	}
	articles := []Article{}
	for _, item := range feed.Items {
		article := Article{
			Id:        Md5(item.Link),
			Url:       item.Link,
			FeedId:    feedId,
			Title:     item.Title,
			Published: item.Published,
			Content:   simplify(item.Content),
			Authors: strings.Join(Select(item.Authors, func(t *gofeed.Person) string {
				return t.Name
			}), ","),
		}
		if article.Content == "" {
			article.Content = simplify(item.Description)
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

func simplify(input string) string {
	length := len(input)
	if length <= 500 {
		return input
	}

	return input[0:length-3] + "..."
}
