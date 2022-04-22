package db

import "time"

type Feed struct {
	Id          string
	Url         string
	Title       string
	Description string
	Image       string
	CreateTime  time.Time
	UpdateTime  time.Time
}

func (f Feed) Key() string {
	return f.Id
}

type Article struct {
	Id           string
	Url          string
	FeedId       string
	Title        string
	Summary      string
	Published    string
	Keyword      string
	Content      string
	Contributors string
	Authors      string
	Image        string
	CreateTime   time.Time
	UpdateTime   time.Time
}

type User struct {
	Id         int
	Mail       string
	Password   string
	Salt       string
	CreateTime time.Time
	UpdateTime time.Time
}

type Subscription struct {
	Id         int
	UserId     int
	FeedId     string
	CreateTime time.Time
	UpdateTime time.Time
}

type ReadRecord struct {
	UserId     int
	ArticleId  string
	CreateTime time.Time
	UpdateTime time.Time
}
