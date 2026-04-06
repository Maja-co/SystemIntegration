using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaxiPublisher.Db;

namespace TaxiPublisher;

public class OrderAcceptService : IHostedService {
    private readonly IServiceProvider _serviceProvider;
    private IChannel? _channel;

    public OrderAcceptService(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        // Alt RabbitMQ-logik fra Program.cs flyttet hertil
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = await factory.CreateConnectionAsync();
        _channel = await connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync("accept-order", exclusive: false);
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) => {
            var orderId = Encoding.UTF8.GetString(ea.Body.ToArray());

            // Nyt scope fordi IHostedService er Singleton
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersContext>();

            string replyMessage;
            var order = db.Orders.Find(orderId);

            if (order != null) {
                db.Orders.Remove(order);
                db.SaveChanges();
                replyMessage = "accept";

                // Fortæl ALLE subscribers at ordren er fjernet
                var removeBody = Encoding.UTF8.GetBytes($"remove:{orderId}");
                await _channel.BasicPublishAsync(
                    exchange: "orders", // fanout exchange = går til alle.
                    routingKey: string.Empty,
                    body: removeBody
                );
            }
            else {
                replyMessage = "ikke tilgængelig";
            }

            var replyProperties = new BasicProperties {
                CorrelationId = ea.BasicProperties.CorrelationId
            };
            var body = Encoding.UTF8.GetBytes(replyMessage);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: ea.BasicProperties.ReplyTo,
                mandatory: true,
                basicProperties: replyProperties,
                body: body
            );
        };

        await _channel.BasicConsumeAsync("accept-order", autoAck: true, consumer: consumer);
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _channel?.CloseAsync(); // Sluk pænt
        return Task.CompletedTask;
    }
}