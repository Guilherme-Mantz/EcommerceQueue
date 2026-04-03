using MassTransit;
using Microsoft.Extensions.Logging;
using Orders.Application.Common.Interfaces;

namespace Orders.Infrastructure.Messaging;

/// <summary>
/// MassTransit implementation of IEventPublisher
/// </summary>
public class MassTransitEventPublisher(
    IPublishEndpoint publishEndpoint,
    ILogger<MassTransitEventPublisher> logger) : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ILogger<MassTransitEventPublisher> _logger = logger;

    public async Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _logger.LogDebug("Publishing event {EventType}", typeof(T).Name);
            
            await _publishEndpoint.Publish(eventData, cancellationToken);
            
            _logger.LogInformation("Successfully published event {EventType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType}", typeof(T).Name);
            throw;
        }
    }
}