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