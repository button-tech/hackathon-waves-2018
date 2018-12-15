package main

import (
	"github.com/gin-gonic/gin"
	"./handlers"
	"github.com/ipfs/go-ipfs-api"
	"strings"
	"fmt"
	"os"
	"bytes"
	"net/http"
)

func main(){

    r := gin.Default()
    r.POST("/set", handlers.SetToIpfs)
    r.POST("/get",handlers.GetFromIpfs)
    r.GET("/check",func(c *gin.Context){
		sh := shell.NewShell("35.204.142.46:5001")

		cid, err := sh.Add(strings.NewReader("hello world!"))
		if err != nil {
			fmt.Fprintf(os.Stderr, "error: %s", err)
			os.Exit(1)
		}

		cid2, err := sh.Add(strings.NewReader("123"))
		if err != nil {
			fmt.Fprintf(os.Stderr, "error: %s", err)
			os.Exit(1)
		}

		fmt.Println("added %s", cid+"\n"+cid2)

		cat, err := sh.Cat(cid)
		if err != nil {
			fmt.Println(err)
		}

		buf := new(bytes.Buffer)
		buf.ReadFrom(cat)
		s := buf.String()

		cat2, err := sh.Cat(cid2)
		if err != nil {
			fmt.Println(err)
		}

		buf2 := new(bytes.Buffer)
		buf2.ReadFrom(cat2)
		s2 := buf2.String()

		fmt.Println(s + "\n" + s2)

		c.JSON(http.StatusOK, gin.H{"owner": s,"partner": s2})
	})
    r.Run()


	// Where your local node is running on localhost:5001
	//sh := shell.NewShell("35.204.142.46:5001")
	//
	//cid, err := sh.Add(strings.NewReader("hello world!"))
	//if err != nil {
	//	fmt.Fprintf(os.Stderr, "error: %s", err)
	//	os.Exit(1)
	//}
	//
	//fmt.Println("added %s", cid)
	//
	//cat, err := sh.Cat(cid)
	//if err != nil {
	//	fmt.Println(err)
	//}
	//
	//buf := new(bytes.Buffer)
	//buf.ReadFrom(cat)
	//s := buf.String()
	//
	//fmt.Println(s)

}
