using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Stock.Infrastructure.Messaging;

public class PaymentProcessedConsumer(ILogger<PaymentProcessedConsumer> logger) : IConsumer<PaymentProcessedEvent>
{
    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var paymentEvent = context.Message;

        if (!paymentEvent.Success)
        {
            logger.LogWarning(
                "Payment failed for Order {OrderId}. Reason: {Reason}. Skipping stock reservation.",
                paymentEvent.OrderId, paymentEvent.FailureReason);
            return;
        }

        logger.LogInformation(
            "Payment confirmed for Order {OrderId}. Starting stock reservation.",
            paymentEvent.OrderId);

        await ReserveStockAsync(paymentEvent.OrderId);

        var stockEvent = new StockReservedEvent
        {
            OrderId = paymentEvent.OrderId,
            Success = true,
            ReservedItems = new List<Guid>(),
            ReservedAt = DateTime.UtcNow
        };

        await context.Publish(stockEvent);

        logger.LogInformation(
            "Stock reserved and StockReservedEvent published for Order {OrderId}",
            paymentEvent.OrderId);
    }

    private async Task ReserveStockAsync(Guid orderId)
    {
        logger.LogInformation("Reserving stock for Order {OrderId}", orderId);
        await Task.Delay(500);
        logger.LogInformation("Stock successfully reserved for Order {OrderId}", orderId);
    }
}
