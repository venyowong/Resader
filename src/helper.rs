use chrono::{DateTime, Local, LocalResult};
use chrono::prelude::*;

static DATETIME_FMT: &str = "%Y-%m-%d %H-%M-%S";

pub fn parse_datetime_from_str(s: &str) -> DateTime<Local> {
    let naive = NaiveDateTime::parse_from_str(s, DATETIME_FMT);
    let naive = match naive {
        Ok(datetime) => datetime,
        Err(e) => return Local::now()
    };

    let result = Local.from_local_datetime(&naive);
    match result {
        LocalResult::Single(datetime) => datetime,
        _ => return Local::now()
    }
}

pub fn format_datetime(datetime: &DateTime<Local>) -> String {
    datetime.format(DATETIME_FMT).to_string()
}

pub fn md5(s: &str) -> String {
    format!("{:x}", md5::compute(s))
}