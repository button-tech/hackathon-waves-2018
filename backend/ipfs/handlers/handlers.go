package handlers

import (
	"github.com/gin-gonic/gin"
	"strings"
	"fmt"
	"os"
	"github.com/ipfs/go-ipfs-api"
	"net/http"
	"bytes"
)

func SetToIpfs(c *gin.Context){

	type Data struct {
		Document_owner string `json:"document_owner"`
		Document_partner string `json:"document_partner"`
	}

	var data Data

	c.BindJSON(&data)

	cidOwner, err := Set(data.Document_owner)
	if err != nil {
		fmt.Fprintf(os.Stderr, "error: %s", err)
		os.Exit(1)
	}

	cidPartner, err := Set(data.Document_partner)
	if err != nil {
		fmt.Fprintf(os.Stderr, "error: %s", err)
		os.Exit(1)
	}

	fmt.Println(data)

	c.JSON(http.StatusOK, gin.H{"owner": cidOwner,"partner":cidPartner})
}

func GetFromIpfs(c *gin.Context){

	hash := c.Param("hash")
	
	sh := shell.NewShell("35.204.142.46:5001")

	cat, err := sh.Cat(hash)
	if err != nil {
		fmt.Println(err)
	}

	buf := new(bytes.Buffer)
	buf.ReadFrom(cat)
	resp := buf.String()

	c.JSON(http.StatusOK, gin.H{"data":resp})

}

//

func Set(data string)(string,error){

	sh := shell.NewShell("35.204.142.46:5001")

	cid, err := sh.Add(strings.NewReader(data))
	if err != nil {
		fmt.Fprintf(os.Stderr, "error: %s", err)
		return "",err
	}

	return cid, nil
}

