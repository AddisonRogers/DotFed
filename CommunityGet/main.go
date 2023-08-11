package main

import (
	"encoding/json"
	"fmt"
	"github.com/gocolly/colly"
	"github.com/tidwall/gjson"
	"io"
	"log"
	"os"
)

type Community struct {
	Name string `json:"name"`
	Link string `json:"link"`
}

type Thread struct {
	Title       string      `json:"title"`
	Platform    string      `json:"platform"`
	OpenSignups bool        `json:"open_signups"`
	Communities []Community `json:"communities"`
}

func main() {
	file, err := os.Open("data.json")
	if err != nil {
		panic(err)
	}
	defer func(file *os.File) {
		err := file.Close()
		if err != nil {
			panic(err)
		}
	}(file)

	// read file's content to byte slice
	bytes, err := io.ReadAll(file)
	if err != nil {
		panic(err)
	}

	// convert byte slice to string
	result := gjson.Get(string(bytes), "data.thefederation_node")

	var Threads []Thread

	//threadCh := make(chan Thread, len(result.Array())) // Buffered channel

	for l, name := range result.Array() {

		func(name gjson.Result) {
			var Communities []Community
			Communities = get(name.Get("name").String(), Communities)

			if name.Get("thefederation_platform.name").String() == "lemmy" {
				Threads = append(Threads, Thread{
					Title:       name.Get("name").String(),
					Platform:    name.Get("thefederation_platform.name").String(),
					OpenSignups: name.Get("open_signups").Bool(),
					Communities: Communities,
				}) // Send to channel)
			}

			fmt.Println(l, ":", name.String()) // There is 1691 elements to go through woot
			for k, community := range Communities {
				fmt.Println(k, ":", community)
			}
		}(name)
	}

	// Close channel after all goroutines finish

	// Append values from channel to Threads slice
	/*for thread := range threadCh {
		Threads = append(Threads, thread)
	}*/

	print(len(Threads))
	println("All done")

	newFile, err := os.Create("output.json")
	if err != nil {
		log.Fatalf("Failed to create file: %v\n", err)
	}
	defer func(newFile *os.File) {
		err := newFile.Close()
		if err != nil {
			log.Fatalf("Failed to close file: %v\n", err)
		}
	}(newFile)

	encoder := json.NewEncoder(newFile)
	encoder.SetIndent("", "  ")
	if err := encoder.Encode(&Threads); err != nil {
		fmt.Printf("Failed to encode data to JSON: %v\n", err)
	}
}

func get(url string, Communities []Community) []Community {
	c := colly.NewCollector()

	var i int
	c.OnHTML("tbody tr", func(e *colly.HTMLElement) {
		i++ // count the number of elements in the thing
		var h = e.DOM.Find("td a")
		w, _ := e.DOM.Find("td a").Attr("href")
		Communities = append(Communities, Community{
			Name: h.Text(),
			Link: w,
		})
	})

	j := 1
	for j < 100 { //
		i = 0
		_ = c.Visit(fmt.Sprintf("https://%s/communities?listingType=Local&page=%d", url, j))
		if i == 0 {
			break
		}
		j++

	}

	if i != 0 {
		return Communities
	}
	return Communities
}
