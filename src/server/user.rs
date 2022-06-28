use actix_web::{get, post, HttpResponse, Responder, web};
use serde::{Deserialize, Serialize};
use crate::service::{user, token::{generate_token}};

#[derive(Deserialize)]
pub struct LoginParams {
    mail: String,
    password: String
}

#[derive(Serialize)]
pub struct LoginResult<'a> {
    mail: &'a str,
    #[serde(rename="userId")]
    user_id: i64,
    token: String
}

#[post("/user/login")]
pub async fn login(p: web::Form<LoginParams>) -> impl Responder {
    let (user, code) = user::login(&p.mail, &p.password);
    if code == -1 || code == 1 {
        return HttpResponse::InternalServerError().body("数据库操作失败");
    }
    else if code == 2 {
        return HttpResponse::Unauthorized().body("密码错误");
    }

    let user = user.unwrap();
    let token = generate_token(&p.mail, user.id);
    let token = match token {
        Ok(t) => t,
        Err(_) => return HttpResponse::InternalServerError().body("生成 token 失败")
    };
    let result = LoginResult {
        mail: &p.mail,
        user_id: user.id,
        token: token
    };
    HttpResponse::Ok().json(result)
}