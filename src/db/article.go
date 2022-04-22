package db

import (
	"database/sql"
	"fmt"
	"log"
	"time"
)

func InsertArticles(articles ...Article) bool {
	tx, err := db.Begin()
	if err != nil {
		log.Println(err.Error())
		return false
	}
	stmt, err := tx.Prepare("INSERT INTO article(id, url, feed_id, title, summary, published, keyword, content, contributors, authors, image) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)")
	if err != nil {
		log.Println(err.Error())
		return false
	}
	defer stmt.Close()

	for _, article := range articles {
		stmt.Exec(article.Id, article.Url, article.FeedId, article.Title, article.Summary, article.Published, article.Keyword, article.Content, article.Contributors, article.Authors, article.Image)
	}
	tx.Commit()
	return true
}

// read 0 已读 1 未读 2 全部
func GetArticles(userId int, feedId string, read int) []Article {
	var condition string
	switch read {
	case 0:
		condition = fmt.Sprintf("AND id IN (SELECT article_id FROM readrecord WHERE user_id=%d)", userId)
	case 1:
		condition = fmt.Sprintf("AND id NOT IN (SELECT article_id FROM readrecord WHERE user_id=%d)", userId)
	}
	rows, err := db.Query(fmt.Sprintf("SELECT * FROM article WHERE feed_id='%s' %s ORDER BY create_time DESC", feedId, condition))
	if err != nil {
		log.Printf("GetArticles 失败 %s", err)
		return nil
	}
	defer rows.Close()

	return ToList(rows, getArticleFromRows)
}

func InsertReadRecord(record ReadRecord) {
	db.Exec("INSERT INTO readrecord(user_id, article_id) VALUES(?, ?)",
		record.UserId, record.ArticleId)
}

func getArticleFromRows(r *sql.Rows) Article {
	var id string
	var url string
	var feedId string
	var title string
	var summary string
	var published string
	var keyword string
	var content string
	var contributors string
	var authors string
	var image string
	var createTime time.Time
	var updateTime time.Time
	r.Scan(&id, &url, &feedId, &title, &summary, &published, &keyword, &content,
		&contributors, &authors, &image, &createTime, &updateTime)
	return Article{
		Id:           id,
		Url:          url,
		FeedId:       feedId,
		Title:        title,
		Summary:      summary,
		Published:    published,
		Keyword:      keyword,
		Content:      content,
		Contributors: contributors,
		Authors:      authors,
		Image:        image,
		CreateTime:   createTime,
		UpdateTime:   updateTime,
	}
}
