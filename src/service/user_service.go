package service

import (
	"time"

	"github.com/gbrlsnchs/jwt/v3"
	"github.com/google/uuid"
	"github.com/venyowong/resader/db"
	"github.com/venyowong/resader/helper"
)

type JwtPayload struct {
	jwt.Payload
	Mail   string `json:"mail,omitempty"`
	UserId int    `json:"userid,omitempty"`
}

var hs = jwt.NewHS256([]byte("782d8e17-73b6-4e91-8b75-365836fec388"))

// code int 0 成功 1 创建用户失败 2 密码错误
func Login(mail string, pwd string) (db.User, int) {
	user := db.GetUserByMail(mail)
	if user.Mail == "" { // 未找到用户
		salt := uuid.New().String()
		user = db.User{
			Mail:     mail,
			Salt:     salt,
			Password: helper.Md5(pwd + salt),
		}
		id := db.CreateUser(user)
		if id < 0 {
			return user, 1
		}

		user.Id = int(id)
	} else {
		pwd = helper.Md5(pwd + user.Salt)
		if pwd != user.Password { // 密码错误
			return user, 2
		}
	}

	return user, 0
}

func GenerateToken(mail string, userId int) ([]byte, error) {
	return jwt.Sign(JwtPayload{
		Payload: jwt.Payload{
			Issuer:   "resader",
			Subject:  mail,
			Audience: jwt.Audience{"https://golang.org", "https://jwt.io"},
			IssuedAt: jwt.NumericDate(time.Now()),
		},
		Mail:   mail,
		UserId: userId,
	}, hs)
}

func VerifyToken(token string) JwtPayload {
	var pl JwtPayload
	_, err := jwt.Verify([]byte(token), hs, &pl)
	if err != nil {
		return JwtPayload{}
	}
	return pl
}
