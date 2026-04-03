using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text.Json;
// Opret forbindelse til RabbitMQ server
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();

// Opret channel til at modtage beskeder
using var channel = await connection.CreateChannelAsync();

// Opret fanout exchange kaldet "taxi"
await channel.ExchangeDeclareAsync(
    exchange: "taxi",
    type: ExchangeType.Fanout);

// Opret en midlertidig kø
QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
string queueName = queueDeclareResult.QueueName;

// Bind køen til exchange
await channel.QueueBindAsync(
    queue: queueName,
    exchange: "taxi",
    routingKey: string.Empty);

// Opret consumer som lytter efter beskeder
var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) => {

    // Hent beskeden som bytes
    byte[] body = ea.Body.ToArray();

    // Konverter JSON fra bytes til string
    var json = Encoding.UTF8.GetString(body);

    // Konverter JSON til TaxiOrder objekt
    var order = JsonSerializer.Deserialize<TaxiOrder>(json);

    // Print oplysninger om bestillingen
    Console.WriteLine($"Ny bestilling!");
    Console.WriteLine($"Navn: {order.Name}");
    Console.WriteLine($"Telefon: {order.PhoneNumber}");
    Console.WriteLine($"Afhentning: {order.PickupAddress}");
    Console.WriteLine($"Destination: {order.DestinationAddress}");

    return Task.CompletedTask;
};

// Start med at lytte på køen
await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

// Holder programmet kørende
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();