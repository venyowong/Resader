DROP DATABASE IF EXISTS resader;

CREATE DATABASE resader;

USE resader;

CREATE TABLE feed(
    id VARCHAR(50) NOT NULL PRIMARY KEY,
    url VARCHAR(200) NOT NULL,
    title VARCHAR(200) NOT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME NOT NULL
);

ALTER TABLE feed ADD label VARCHAR(50) NULL;
ALTER TABLE feed ADD description TEXT NULL;
ALTER TABLE feed ADD image VARCHAR(200) NULL;

CREATE TABLE article(
    id VARCHAR(100) NOT NULL PRIMARY KEY,
    url VARCHAR(200) NOT NULL,
    feed_id VARCHAR(50) NOT NULL,
    title VARCHAR(200) NOT NULL,
    summary VARCHAR(500),
    published DATETIME,
    updated DATETIME,
    created DATETIME NOT NULL,
    keyword VARCHAR(300),
    content VARCHAR(500),
    contributors VARCHAR(100),
    authors VARCHAR(100),
    copyright VARCHAR(100),
    create_time DATETIME NOT NULL,
    update_time DATETIME NOT NULL
);

ALTER TABLE article ADD INDEX feed_id_index(feed_id);
ALTER TABLE article DROP created;
ALTER TABLE article MODIFY url VARCHAR(500) NOT NULL;
ALTER TABLE article ADD image VARCHAR(200) NULL;

CREATE TABLE subscription(
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id VARCHAR(50) NOT NULL,
    feed_id VARCHAR(50) NOT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME NOT NULL
);

ALTER TABLE subscription ADD INDEX user_id_index(user_id);

CREATE TABLE user(
    id VARCHAR(50) NOT NULL PRIMARY KEY,
    mail VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(50) NOT NULL,
    salt VARCHAR(50) NOT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME NOT NULL
);

ALTER TABLE user ADD role int DEFAULT 1;
ALTER TABLE user ADD oauth_id VARCHAR(100);
ALTER TABLE user ADD name VARCHAR(100);
ALTER TABLE user ADD avatar_url VARCHAR(500);
ALTER TABLE user ADD source VARCHAR(50) NOT NULL DEFAULT 'resader';
ALTER TABLE user ADD url VARCHAR(500);
ALTER TABLE user ADD location VARCHAR(50);
ALTER TABLE user ADD company VARCHAR(50);
ALTER TABLE user ADD blog VARCHAR(500);
ALTER TABLE user ADD bio VARCHAR(200);

# 若要修改管理员密码，需要先对明文密码进行 md5 加密，然后加密字符串拼接上 salt，再进行一次 md5 加密，才能得到 password
INSERT INTO user(id, mail, password, salt, role, create_time, update_time) VALUES('eadafe4dcd43488ab0350e226f23195e', 'admin@admin.com', 'f3f9d460d8c3a2d4ffe31859f21c9768', '5c8d6080b70c4c3faaaa26e92b271eef', 0, now(), now());

CREATE TABLE readrecord(
    article_id VARCHAR(100) NOT NULL,
    user_id VARCHAR(50) NOT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME NOT NULL
);

ALTER TABLE readrecord ADD UNIQUE unique_index(user_id, article_id);

CREATE TABLE feed_browse_record(
    feed_id VARCHAR(100) NOT NULL,
    user_id VARCHAR(50) NOT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME NOT NULL
);

ALTER TABLE feed_browse_record ADD UNIQUE unique_index(user_id, feed_id);

CREATE TABLE recommend(
    label VARCHAR(50) NOT NULL,
    feed_id VARCHAR(100) NOT NULL,
    create_time DATETIME NOT NULL,
    update_time DATETIME NOT NULL
);

ALTER TABLE recommend ADD UNIQUE unique_index(label, feed_id);