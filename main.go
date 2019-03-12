package main

import (
	"encoding/json"
	"log"
	"net/http"
	"time"

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
	ID        string   `json:"id,omitempty"`
	Firstname string   `json:"firstname,omitempty"`
	Lastname  string   `json:"lastname,omitempty"`
	Address   *Address `json:"address,omitempty"`
}

// Address : is for addresses
type Address struct {
	City  string `json:"city,omitempty"`
	State string `json:"state,omitempty"`
}

// Create person, publish to topic
func CreatePerson(w http.ResponseWriter, r *http.Request) {
	//params := mux.Vars(r)
	var person Person
	_ = json.NewDecoder(r.Body).Decode(&person)
	json.NewEncoder(w).Encode(person)

	//url := os.Getenv("CLOUDAMQP_URL")
	url := "amqp://iipsuwdh:d6SvD6ME8MyYbSbDwCeFDOEEb7rSLEgK@otter.rmq.cloudamqp.com/iipsuwdh"
	connection, _ := amqp.Dial(url)
	//	if err != nil {
	//		return fmt.Errorf("Dial: %s", err)
	//	}

	go func(con *amqp.Connection) {
		timer := time.NewTicker(1 * time.Second)
		channel, _ := connection.Channel()

		for t := range timer.C {
			msg := amqp.Publishing{
				DeliveryMode: 1,
				Timestamp:    t,
				ContentType:  "text/plain",
				Body:         []byte("Hello world"),
			}
			mandatory, immediate := false, false
			channel.Publish("amq.topic", "ping", mandatory, immediate, msg)
		}
	}(connection)

	select {}
}
