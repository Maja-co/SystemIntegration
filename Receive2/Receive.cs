using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// 1. Opret forbindelse (samme som i senderen)
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// 2. Deklarer køen (skal matche senderen præcis!)
await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

Console.WriteLine(" [*] Waiting for messages.");

// 3. Opret en forbruger (consumer) der lytter
var consumer = new AsyncEventingBasicConsumer(channel);

// Denne kode kører, hver gang en besked modtages
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Person? modtagetPerson = JsonSerializer.Deserialize<Person>(message);
    Console.WriteLine($" [x] Navn: {modtagetPerson.Name}, Alder: {modtagetPerson.Age}");
    return Task.CompletedTask;
};

// 4. Start lytningen
await channel.BasicConsumeAsync(queue: "hello", autoAck: true, consumer: consumer);

Console.WriteLine(" Tryk på Enter for at lukke...");
Console.ReadLine();