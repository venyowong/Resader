use rusqlite::Error;

use chrono::prelude::*;
use rusqlite::Connection;
use rusqlite::Row;
use rusqlite::params;
use crate::helper;

pub struct User {
    pub id: i64,
    pub mail: String,
    pub password: String,
    pub salt: String,
    pub create_time: DateTime<Local>,
    pub update_time: DateTime<Local>
}

pub fn get_user_by_mail(mail: &str) -> Result<User, Error> {
    let conn = Connection::open(super::DB_NAME)?;
    conn.query_row("SELECT * FROM user WHERE mail=?1", params![mail], get_user)
}

pub fn insert_user(user: &User) -> Result<i64, Error> {
    let conn = Connection::open(super::DB_NAME)?;
    conn.execute("INSERT INTO user(mail, password, salt) VALUES(?1, ?2, ?3)", 
        params![user.mail, user.password, user.salt])?;
    Ok(conn.last_insert_rowid())
}

fn get_user(row: &Row) -> Result<User, Error> {
    let create_time: String = row.get(4)?;
    let update_time: String = row.get(5)?;
    Ok(User {
        id: row.get(0)?,
        mail: row.get(1)?,
        password: row.get(2)?,
        salt: row.get(3)?,
        create_time: helper::parse_datetime_from_str(&create_time),
        update_time: helper::parse_datetime_from_str(&update_time)
    })
}