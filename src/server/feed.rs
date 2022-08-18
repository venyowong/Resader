use std::cmp::Ordering;

use actix_web::{Responder, HttpResponse, get, HttpRequest, web, post};
use chrono::Local;
use rusqlite::Error;
use serde::{Serialize, Deserialize};
use futures::executor::block_on;

use crate::{db::feed::{get_feeds_by_user, Feed, get_feed_by_id, insert_feed}, helper::{self, md5}, service::feed::fetch};

use super::handle_with_token;

#[derive(Serialize)]
struct FeedsResult {
    status: i32,
    data: Vec<FeedModel>
}

#[derive(Serialize)]
struct FeedModel {
    id: String,
    url: String,
    title: String,
    desc: String,
    image: String,
    active: bool,
    create_time: String,
    update_time: String
}

#[derive(Deserialize)]
pub struct SubscribeParams {
    url: String
}

#[get("/auth/feeds")]
pub async fn get_feeds(req: HttpRequest) -> impl Responder {
    return handle_with_token(&req, |r, payload| {
        let feeds = get_feeds_by_user(payload["user_id"].parse::<i64>().unwrap());
        let mut feeds = match feeds {
            Ok(f) => f,
            Err(e) => return HttpResponse::InternalServerError().body("查询数据库失败")
        };
        feeds.sort_by(|f1, f2| {
            if f1.create_time > f2.create_time {
                Ordering::Greater
            }
            else if f1.create_time == f2.create_time {
                Ordering::Equal
            }
            else {
                Ordering::Less
            }
        });
        let mut data: Vec<FeedModel> = Vec::new();
        for feed in feeds {
            data.push(convert_feed_model(feed));
        }

        HttpResponse::Ok().json(FeedsResult {
            status: 0,
            data: data
        })
    });
}

#[post("/auth/subscribe/feed")]
pub async fn subscribe_feed(req: HttpRequest, p: web::Form<SubscribeParams>) -> impl Responder {
    return handle_with_token(&req, move |r, payload| {
        let feed_id = md5(&p.url);
        let feed = get_feed_by_id(&feed_id);
        let feed = match feed {
            Ok(f) => f,
            Err(Error::QueryReturnedNoRows) => {
                let channel = block_on(fetch(&p.url));
                let channel = match channel {
                    Ok(c) => c,
                    Err(_) => return HttpResponse::BadRequest().body("获取 feed 失败")
                };

                let img = match channel.image {
                    Some(i) => i.link,
                    None => String::new()
                };
                let f = Feed {
                    id: feed_id.clone(),
                    url: p.url.clone(),
                    title: channel.title,
                    desc: channel.description,
                    image: img,
                    active: false,
                    create_time: Local::now(),
                    update_time: Local::now()
                };
                let success = insert_feed(&f);
                let success = match success {
                    Ok(s) => s,
                    Err(_) => return HttpResponse::BadRequest().body("插入 feed 失败")
                };
                if !success {
                    return HttpResponse::BadRequest().body("插入 feed 失败");
                }

                f
            },
            Err(_) => return HttpResponse::InternalServerError().body("数据库查询失败")
        };
        HttpResponse::Ok().body("")
    });
}

fn convert_feed_model(feed: Feed) -> FeedModel {
    FeedModel {
        id: feed.id,
        url: feed.url,
        title: feed.title,
        desc: feed.desc,
        image: feed.image,
        active: feed.active,
        create_time: helper::format_datetime(&feed.create_time),
        update_time: helper::format_datetime(&feed.update_time)
    }
}