package api

import (
	"log"

	"github.com/gin-gonic/gin"

	"github.com/venyowong/resader/service"
)

func UserLogin(c *gin.Context) {
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

	user, code := service.Login(mail, pwd)
	switch code {
	case 1:
		c.String(500, "创建用户失败")
		return
	case 2:
		c.String(401, "密码错误")
		return
	}

	token, err := service.GenerateToken(mail, user.Id)
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

func getUserId(c *gin.Context) (int, bool) {
	jwt, e := c.Get("jwt")
	if !e {
		c.String(401, "token 不合法")
		return 0, false
	}
	return jwt.(service.JwtPayload).UserId, true
}
