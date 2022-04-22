package db

import (
	"database/sql"
	"log"
	"time"
)

func InsertFeed(feed Feed) bool {
	result, err := db.Exec("INSERT INTO feed(id, url, title, description, image) VALUES(?, ?, ?, ?, ?)",
		feed.Id, feed.Url, feed.Title, feed.Description, feed.Image)
	if err != nil {
		log.Printf("插入 feed 失败 %s", err)
		return false
	}
	row, err := result.RowsAffected()
	if err != nil {
		log.Printf("插入 feed 失败 %s", err)
		return false
	}
	return row > 0
}

func GetFeeds() []Feed {
	rows, err := db.Query("SELECT * FROM feed")
	if err != nil {
		log.Printf("GetFeeds 失败 %s", err)
		return nil
	}
	defer rows.Close()

	return ToList(rows, getFeedFromRows)
}

func GetFeedsByUser(userId int) []Feed {
	rows, err := db.Query("SELECT * FROM feed WHERE id IN (SELECT feed_id FROM subscription WHERE user_id=?)", userId)
	if err != nil {
		log.Printf("GetFeeds 失败 %s", err)
		return nil
	}
	defer rows.Close()

	return ToList(rows, getFeedFromRows)
}

func GetFeedById(id string) Feed {
	row := db.QueryRow("SELECT * FROM feed WHERE id=?", id)
	return getFeedByRow(row)
}

func getFeedFromRows(r *sql.Rows) Feed {
	var id string
	var url string
	var title string
	var description string
	var image string
	var createTime time.Time
	var updateTime time.Time
	r.Scan(&id, &url, &title, &description, &image, &createTime, &updateTime)
	return Feed{
		Id:          id,
		Url:         url,
		Title:       title,
		Description: description,
		Image:       image,
		CreateTime:  createTime,
		UpdateTime:  updateTime,
	}
}

func getFeedByRow(r *sql.Row) Feed {
	if r == nil {
		return Feed{}
	}

	var id string
	var url string
	var title string
	var desc string
	var image string
	var createTime time.Time
	var updateTime time.Time
	r.Scan(&id, &url, &title, &desc, &image, &createTime, &updateTime)
	return Feed{
		Id:          id,
		Url:         url,
		Title:       title,
		Description: desc,
		Image:       image,
		CreateTime:  createTime,
		UpdateTime:  updateTime,
	}
}
