using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TaxiSubscriber.Model;

namespace TaxiSubscriber {
    internal class Program {
        private static readonly List<Order> _orders = [];
        private static string? _pendingOrderId = null;

        static async Task Main(string[] args) {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "orders", type: ExchangeType.Fanout);
            QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
            string queueName = queueDeclareResult.QueueName;
            await channel.QueueBindAsync(queue: queueName, exchange: "orders", routingKey: string.Empty);
            var replyQueue = await channel.QueueDeclareAsync();
            Console.WriteLine("Venter på order.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) => {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                // Tjek om det er en "fjern ordre" besked
                if (message.StartsWith("remove:"))
                {
                    var removedId = message.Replace("remove:", "");
                    _orders.RemoveAll(order => order.Id == removedId);
                    Console.WriteLine($"Ordre {removedId} er accepteret af en anden chauffør");
                    PrintOrders();
                    return Task.CompletedTask;
                }

                // Eksisterende logik — normal ny ordre
                Order? order = JsonSerializer.Deserialize<Order>(message);
                if (order is not null)
                {
                    _orders.Add(order);
                    PrintOrders();
                }

                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

            var replyConsumer = new AsyncEventingBasicConsumer(channel);
            replyConsumer.ReceivedAsync += (model, ea) => {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Reply Recieved: {message}");
                if (message == "accept") {
                    _orders.RemoveAll(order => order.Id == _pendingOrderId); // ← brug det
                    PrintOrders();
                } else {
                    Console.WriteLine($"Fejl: {message}.");
                }
                return Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(replyQueue.QueueName, autoAck: true, consumer: replyConsumer);
            
            
            var resultTask = GetUserInput(channel, replyQueue.QueueName);
            while (true) {
                if (resultTask.IsCompleted) {
                    resultTask = GetUserInput(channel, replyQueue.QueueName);
                }

                Thread.Sleep(100);
            }
        }

        private static void PrintOrders() {
            Console.WriteLine("Id  | Destination | Afhentnings tidspunkt");
            Console.WriteLine("-----------------------------------------");
            _orders.ForEach(order => {
                string? pickUpTime = order.QuickOrder ? "Snarest muligt" : order.PickUpTime.ToString();
                Console.WriteLine($"{order.Id} | {order.Destination} | {pickUpTime}");
            });
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Vælg en order ved at indtaste id'et");
        }

        private static Task GetUserInput(IChannel channel, string replyQueueName) {
            return Task.Run(async () => {
                string? input = Console.ReadLine();
                if (input is not null) {
                    _pendingOrderId = input;
                    Console.WriteLine($"Bruger input {input}");
                    var properties = new BasicProperties() {
                        CorrelationId = Guid.NewGuid().ToString(),
                        ReplyTo = replyQueueName
                    };
                    
                    var body = Encoding.UTF8.GetBytes(input);

                    await channel.BasicPublishAsync(
                        exchange: string.Empty,
                        routingKey: "accept-order",
                        mandatory: true,
                        basicProperties: properties,
                        body: body
                    );

                    //_orders.RemoveAll(order => order.Id == input); // fjerner lokalt med det samme
                    PrintOrders();
                }
            });
        }
    }
}