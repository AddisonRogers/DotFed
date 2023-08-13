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
		fmt.Println(gjson.Get(json, "1.name")) // TODO get the name

		gjson.Get(json, "#.communities").ForEach(func(i, communities gjson.Result) bool {
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
