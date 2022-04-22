package db

import (
	"database/sql"
	"fmt"
	"os"

	_ "github.com/mattn/go-sqlite3"
)

var db *sql.DB

func Init() {
	if _, err := os.Stat("./resader.db"); err != nil {
		db, err = sql.Open("sqlite3", "./resader.db")
		if err != nil {
			fmt.Println(err.Error())
			return
		}

		_, err := db.Exec(`CREATE TABLE feed(
			id VARCHAR(50) NOT NULL PRIMARY KEY,
			url VARCHAR(200) NOT NULL,
			title VARCHAR(200) NOT NULL,
			description TEXT NULL,
			image VARCHAR(200) NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);
		
		CREATE TABLE article(
			id VARCHAR(100) NOT NULL PRIMARY KEY,
			url VARCHAR(500) NOT NULL,
			feed_id VARCHAR(50) NOT NULL,
			title VARCHAR(200) NOT NULL,
			summary VARCHAR(500),
			published VARCHAR(100),
			keyword VARCHAR(300),
			content VARCHAR(500),
			contributors VARCHAR(100),
			authors VARCHAR(100),
			image VARCHAR(200) NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);
		
		CREATE INDEX feed_id_index ON article(feed_id);
		
		CREATE TABLE subscription(
			id INTEGER PRIMARY KEY AUTOINCREMENT,
			user_id int NOT NULL,
			feed_id VARCHAR(50) NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);
		
		CREATE UNIQUE INDEX user_feed_index ON subscription(user_id, feed_id);
		
		CREATE TABLE user(
			id INTEGER PRIMARY KEY AUTOINCREMENT,
			mail VARCHAR(50) NOT NULL UNIQUE,
			password VARCHAR(50) NOT NULL,
			salt VARCHAR(50) NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);
		
		CREATE TABLE readrecord(
			article_id VARCHAR(100) NOT NULL,
			user_id int NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);
		
		CREATE UNIQUE INDEX user_article_unique ON readrecord(user_id, article_id);
		
		CREATE TABLE feed_browse_record(
			feed_id VARCHAR(100) NOT NULL,
			user_id VARCHAR(50) NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);
		
		CREATE UNIQUE INDEX user_feed_unique ON feed_browse_record(user_id, feed_id);
		
		CREATE TABLE recommend(
			label VARCHAR(50) NOT NULL,
			feed_id VARCHAR(100) NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);
		
		CREATE UNIQUE INDEX label_feed_unique ON recommend(label, feed_id);`)
		if err != nil {
			fmt.Printf("创建数据库失败 %q", err)
			db.Close()
			os.Remove("resader.db")
			return
		}
	} else {
		db, err = sql.Open("sqlite3", "./resader.db")
		if err != nil {
			fmt.Println(err.Error())
			return
		}
	}
}

func Close() {
	db.Close()
}
