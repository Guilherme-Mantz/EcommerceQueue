using Microsoft.Extensions.Caching.Memory;

namespace Messaging;

/// <summary>
/// In-memory implementation of IIdempotencyService for development/testing
/// </summary>
public class InMemoryIdempotencyService(IMemoryCache cache) : IIdempotencyService
{
    private readonly IMemoryCache _cache = cache;

    public Task<bool> IsProcessedAsync(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }

    public Task MarkAsProcessedAsync(string key, TimeSpan? expiration = null)
    {
        var cacheExpiration = expiration ?? TimeSpan.FromHours(1);
        _cache.Set(key, true, cacheExpiration);
        return Task.CompletedTask;
    }
}