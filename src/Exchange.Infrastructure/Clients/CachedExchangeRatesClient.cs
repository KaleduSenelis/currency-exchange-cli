using Exchange.Application.Contracts;
using Exchange.Application.Interfaces;
using Exchange.Infrastructure.Clients.Options;
using Exchange.Infrastructure.Fallback;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exchange.Infrastructure.Clients;

public class CachedExchangeRatesClient(
    ExchangeRatesClient innerClient, 
    IMemoryCache memoryCache, 
    IOptions<ExchangeRatesClientOptions> options,
    ILogger<CachedExchangeRatesClient> logger) : IExchangeRatesClient
{
    private readonly TimeSpan CacheTtl = options.Value.CacheTtl;
    public async Task<ExchangeResponse> GetExchangeRate(string currency, CancellationToken cancellationToken)
    {
        var cacheHit  = memoryCache.Get<ExchangeResponse>(currency);

        if (cacheHit is not null)
        {
            return cacheHit;
        }

        try
        {
            var freshRate = await innerClient.GetExchangeRate(currency, cancellationToken);
            memoryCache.Set(currency, freshRate, CacheTtl);

            return freshRate;
        }

        catch (Exception)
        {
            logger.LogWarning($"Failed to fetch exchange rate from external API for {currency}. Falling back to hardcoded rate.");
            
            if (FallbackExchangeRates.TryGetRate(currency, out var fallbackRate))
            { 
                return fallbackRate;
            }

            throw new InvalidOperationException($"No fallback rate available for {currency}");
        }
    }
}
