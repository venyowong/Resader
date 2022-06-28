use actix_web::{App, HttpServer};
use actix_files as fs;

pub mod server;
pub mod service;
pub mod db;
pub mod  helper;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    db::init();

    HttpServer::new(|| {
        App::new()
            .service(fs::Files::new("/mob", "./assets/mob"))
            .service(fs::Files::new("/js", "./assets/js"))
            .service(server::user::login)
            .service(server::feed::get_feeds)
    })
    .bind(("127.0.0.1", 19849))?
    .run()
    .await
}