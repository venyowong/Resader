use crate::db::user::{User, get_user_by_mail, insert_user};
use chrono::Local;
use rusqlite::Error;
use uuid::Uuid;

pub fn login(mail: &str, pwd: &str) -> (Option<User>, i16) {
    let user = get_user_by_mail(mail);
    let user = match user {
        Ok(u) => u,
        Err(Error::QueryReturnedNoRows) => {
            let salt = Uuid::new_v4().to_string();
            let mut u = User {
                id: 0,
                mail: mail.to_string(),
                password: format!("{:x}", md5::compute(pwd.to_string() + &salt)),
                salt: salt,
                create_time: Local::now(),
                update_time: Local::now()
            };
            let id = insert_user(&u);
            let id = match id {
                Ok(i) => i,
                Err(e) => return (None, 1)
            };
            u.id = id;
            return (Some(u), 0)
        },
        Err(_) => return (None, -1)
    };

    if format!("{:x}", md5::compute(pwd.to_string() + &user.salt)) != user.password {
        return (None, 2);
    }
    else {
        return (Some(user), 0);
    }
}