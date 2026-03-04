using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using AirportAPI;


// 1. Opsæt forbindelse
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// 2. Hvilken kø skal vi lytte på?
string queueName = args.Length > 0 ? args[0].ToUpper() : "SAS";

await channel.QueueDeclareAsync(queue: queueName,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine($"[*] Venter på beskeder fra {queueName}. Tryk på [enter] for at stoppe.");

// 3. Selve "lytteren"
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) => {
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var mitObjekt = JsonSerializer.Deserialize<GateInfo>(message);
    Console.WriteLine($"Gate nummeret er:  {mitObjekt.GateNumber}, for fly: {mitObjekt.FlightNumber} ");
    await Task.CompletedTask;
};

// Start lytningen asynkront
await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

Console.ReadLine();