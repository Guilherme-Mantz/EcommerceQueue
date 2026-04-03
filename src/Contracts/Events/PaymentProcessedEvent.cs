
namespace Events
{
    public record PaymentProcessedEvent
    {
        public Guid OrderId { get; init; }
        public Guid PaymentId { get; init; }
        public bool Success { get; init; }
        public string? FailureReason { get; init; }
        public DateTime ProcessedAt { get; init; }
    }
}
