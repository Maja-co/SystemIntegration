using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using TaxiPublisher.Db;

namespace TaxiPublisher {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHostedService<OrderAcceptanceService>();
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<OrdersContext>(op => op.UseInMemoryDatabase("OrdersDb"));

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("orders", ExchangeType.Fanout);
            builder.Services.AddSingleton<IChannel>(channel);
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