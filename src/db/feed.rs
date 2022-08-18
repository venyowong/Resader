use chrono::{DateTime, Local};
use rusqlite::{Connection, params, Row, Error};

use crate::helper;

pub struct Feed {
    pub id: String,
    pub url: String,
    pub title: String,
    pub desc: String,
    pub image: String,
    pub active: bool,
    pub create_time: DateTime<Local>,
    pub update_time: DateTime<Local>
}

pub fn get_feeds_by_user(user_id: i64) -> Result<Vec<Feed>, Error> {
    let conn = Connection::open(super::DB_NAME)?;
    let mut stmt = conn.prepare("SELECT f.*, b.t >= IFNULL(a.t, DATETIME('1990-01-01')) Active FROM feed f
	LEFT JOIN (SELECT feed_id, MAX(create_time) t  FROM feed_browse_record WHERE user_id=?1 GROUP BY feed_id) a ON f.id==a.feed_id
	LEFT JOIN (SELECT feed_id, MAX(create_time) t  FROM article GROUP BY feed_id) b ON f.id==b.feed_id
	WHERE f.id IN (SELECT feed_id FROM subscription WHERE user_id=?2)")?;
    let feeds = stmt.query_map(params![user_id, user_id], get_feed)?;
    let mut result: Vec<Feed> = Vec::new();
    for feed in feeds {
        result.push(feed?)
    }
    Ok(result)
}

pub fn get_feed_by_id(id: &str) -> Result<Feed, Error> {
    let conn = Connection::open(super::DB_NAME)?;
    conn.query_row("SELECT * FROM feed WHERE id=?1", params![id], get_feed)
}

pub fn insert_feed(f: &Feed) -> Result<bool, Error> {
    let conn = Connection::open(super::DB_NAME)?;
    let rows = conn.execute("INSERT INTO feed(id, url, title, desc, image) VALUES(?1, ?2, ?3, ?4, ?5)", 
        params![f.id, f.url, f.title, f.desc, f.image])?;
    Ok(rows > 0)
}

fn get_feed(row: &Row) -> Result<Feed, Error> {
    let create_time: String = row.get(6)?;
    let update_time: String = row.get(7)?;
    Ok(Feed {
        id: row.get(0)?,
        url: row.get(1)?,
        title: row.get(2)?,
        desc: row.get(3)?,
        image: row.get(4)?,
        active: row.get(5)?,
        create_time: helper::parse_datetime_from_str(&create_time),
        update_time: helper::parse_datetime_from_str(&update_time)
    })
}