package main

import (
	"./handlers"
	"github.com/gin-gonic/gin"
	"github.com/gin-gonic/contrib/cors"

)

func main() {

	r := gin.Default()
	r.Use(cors.Default())
	r.POST("/set", handlers.SetToIpfs)
	r.GET("/get/:hash", handlers.GetFromIpfs)
	r.Run()
}



