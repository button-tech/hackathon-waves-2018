package main

import (
	"./handlers"
	"github.com/gin-gonic/gin"
)

func main() {

	r := gin.Default()
	r.POST("/set", handlers.SetToIpfs)
	r.POST("/get", handlers.GetFromIpfs)
	r.Run()
}
