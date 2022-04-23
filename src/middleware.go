package main

import (
	"net/http"
	"strings"

	"github.com/gin-gonic/gin"
	"github.com/venyowong/resader/service"
)

func CORS(c *gin.Context) {

	// First, we add the headers with need to enable CORS
	// Make sure to adjust these headers to your needs
	c.Header("Access-Control-Allow-Origin", "*")
	c.Header("Access-Control-Allow-Methods", "*")
	c.Header("Access-Control-Allow-Headers", "*")

	// Second, we handle the OPTIONS problem
	if c.Request.Method != "OPTIONS" {

		c.Next()

	} else {

		// Everytime we receive an OPTIONS request,
		// we just return an HTTP 200 Status Code
		// Like this, Angular can now do the real
		// request using any other method than OPTIONS
		c.AbortWithStatus(http.StatusOK)
	}
}

func Auth(c *gin.Context) {
	if !strings.Contains(c.Request.URL.Path, "/auth/") {
		c.Next()
		return
	}

	token := c.Request.Header.Get("token")
	if token == "" {
		c.AbortWithStatus(403)
		return
	}

	pl := service.VerifyToken(token)
	if pl.Mail == "" {
		c.AbortWithStatus(401)
		return
	}

	c.Set("jwt", pl)
	c.Next()
}
