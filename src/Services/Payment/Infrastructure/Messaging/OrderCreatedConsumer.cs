using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Payment.Infrastructure.Messaging;

/// <summary>
/// Consumer for OrderCreatedEvent to process payments
/// </summary>
public class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderEvent = context.Message;

        _logger.LogInformation(
            "Received OrderCreatedEvent for Order {OrderId}, Customer {CustomerId}, Amount {Amount}",
            orderEvent.OrderId, orderEvent.CustomerId, orderEvent.TotalAmount);

        try
        {
            // Simulate payment processing
            await ProcessPaymentAsync(orderEvent);

            _logger.LogInformation(
                "Payment processing completed for Order {OrderId}",
                orderEvent.OrderId);

            // TODO: Publish PaymentProcessedEvent after successful payment
            // var paymentEvent = new PaymentProcessedEvent 
            // {
            //     OrderId = orderEvent.OrderId,
            //     PaymentId = Guid.NewGuid(),
            //     Amount = orderEvent.TotalAmount,
            //     Status = PaymentStatus.Completed,
            //     ProcessedAt = DateTime.UtcNow
            // };
            // await context.Publish(paymentEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process payment for Order {OrderId}",
                orderEvent.OrderId);

            throw; // Re-throw to trigger retry mechanism
        }
    }

    private async Task ProcessPaymentAsync(OrderCreatedEvent orderEvent)
    {
        // Simulate payment gateway call
        _logger.LogInformation(
            "Processing payment for Order {OrderId} with amount {Amount}",
            orderEvent.OrderId, orderEvent.TotalAmount);

        // Simulate processing delay
        await Task.Delay(1000);

        // Simulate random payment failures for testing
        var random = new Random();
        if (random.NextDouble() < 0.1) // 10% failure rate
        {
            throw new InvalidOperationException("Payment gateway temporarily unavailable");
        }

        _logger.LogInformation(
            "Payment successfully processed for Order {OrderId}",
            orderEvent.OrderId);
    }
}