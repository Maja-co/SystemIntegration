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
        // Opretter køen
        await _channel.QueueDeclareAsync("accept-order", exclusive: false);
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) => {
            var orderId = Encoding.UTF8.GetString(ea.Body.ToArray());
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersContext>();

            var order = db.Orders.Find(orderId);
            string replyMessage;

            if (order != null) {
                db.Orders.Remove(order);
                db.SaveChanges();
                replyMessage = "accept";
                var deleteMessage = $"REMOVE:{orderId}";
                var deleteBody = Encoding.UTF8.GetBytes(deleteMessage);
                
                // Sender beskeden ud til fanout-exchangen "orders"
                await _channel.BasicPublishAsync("orders", string.Empty, deleteBody);
            }
            else {
                replyMessage = "ikke tilgængelig";
            }

            // Send svar tilbage til den chauffør der prøvede at tage ordren
            if (!string.IsNullOrEmpty(ea.BasicProperties.ReplyTo)) {
                var replyProperties = new BasicProperties {
                    CorrelationId = ea.BasicProperties.CorrelationId
                };
                var body = Encoding.UTF8.GetBytes(replyMessage);

                // Sender svar direkte tilbage til chaufførens egen kø
                await _channel.BasicPublishAsync(string.Empty, ea.BasicProperties.ReplyTo, true, replyProperties, body);
            }
        };

        await _channel.BasicConsumeAsync("accept-order", autoAck: true, consumer: consumer);
        
        // Holder servicen kørende indtil programmet lukkes
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}