using Exchange.Infrastructure.Clients.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exchange.Infrastructure.Jobs;

public class CacheClearJob(
    IMemoryCache memoryCache,
    IOptions<ExchangeRatesClientOptions> options,
    ILogger<CacheClearJob> logger) : BackgroundService
{
    private readonly TimeSpan _cleanTime = options.Value.CacheClearTimeUtc!.Value;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var todayCleanTime = now.Date + _cleanTime;
            var nextCleanTime = todayCleanTime > now ? todayCleanTime : todayCleanTime.AddDays(1);
            var cleanDelay = nextCleanTime - now;

            logger.LogInformation($"Cache will be cleared at {nextCleanTime} UTC, in {cleanDelay}");

            try
            {
                await Task.Delay(cleanDelay, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (memoryCache is MemoryCache concreteMemoryCache)
            {
                concreteMemoryCache.Clear();
            }
            else
            {
                logger.LogWarning("Could not clear cache");
            }
        }
    }
}
