using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Messaging;

public interface IIdempotencyService
{
    Task<bool> IsProcessedAsync(string key);
    Task MarkAsProcessedAsync(string key, TimeSpan? expiration = null);
}

public class RedisIdempotencyService(
    IDistributedCache cache,
    ILogger<RedisIdempotencyService> logger) : IIdempotencyService
{
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<RedisIdempotencyService> _logger = logger;

    public async Task<bool> IsProcessedAsync(string key)
    {
        var value = await _cache.GetStringAsync($"idempotency:{key}");
        return value != null;
    }

    public async Task MarkAsProcessedAsync(string key, TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromDays(7)
        };

        await _cache.SetStringAsync(
            $"idempotency:{key}",
            DateTime.UtcNow.ToString("O"),
            options
        );

        _logger.LogInformation("Marked {Key} as processed", key);
    }
}
