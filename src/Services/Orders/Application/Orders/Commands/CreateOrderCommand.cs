using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Messaging;
using Observability;
using Orders.Application.Common.Interfaces;
using Contracts.Events;

namespace Orders.Application.Orders.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    decimal TotalAmount,
    List<OrderItemDto> Items
) : IRequest<CreateOrderResult>;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price
);

public record CreateOrderResult(
    Guid OrderId,
    bool IsSuccess,
    string? ErrorMessage = null
);

public class CreateOrderCommandHandler(
    IEventPublisher eventPublisher,
    IIdempotencyService idempotencyService,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{

    private static readonly ActivitySource s_activitySource =
        ActivitySourceProvider.CreateActivitySource("Orders.Application");

    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("CreateOrder");
        activity?.SetTag("order.customer_id", command.CustomerId.ToString());
        activity?.SetTag("order.amount", command.TotalAmount);

        var idempotencyKey = $"order:{command.CustomerId}:{command.TotalAmount}";
        if (await idempotencyService.IsProcessedAsync(idempotencyKey))
        {
            logger.LogWarning("Duplicate order attempt for customer {CustomerId}", command.CustomerId);
            return new CreateOrderResult(Guid.Empty, false, "Order already processed");
        }

        var orderId = Guid.NewGuid();

        var orderEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            CustomerId = command.CustomerId,
            TotalAmount = command.TotalAmount,
            Items = command.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList(),
            CreatedAt = DateTime.Now
        };

        await eventPublisher.PublishAsync(orderEvent, cancellationToken);

        await idempotencyService.MarkAsProcessedAsync(idempotencyKey);

        logger.LogInformation(
            "Order {OrderId} created successfully for customer {CustomerId} with amount {Amount}",
            orderId, command.CustomerId, command.TotalAmount);

        return new CreateOrderResult(orderId, true);
    }
}
