package api

import (
	"log"
	"time"

	"github.com/gbrlsnchs/jwt/v3"
	"github.com/gin-gonic/gin"
	"github.com/google/uuid"

	"github.com/venyowong/resader/db"
	. "github.com/venyowong/resader/helper"
)

type JwtPayload struct {
	jwt.Payload
	Mail   string `json:"mail,omitempty"`
	UserId int    `json:"userid,omitempty"`
}

var hs = jwt.NewHS256([]byte("782d8e17-73b6-4e91-8b75-365836fec388"))

func Login(c *gin.Context) {
	mail := c.PostForm("mail")
	if mail == "" {
		c.String(403, "邮箱不能为空")
		return
	}
	pwd := c.PostForm("password")
	if pwd == "" {
		c.String(403, "密码不能为空")
		return
	}

	user := db.GetUserByMail(mail)
	if user.Mail == "" { // 未找到用户
		salt := uuid.New().String()
		user = db.User{
			Mail:     mail,
			Salt:     salt,
			Password: Md5(pwd + salt),
		}
		id := db.CreateUser(user)
		if id < 0 {
			c.String(500, "创建用户失败")
			return
		}

		user.Id = int(id)
	} else {
		pwd = Md5(pwd + user.Salt)
		if pwd != user.Password { // 密码错误
			c.String(401, "密码错误")
			return
		}
	}

	token, err := jwt.Sign(JwtPayload{
		Payload: jwt.Payload{
			Issuer:   "resader",
			Subject:  mail,
			Audience: jwt.Audience{"https://golang.org", "https://jwt.io"},
			IssuedAt: jwt.NumericDate(time.Now()),
		},
		Mail:   mail,
		UserId: user.Id,
	}, hs)
	if err != nil {
		log.Printf("生成 token 失败 %s", err)
		c.String(500, "生成 token 失败")
		return
	}

	c.JSON(200, gin.H{
		"mail":   mail,
		"userId": user.Id,
		"token":  string(token),
	})
}

func VerifyToken(token string) JwtPayload {
	var pl JwtPayload
	_, err := jwt.Verify([]byte(token), hs, &pl)
	if err != nil {
		return JwtPayload{}
	}
	return pl
}
