using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Shared;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Opretter forbindelse til RabbitMQ
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    exchange: "taxi",
    type: ExchangeType.Fanout);


// Web API endpoint
app.MapPost("/order", async (TaxiOrder order) =>
{
    var json = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(json);

    await channel.BasicPublishAsync(
        exchange: "taxi",
        routingKey: string.Empty,
        body: body);

    Console.WriteLine($"Bestilling sendt for: {order.Name}");
    return Results.Ok("Bestilling modtaget!");
});

app.Run();