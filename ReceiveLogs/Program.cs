using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Opret forbindelse til RabbitMQ server
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();

// Opret channel til at modtage beskeder
using var channel = await connection.CreateChannelAsync();

// Opret fanout exchange kaldet "logs"
await channel.ExchangeDeclareAsync(
    exchange: "logs",
    type: ExchangeType.Fanout);

// Opret en midlertidig kø
QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
string queueName = queueDeclareResult.QueueName;

// Bind køen til exchange
await channel.QueueBindAsync(
    queue: queueName,
    exchange: "logs",
    routingKey: string.Empty);

// Opret consumer som lytter efter beskeder
var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += (model, ea) =>
{
    // Konverter besked fra bytes til tekst
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    // Print beskeden i konsollen
    Console.WriteLine($" [x] {message}");

    return Task.CompletedTask;
};

// Start med at lytte på køen
await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

// Holder programmet kørende
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();