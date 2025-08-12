using Exchange.Application.Contracts;

namespace Exchange.Infrastructure.Fallback;

internal static class FallbackExchangeRates
{
    private static readonly Dictionary<string, decimal> _rates = new(StringComparer.OrdinalIgnoreCase)
    {
        {"EUR", 1m },
        {"USD", 1.17m },
        {"GBP", 0.87m },
        {"SEK", 11.15m },
        {"NOK", 11.97m },
        {"CHF", 0.94m },
        {"JPY", 160.5m },
        {"DKK", 7.45m }
    };

    internal static bool TryGetRate(string currency, out ExchangeResponse fallbackExchangeRate)
    {
        if (_rates.TryGetValue(currency, out var rate))
        {
            fallbackExchangeRate = new ExchangeResponse(
                DateOnly.FromDateTime(DateTime.UtcNow),
                "EUR",
                currency,
                rate
            );
            return true;
        }

        fallbackExchangeRate = null!;
        return false;
    }
}
