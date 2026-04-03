using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scalar.AspNetCore;
using TaxiPublisher.Db;

namespace TaxiPublisher {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<OrdersContext>(op => op.UseInMemoryDatabase("OrdersDb"));

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("orders", ExchangeType.Fanout);
            builder.Services.AddSingleton<IChannel>(channel);
            var app = builder.Build();

            await channel.QueueDeclareAsync("accept-order", exclusive: false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) => {
                Console.WriteLine($"Received Request: {ea.BasicProperties.CorrelationId}");
                string replyMessage;

                var orderId = Encoding.UTF8.GetString(ea.Body.ToArray());
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrdersContext>();

                var order = db.Orders.Find(orderId);
                if (order != null) {
                    db.Orders.Remove(order);
                    db.SaveChanges();
                    replyMessage = "accept";  // ← simpelt
                } else {
                    replyMessage = "ikke tilgængelig";
                }

                var replyProperties = new BasicProperties {
                    CorrelationId = ea.BasicProperties.CorrelationId // ← kopiér ID
                };
                var body = Encoding.UTF8.GetBytes(replyMessage);
                if (string.IsNullOrEmpty(ea.BasicProperties.ReplyTo)) {
                    Console.WriteLine("No reply-to property set. Cannot send reply.");
                    return;
                }

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: ea.BasicProperties.ReplyTo, // ← send til klientens kø
                    mandatory: true,
                    basicProperties: replyProperties,
                    body: body
                );
            };
            await channel.BasicConsumeAsync("accept-order", autoAck: true, consumer: consumer);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}