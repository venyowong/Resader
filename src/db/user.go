package db

import (
	"database/sql"
	"log"
	"time"
)

func GetUserByMail(mail string) User {
	row := db.QueryRow("SELECT * FROM user WHERE mail=?", mail)
	return getUserFromRow(row)
}

func CreateUser(user User) int64 {
	result, err := db.Exec("INSERT INTO user(mail, password, salt) VALUES(?, ?, ?)",
		user.Mail, user.Password, user.Salt)
	if err != nil {
		log.Printf("插入 user 失败 %s", err)
		return -1
	}
	id, err := result.LastInsertId()
	if err != nil {
		log.Printf("插入 user 失败 %s", err)
		return -1
	}
	return id
}

func InsertSubscription(sub Subscription) bool {
	result, err := db.Exec("INSERT INTO subscription(user_id, feed_id) VALUES(?, ?)",
		sub.UserId, sub.FeedId)
	if err != nil {
		log.Printf("插入 Subscription 失败 %s", err)
		return false
	}
	r, err := result.RowsAffected()
	if err != nil {
		log.Printf("插入 Subscription 失败 %s", err)
		return false
	}
	return r > 0
}

func GetSubscription(userId int, feedId string) Subscription {
	row := db.QueryRow("SELECT * FROM subscription WHERE user_id=? AND feed_id=?", userId, feedId)
	return getSubscriptionByRow(row)
}

func DeleteSubscription(userId int, feedId string) bool {
	result, err := db.Exec("DELETE FROM subscription WHERE user_id=? AND feed_id=?", userId, feedId)
	if err != nil {
		log.Printf("删除 Subscription 失败 %s", err)
		return false
	}
	r, err := result.RowsAffected()
	if err != nil {
		log.Printf("删除 Subscription 失败 %s", err)
		return false
	}
	return r > 0
}

func getUserFromRow(r *sql.Row) User {
	if r == nil {
		return User{}
	}

	var id int
	var mail string
	var pwd string
	var salt string
	var createTime time.Time
	var updateTime time.Time
	r.Scan(&id, &mail, &pwd, &salt, &createTime, &updateTime)
	return User{
		Id:         id,
		Mail:       mail,
		Password:   pwd,
		Salt:       salt,
		CreateTime: createTime,
		UpdateTime: updateTime,
	}
}

func getSubscriptionByRow(r *sql.Row) Subscription {
	if r == nil {
		return Subscription{}
	}

	var id int
	var userId int
	var feedId string
	var createTime time.Time
	var updateTime time.Time
	r.Scan(&id, &userId, &feedId, &createTime, &updateTime)
	return Subscription{
		Id:         id,
		UserId:     userId,
		FeedId:     feedId,
		CreateTime: createTime,
		UpdateTime: updateTime,
	}
}
