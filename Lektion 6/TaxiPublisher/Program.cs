using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using TaxiPublisher.Db;

namespace TaxiPublisher {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<OrdersContext>(op => op.UseInMemoryDatabase("OrdersDb"));
            
            // Registrer RabbitMQ channel til publish (som stadig bruges af controller)
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("orders", ExchangeType.Fanout);
            builder.Services.AddSingleton<IChannel>(channel);

            // Registrer vores nye Hosted Service!
            builder.Services.AddHostedService<OrderAcceptService>();

            var app = builder.Build();

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