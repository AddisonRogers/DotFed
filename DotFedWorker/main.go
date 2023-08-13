package main

import (
	"database/sql"
	"fmt"
	_ "github.com/lib/pq"
	amqp "github.com/rabbitmq/amqp091-go"
	"github.com/tidwall/gjson"
	"log"
	"os"
	"time"
	"encoding/json"
	"fmt"
	"github.com/gocolly/colly"
	"github.com/tidwall/gjson"
	"io"
	"log"
	"os"
)

func failOnError(err error, msg string) {
	if err != nil {
		log.Panicf("%s: %s", msg, err)
	}
}

func main() {

	//db := Database()
	msgs := RabbitMq()

	go func() {
		for d := range msgs {
			log.Printf("Received a message: %s", d.Body)

			// Update(d.Body) // TODO Validation and stuff

			err := d.Ack(false)
			failOnError(err, "Failed to acknowledge a message")
		}
	}()
	for {
		log.Printf("h")
		time.Sleep(1 * time.Second)
		jsonData, err := os.ReadFile("output.json")
		failOnError(err, "Failed to read file")

		json := string(jsonData)
		results := gjson.Get(json, "#.title").Array() // TODO get the name
		gjson.Get(json, "#.communities").ForEach(func(i, communities gjson.Result) bool {
			println(results[i.Int()].String())
			time.Sleep(1 * time.Second) //TODO remove when ready
			if communities.Raw != "null" {
				time.Sleep(1 * time.Second) //TODO remove when ready
				communities.ForEach(func(_, community gjson.Result) bool {
					fmt.Printf("Name: %s, Link: %s\n", community.Get("name").String(), community.Get("link").String())
					// Update() // TODO put the correct thing here
					return true
				})
			}
			return true
		})
	}
}

func Database() *sql.DB {
	connStr := "user=pqgotest dbname=pqgotest sslmode=verify-full"
	db, err := sql.Open("postgres", connStr)
	failOnError(err, "Failed to open database")
	return db
}

func RabbitMq() <-chan amqp.Delivery {
	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/") // TODO Change this to the docker container
	failOnError(err, "Failed to connect to RabbitMQ")
	defer func(conn *amqp.Connection) {
		err := conn.Close()
		failOnError(err, "Failed to close connection")
	}(conn) // Close when done

	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer func(ch *amqp.Channel) {
		err := ch.Close()
		failOnError(err, "Failed to close channel")
	}(ch) // Close when done

	q, err := ch.QueueDeclare(
		"Update", // name
		false,    // durable
		false,    // delete when unused
		false,    // exclusive
		false,    // no-wait
		nil,      // arguments
	)
	failOnError(err, "Failed to declare a queue")

	msgs, err := ch.Consume(
		q.Name, // queue
		"",     // consumer
		false,  // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	failOnError(err, "Failed to register a consumer")
	return msgs
}

func Update() {

}

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

func CommunityGet() {
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

	os.Remove("output.json")
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
