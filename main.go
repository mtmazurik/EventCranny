package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"

	"github.com/gorilla/mux"
	"github.com/streadway/amqp"
)

// simple event cranny service - wraps cloudAMQP.com hosted RabbitMQ
func main() {
	router := mux.NewRouter()
	router.HandleFunc("/person", CreatePerson).Methods("POST")
	log.Fatal(http.ListenAndServe(":8901", router))
}

// Person : is for person
type Person struct {
	ID        string `json:"id,omitempty"`
	Firstname string `json:"firstname,omitempty"`
	Lastname  string `json:"lastname,omitempty"`
	City      string `json:"city,omitempty"`
	State     string `json:"state,omitempty"`
}

func CreatePerson(w http.ResponseWriter, r *http.Request) {
	//params := mux.Vars(r)
	var person Person
	_ = json.NewDecoder(r.Body).Decode(&person)
	json.NewEncoder(w).Encode(person)

	//url := os.Getenv("CLOUDAMQP_URL")
	url := "amqp://iipsuwdh:d6SvD6ME8MyYbSbDwCeFDOEEb7rSLEgK@otter.rmq.cloudamqp.com/iipsuwdh"
	conn, err := amqp.Dial(url)
	if err != nil {
		fmt.Println("err: ", err)
		//		return fmt.Errorf("Dial: %s", err)
	}
	defer conn.Close()

	ch, _ := conn.Channel()
	defer ch.Close()

	queue, err := ch.QueueDeclare(
		"personQueue",
		false, //persist
		false, //autodelete,
		false, //exclusive
		false, //no-wait
		nil,   // adv args
	)

	var body []byte
	body, err = json.Marshal(person) // convert to []byte array
	if err != nil {
		fmt.Println("err:", err)
	}
	err = ch.Publish(
		"",         //exchange
		queue.Name, //routing key
		false,      //mandatory
		false,      //immediate
		amqp.Publishing{
			ContentType: "text/plain", // MIME content type https://www.freeformatter.com/mime-types-list.html
			Body:        body,
		})

	// goroutine asynch?
	// go func(con *amqp.Connection) {
	// 	timer := time.NewTicker(1 * time.Second)
	// 	channel, _ := connection.Channel()

	// 	for t := range timer.C {
	// 		msg := amqp.Publishing{
	// 			DeliveryMode: 1,
	// 			Timestamp:    t,
	// 			ContentType:  "text/plain",
	// 			Body:         []byte("Hello world"),
	// 		}
	// 		mandatory, immediate := false, false
	// 		channel.Publish("amq.topic", "ping", mandatory, immediate, msg)
	// 	}
	//}(connection)

	// select {}
}
