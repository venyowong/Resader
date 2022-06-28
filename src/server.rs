use std::collections::BTreeMap;

use actix_web::{HttpResponse, HttpRequest};
use futures_util::Future;

use crate::service::token::verify_token;

pub mod user;
pub mod feed;

pub fn handle_with_token<F>(req: &HttpRequest, func: F) -> HttpResponse
where F: Fn(&HttpRequest, &BTreeMap<String, String>) -> HttpResponse
{
    let token = req.headers().get("token");
    let token = match token {
        Some(t) => t.to_str().unwrap(),
        None => {
            return HttpResponse::Forbidden().body("未获取到 token");
        }
    };

    let payload = verify_token(token);
    let payload = match payload {
        Ok(pl) => pl,
        Err(_) => {
            return HttpResponse::Unauthorized().body("token 不合法");
        }
    };

    return func(req, &payload);
}

pub async fn handle_with_token_async<F>(req: &HttpRequest, func: F) -> HttpResponse
where F: Fn(&HttpRequest, &BTreeMap<String, String>) -> Box<dyn Future<Output = HttpResponse>>
{
    let token = req.headers().get("token");
    let token = match token {
        Some(t) => t.to_str().unwrap(),
        None => {
            return HttpResponse::Forbidden().body("未获取到 token");
        }
    };

    let payload = verify_token(token);
    let payload = match payload {
        Ok(pl) => pl,
        Err(_) => {
            return HttpResponse::Unauthorized().body("token 不合法");
        }
    };

    let r = func(req, &payload);
    r.await
}