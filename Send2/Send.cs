using System.Text;
using RabbitMQ.Client;

// 1. Opret forbindelse til RabbitMQ serveren (kører i Docker på localhost)
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// 2. Deklarer en kø, vi kalder "hello"
// Dette sikrer, at køen eksisterer, før vi sender til den.
await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

// 3. Forbered beskeden

Person person = new Person("Marie", 26, "Marie@gmail.com");
string json = System.Text.Json.JsonSerializer.Serialize(person);
var body = Encoding.UTF8.GetBytes(json);

// 4. Send2 beskeden til køen
await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);

Console.WriteLine($" [x] Sent {json}");

// Vent på brugerinput før lukning, så du kan nå at se beskeden
Console.WriteLine(" Tryk på Enter for at lukke...");
Console.ReadLine();