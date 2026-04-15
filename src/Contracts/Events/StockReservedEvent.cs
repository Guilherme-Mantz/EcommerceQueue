namespace Contracts.Events;

public record StockReservedEvent
{
    public Guid OrderId { get; init; }
    public bool Success { get; init; }
    public List<Guid> ReservedItems { get; init; } = new();
    public DateTime ReservedAt { get; init; }
}
