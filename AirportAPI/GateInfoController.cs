using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using AirportAPI; 

namespace AirportAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class GateInfoController : ControllerBase {
    private readonly ILogger<GateInfoController> _logger;

    public GateInfoController(ILogger<GateInfoController> logger) {
        _logger = logger;
    }

    [HttpGet(Name = "gateno")]
    public async Task<bool> get(
        [FromHeader] Airline airline,
        [FromHeader] int gateNumber,
        [FromHeader] string flightNumber) {
        
        _logger.LogInformation($"API modtog: {airline} fly {flightNumber} ved gate {gateNumber}");

        var info = new GateInfo(gateNumber, flightNumber);
        
        // 1. Opret forbindelse asynkront
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // 2. Opret køen asynkront
        await channel.QueueDeclareAsync(queue: airline.ToString(), 
            durable: false, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);

        // 3. Serialiser til JSON
        var message = JsonSerializer.Serialize(info);
        var body = Encoding.UTF8.GetBytes(message);

        // 4. Publicer beskeden asynkront
        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: airline.ToString(),
            body: body);

        _logger.LogInformation($"GateInfo objekt sendt til køen: {airline}");
        return true;
    }
}