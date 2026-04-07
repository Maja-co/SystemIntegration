namespace Orders.Api.Models;

public class OutboxMessage {
    public int Id { get; set; }
    public string Type { get; set; }
    public string Payload { get; set; }
    public DateTime ? ProcessedAtUTC { get; set; } 
}