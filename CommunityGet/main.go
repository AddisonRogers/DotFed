package main

import (
	"encoding/json"
	"fmt"
	"github.com/gocolly/colly"
	"github.com/tidwall/gjson"
	"io"
	"log"
	"os"
	"sync"
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

	var wg sync.WaitGroup
	threadCh := make(chan Thread, len(result.Array())) // Buffered channel

	for _, name := range result.Array() {
		wg.Add(1) // Increment the WaitGroup counter

		go func(name gjson.Result) {
			defer wg.Done() // Decrement counter when goroutine completes

			var Communities []Community
			Communities = get(name.Get("name").String(), Communities)

			if name.Get("thefederation_platform.name").String() == "lemmy" {
				threadCh <- Thread{
					Title:       name.Get("name").String(),
					Platform:    name.Get("thefederation_platform.name").String(),
					OpenSignups: name.Get("open_signups").Bool(),
					Communities: Communities,
				} // Send to channel
			}

			fmt.Println(name.String())

		}(name)
	}

	// Close channel after all goroutines finish
	go func() {
		wg.Wait()
		close(threadCh)
	}()

	// Append values from channel to Threads slice
	for thread := range threadCh {
		Threads = append(Threads, thread)
	}

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

	c.OnHTML("tbody tr", func(e *colly.HTMLElement) {
		var h = e.DOM.Find("td a")
		w, _ := e.DOM.Find("td a").Attr("href")
		fmt.Println(h.Text(), w)
		Communities = append(Communities, Community{
			Name: h.Text(),
			Link: w,
		})

	})

	for i := 1; i < 100; i++ { // I can speed this up by getting the actual count of pages
		_ = c.Visit(fmt.Sprintf("https://%s/communities?listingType=Local&page=%d", url, i))
	}

	return Communities
}
