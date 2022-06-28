use std::fs::File;

use rusqlite::{Connection, params};

pub mod user;
pub mod feed;

static DB_NAME: &str = "resader.db";

pub fn init() {
    let file = File::open(&DB_NAME);
    if let Err(_) = file { // 数据库未初始化
        let conn = Connection::open(DB_NAME).unwrap();
        conn.execute("CREATE TABLE feed(
			id VARCHAR(50) NOT NULL PRIMARY KEY,
			url VARCHAR(200) NOT NULL,
			title VARCHAR(200) NOT NULL,
			description TEXT NULL,
			image VARCHAR(200) NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);", params![]).unwrap();
        conn.execute("CREATE TABLE article(
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
		);", params![]).unwrap();
        conn.execute("CREATE INDEX feed_id_index ON article(feed_id);", params![]).unwrap();
        conn.execute("CREATE TABLE subscription(
			id INTEGER PRIMARY KEY AUTOINCREMENT,
			user_id int NOT NULL,
			feed_id VARCHAR(50) NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);", params![]).unwrap();
        conn.execute("CREATE UNIQUE INDEX user_feed_sub_index ON subscription(user_id, feed_id);", params![]).unwrap();
        conn.execute("CREATE TABLE user(
			id INTEGER PRIMARY KEY AUTOINCREMENT,
			mail VARCHAR(50) NOT NULL UNIQUE,
			password VARCHAR(50) NOT NULL,
			salt VARCHAR(50) NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);", params![]).unwrap();
        conn.execute("CREATE TABLE readrecord(
			article_id VARCHAR(100) NOT NULL,
			user_id int NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);", params![]).unwrap();
        conn.execute("CREATE UNIQUE INDEX user_article_unique ON readrecord(user_id, article_id);", params![]).unwrap();
        conn.execute("CREATE TABLE feed_browse_record(
			feed_id VARCHAR(100) NOT NULL,
			user_id int NOT NULL,
			create_time DATETIME NOT NULL DEFAULT current_timestamp,
			update_time DATETIME NOT NULL DEFAULT current_timestamp
		);", params![]).unwrap();
        conn.execute("CREATE INDEX user_feed_browse_index ON feed_browse_record(user_id, feed_id);", params![]).unwrap();
    }
}