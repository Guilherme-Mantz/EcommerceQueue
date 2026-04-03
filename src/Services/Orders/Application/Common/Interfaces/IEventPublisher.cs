namespace Orders.Application.Common.Interfaces;

/// <summary>
/// Abstraction for publishing domain events to message broker
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to the message broker
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="eventData">Event data to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class;
}