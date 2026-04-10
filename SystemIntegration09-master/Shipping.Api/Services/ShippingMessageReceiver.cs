using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shipping.Api.Models;

namespace Shipping.Api.Services {
    public class ShippingMessageReceiver : IHostedService {
        private readonly ILogger<ShippingMessageReceiver> _logger;
        private readonly IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;

        public ShippingMessageReceiver(IConnection connection, ILogger<ShippingMessageReceiver> logger,
            IServiceScopeFactory scopeFactory) {
            _connection = connection;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            using var channel = await _connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: "shipping_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) => {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                var order = System.Text.Json.JsonSerializer.Deserialize<Order>(message);

                _logger.LogInformation("Received message: {Message}", message);

                using var scope = _scopeFactory.CreateScope();
                var shippingContext = scope.ServiceProvider.GetRequiredService<Data.ShippingContext>();

                // Er ordren null - ja afslut
                if (order == null) return;

                // Tjekker databasen for eksisterende OrderId
                var existing = await shippingContext.ShippingOrders
                    .FirstOrDefaultAsync(s => s.OrderId == order.Id);

                // Hvis den findes, logges den og afbryder funktionen tidligt (Idempotent Receiver)
                if (existing != null) {
                    _logger.LogWarning("Advarsel: Ordre {OrderId} er allerede behandlet. Skipper dublet.", order.Id);
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }

                // Ordren er ny = nyt objekt
                var shippingOrder = new ShippingOrder {
                    ShippingId = Guid.NewGuid(),
                    OrderId = order.Id,
                    ShippingAdress = order.ShippingAddress ?? string.Empty,
                    Status = ShippingStatus.Pending
                };

                // Gem i databasen
                shippingContext.ShippingOrders.Add(shippingOrder);
                await shippingContext.SaveChangesAsync();
                await channel.BasicAckAsync(ea.DeliveryTag, false);
                
                _logger.LogInformation("Creating shipping order for OrderId: {OrderId}", shippingOrder.OrderId);
                await Task.CompletedTask;
            };
            
            await channel.BasicConsumeAsync(queue: "shipping_queue",
                autoAck: false, // False = underskrift. True = bare lever
                consumer: consumer);
        }
        
        public Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Stopping ShippingMessageReceiver...");
            return Task.CompletedTask;
        }
    }
}