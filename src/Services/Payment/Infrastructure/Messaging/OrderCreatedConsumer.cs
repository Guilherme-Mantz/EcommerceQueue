using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Payment.Infrastructure.Messaging;

public class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderEvent = context.Message;

        logger.LogInformation(
            "Received OrderCreatedEvent for Order {OrderId}, Customer {CustomerId}, Amount {Amount}",
            orderEvent.OrderId, orderEvent.CustomerId, orderEvent.TotalAmount);

        try
        {
            await ProcessPaymentAsync(orderEvent);

            var paymentEvent = new PaymentProcessedEvent
            {
                OrderId = orderEvent.OrderId,
                PaymentId = Guid.NewGuid(),
                Success = true,
                ProcessedAt = DateTime.UtcNow
            };

            await context.Publish(paymentEvent);

            logger.LogInformation(
                "Payment processed and PaymentProcessedEvent published for Order {OrderId}",
                orderEvent.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to process payment for Order {OrderId}",
                orderEvent.OrderId);

            var failureEvent = new PaymentProcessedEvent
            {
                OrderId = orderEvent.OrderId,
                PaymentId = Guid.Empty,
                Success = false,
                FailureReason = ex.Message,
                ProcessedAt = DateTime.UtcNow
            };

            await context.Publish(failureEvent);

            throw; // re-throw para acionar retry do MassTransit
        }
    }

    private async Task ProcessPaymentAsync(OrderCreatedEvent orderEvent)
    {
        logger.LogInformation(
            "Processing payment for Order {OrderId} with amount {Amount}",
            orderEvent.OrderId, orderEvent.TotalAmount);

        await Task.Delay(1000);

        var random = new Random();
        if (random.NextDouble() < 0.1)
        {
            throw new InvalidOperationException("Payment gateway temporarily unavailable");
        }

        logger.LogInformation(
            "Payment successfully processed for Order {OrderId}",
            orderEvent.OrderId);
    }
}
