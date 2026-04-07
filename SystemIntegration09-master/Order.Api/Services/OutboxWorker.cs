using System.Text.Json;
using Orders.Api.data;
using Orders.Api.Models;

namespace Orders.Api.Services;

public class OutboxWorker : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxWorker> _logger;

    public OutboxWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxWorker> logger) {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            using (var scope = _scopeFactory.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<OrdersContext>();
                var sender = scope.ServiceProvider.GetRequiredService<IShippingMessageSender>();

                // 1. Find ubehandlede beskeder i databasen
                var message = context.OutboxMessages.Where(outMessage => outMessage.ProcessedAtUTC == null)
                    .FirstOrDefault();
                if (message != null) {
                    try {
                        // Serialisering (objekt → JSON)
                        var order = JsonSerializer.Deserialize<Order>(message.Payload);
                        if (order != null) {
                            // 2. Send dem via 'sender'
                            await sender.SendMessageAsync(order);

                            // 3. Marker dem som færdige i 'context'
                            message.ProcessedAtUTC = DateTime.UtcNow;
                            await context.SaveChangesAsync();

                            _logger.LogInformation("Outbox worker sendte besked for ordre: {Id}", order.Id);
                        }
                    }
                    catch (Exception e) {
                        _logger.LogError(e, "Fejl ved behandling af outbox besked {Id}", message.Id);
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}