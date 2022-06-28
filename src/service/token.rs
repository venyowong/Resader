use chrono::{Local};
use hmac::{Hmac, Mac};
use sha2::Sha256;
use std::collections::BTreeMap;
use jwt::{SignWithKey, VerifyWithKey, error::Error};
use crate::helper;
use lazy_static::lazy_static;

struct JwtKey {
    key: Hmac<Sha256>
}

lazy_static! {
    static ref JWT_KEY: JwtKey = JwtKey {
        key: Hmac::new_from_slice(b"782d8e17-73b6-4e91-8b75-365836fec388").unwrap()
    };
}

pub fn generate_token(mail: &str, user_id: i64) -> Result<String, Error> {
    let mut claims = BTreeMap::new();
    claims.insert("iss", "resader");
    claims.insert("sub", mail);
    let iat = helper::format_datetime(&Local::now());
    claims.insert("iat", &iat);
    claims.insert("mail", mail);
    let uid = user_id.to_string();
    claims.insert("user_id", &uid);
    claims.sign_with_key(&JWT_KEY.key)
}

pub fn verify_token(token: &str) -> Result<BTreeMap<String, String>, Error> {
    token.verify_with_key(&JWT_KEY.key)
}