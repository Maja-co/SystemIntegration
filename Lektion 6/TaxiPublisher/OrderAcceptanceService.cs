using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaxiPublisher.Db;

namespace TaxiPublisher;

public class OrderAcceptanceService : BackgroundService {
    private readonly IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    public OrderAcceptanceService(IChannel channel, IServiceProvider serviceProvider) {
        _channel = channel;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await _channel.QueueDeclareAsync("accept-order", exclusive: false);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) => {
            Console.WriteLine($"Received Request: {ea.BasicProperties.CorrelationId}");
            string replyMessage;

            var orderId = Encoding.UTF8.GetString(ea.Body.ToArray());
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersContext>();

            var order = db.Orders.Find(orderId);
            if (order != null) {
                db.Orders.Remove(order);
                db.SaveChanges();
                replyMessage = "accept";
            }
            else {
                replyMessage = "ikke tilgængelig";
            }

            var replyProperties = new BasicProperties {
                CorrelationId = ea.BasicProperties.CorrelationId
            };
            var body = Encoding.UTF8.GetBytes(replyMessage);
            if (string.IsNullOrEmpty(ea.BasicProperties.ReplyTo)) {
                Console.WriteLine("No reply-to property set. Cannot send reply.");
                return;
            }

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: ea.BasicProperties.ReplyTo,
                mandatory: true,
                basicProperties: replyProperties,
                body: body
            );
        };
        await _channel.BasicConsumeAsync("accept-order", autoAck: true, consumer: consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}