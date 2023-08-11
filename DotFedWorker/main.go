package main

import (
	_ "github.com/lib/pq"
	amqp "github.com/rabbitmq/amqp091-go"
	"log"
	"time"
)

func failOnError(err error, msg string) {
	if err != nil {
		log.Panicf("%s: %s", msg, err)
	}
}

func main() { /*
		connStr := "user=pqgotest dbname=pqgotest sslmode=verify-full"
		db, err := sql.Open("postgres", connStr)
		if err != nil {
			log.Fatal(err)
		}
	*/// Connects to the DB

	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/") // TODO Change this to the docker container
	failOnError(err, "Failed to connect to RabbitMQ")
	defer func(conn *amqp.Connection) {
		err := conn.Close()
		if err != nil {
			log.Panicf("Failed to close connection: %s", err)
		}
	}(conn) // Close when done

	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer func(ch *amqp.Channel) {
		err := ch.Close()
		if err != nil {
			log.Panicf("Failed to close channel: %s", err)
		}
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

	go func() {
		for d := range msgs {
			log.Printf("Received a message: %s", d.Body)
			d.Ack(false)
		}
	}()
	for {
		log.Printf("h")
		time.Sleep(1 * time.Second)
		// Goes through the DB and updates the values
	}
}
func update() {
}
