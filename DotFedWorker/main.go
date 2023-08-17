package main

import (
	"context"
	"encoding/json"
	"fmt"
	"github.com/gocolly/colly"
	amqp "github.com/rabbitmq/amqp091-go"
	"github.com/tidwall/gjson"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"io"
	"log"
	"net/http"
	"os"
	"time"
)

func failOnError(err error, msg string) {
	if err != nil {
		log.Panicf("%s: %s", msg, err)
	}
}

func main() {
	db := Database()
	msgs := RabbitMq()

	go func() {
		for d := range msgs {
			log.Printf("Received a message: %s", d.Body)

			// NewUpdate(d.Body) // TODO Validation and stuff

			err := d.Ack(false)
			failOnError(err, "Failed to acknowledge a message")
		}
	}()
	for {
		log.Printf("h")
		time.Sleep(1 * time.Second)
		jsonData, err := os.ReadFile("output.json")
		if err != nil {
			CommunityGet()
			jsonData, err = os.ReadFile("output.json")
			failOnError(err, "Failed to read file")
		}

		json := string(jsonData)
		results := gjson.Get(json, "#.title").Array() // TODO get the name
		gjson.Get(json, "#.communities").ForEach(func(i, communities gjson.Result) bool {
			println(results[i.Int()].String())
			time.Sleep(1 * time.Second) //TODO remove when ready
			if communities.Raw != "null" {
				time.Sleep(1 * time.Second) //TODO remove when ready
				communities.ForEach(func(_, community gjson.Result) bool {
					fmt.Printf("Name: %s, Link: %s\n", community.Get("name").String(), community.Get("link").String())
					// NewUpdate() // TODO put the correct thing here
					return true
				})
			}
			return true
		})
	}
}

func Database() *mongo.Client {
	const uri = "mongodb://localhost:27017"
	// Use the SetServerAPIOptions() method to set the Stable API version to 1
	serverAPI := options.ServerAPI(options.ServerAPIVersion1)
	opts := options.Client().ApplyURI(uri).SetServerAPIOptions(serverAPI)
	// Create a new client and connect to the server
	client, err := mongo.Connect(context.TODO(), opts)
	if err != nil {
		panic(err)
	}
	defer func() {
		if err = client.Disconnect(context.TODO()); err != nil {
			panic(err)
		}
	}()
	var result bson.M
	if err := client.Database("admin").RunCommand(context.TODO(), bson.D{{"ping", 1}}).Decode(&result); err != nil {
		panic(err)
	}
	fmt.Println("Pinged your deployment. You successfully connected to MongoDB!")
	return client
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
		"NewUpdate", // name
		false,       // durable
		false,       // delete when unused
		false,       // exclusive
		false,       // no-wait
		nil,         // arguments
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

type CommunityUpdate struct {
	Collection     string
	CollectionLink string
	CommunityName  string
	CommunityLink  string
	Platform       string
	OpenSignups    bool
}

func Update(db *mongo.Client, community CommunityUpdate) { // This is used to populate the database

	resp, err := http.Get(fmt.Sprintf("https://%s/api/v3/post/list?community_name=%d&limit=50&page=1", community.CommunityLink, community.CommunityName))
	failOnError(err, "Failed to get the posts")
	defer func(Body io.ReadCloser) {
		err := Body.Close()
		failOnError(err, "Failed to close the body")
	}(resp.Body)

	body, err := io.ReadAll(resp.Body)
	failOnError(err, "Failed to read the body")

	bodyStr := string(body)
	_posts := gjson.Get(bodyStr, "posts.#.name").Array()

	var posts []Post
	// Something something for item in the gjson array append to the posts array

	var newCommunities []NCommunity
	var newCommunity = NCommunity{
		Name:  community.CommunityName,
		Link:  community.CommunityLink,
		Posts: posts,
	}

	newCommunities = append(newCommunities, newCommunity)

	newNode := Node{
		Name:        community.Collection,
		Link:        community.CollectionLink,
		Platform:    community.Platform,
		OpenSignups: community.OpenSignups,
		Communities: newCommunities,
	}

	coll := db.Database("nodes").Collection(community.Collection)
	filter := bson.D{{"Name", community.CommunityName}}
	opts := options.Update().SetUpsert(true)
	_, err = coll.UpdateOne(context.TODO(), filter, newNode, opts)
	failOnError(err, "Failed to update the document")
}

type Node struct {
	Name        string       `bson:"name"`
	Link        string       `bson:"link"`
	Platform    string       `bson:"platform"`
	OpenSignups bool         `bson:"open_signups"`
	Communities []NCommunity `bson:"communities, omitEmpty"`
}

type NCommunity struct {
	Name  string `bson:"name"`
	Link  string `bson:"link"`
	Posts []Post `bson:"posts, omitEmpty"`
}

type Post struct {
	Title     string    `bson:"title"`
	Link      string    `bson:"link"`
	Author    string    `bson:"author"`
	Published string    `bson:"published"`
	Content   string    `bson:"content"`
	UpVotes   int       `bson:"up_votes"`
	DownVotes int       `bson:"down_votes"`
	Comments  []Comment `bson:"comments, omitEmpty"`
}

type Comment struct {
	Author    string `bson:"author"`
	Published string `bson:"published"`
	Content   string `bson:"content"`
	UpVotes   int    `bson:"up_votes"`
	DownVotes int    `bson:"down_votes"`
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

// CommunityGet This loops through the JSON doc and then calls the get function to get the communities from the website
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
				})
			}

			fmt.Println(l, ":", name.String())
			for k, community := range Communities {
				fmt.Println(k, ":", community)
			}
		}(name)
	}

	print(len(Threads))
	println("All done")

	err = os.Remove("output.json")
	failOnError(err, "Failed to remove file")
	newFile, err := os.Create("output.json")
	failOnError(err, "Failed to create file")
	defer func(newFile *os.File) {
		err := newFile.Close()
		failOnError(err, "Failed to close file")
	}(newFile)

	encoder := json.NewEncoder(newFile)
	encoder.SetIndent("", "  ")
	if err := encoder.Encode(&Threads); err != nil {
		fmt.Printf("Failed to encode data to JSON: %v\n", err)
	}
}

// This goes to the URL and gets the communities from the website and then appends it to the community list
func get(url string, communities []Community) []Community {
	c := colly.NewCollector()

	var i int
	c.OnHTML("tbody tr", func(e *colly.HTMLElement) {
		i++ // count the number of elements in the thing
		var h = e.DOM.Find("td a")
		w, _ := e.DOM.Find("td a").Attr("href")
		communities = append(communities, Community{
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
		return communities
	}
	return communities
}
