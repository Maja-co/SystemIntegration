using RabbitMQ.Client;
using System.Text;

/*
  EmitLog → "logs" exchange → (ingen køer bundet endnu)
                                           ↓
                                    Besked forsvandt!
 */
// Forsinkelse så reciver er klar (1 sekund)
await Task.Delay(1000); 

// Opret forbindelse til RabbitMQ server
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();

// Opret en channel til at sende beskeder
using var channel = await connection.CreateChannelAsync();

// Opret en fanout exchange kaldet "logs"
await channel.ExchangeDeclareAsync(
    exchange: "logs",
    type: ExchangeType.Fanout);

// Hent beskeden fra arguments eller brug standard tekst
var message = GetMessage(args);

// Konverter beskeden til bytes
var body = Encoding.UTF8.GetBytes(message);

// Send beskeden til exchange
await channel.BasicPublishAsync(
    exchange: "logs",
    routingKey: string.Empty,
    body: body);

// Vis hvad der blev sendt
Console.WriteLine($" [x] Sent {message}");
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static string GetMessage(string[] args) {
    // Returner arguments som tekst eller default besked
    return args.Length > 0 ? string.Join(" ", args) : "info: Hello World!";
}