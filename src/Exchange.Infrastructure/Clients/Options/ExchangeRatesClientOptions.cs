using System.ComponentModel.DataAnnotations;

namespace Exchange.Infrastructure.Clients.Options;

public class ExchangeRatesClientOptions
{
    [Required, Url]
    public string BaseUrl { get; init; } = default!;

    [Range(typeof(TimeSpan), "00:01:00", "1.00:00:00", ErrorMessage = "CacheTtl must be between 1 minute and 1 day.")]
    public TimeSpan CacheTtl { get; init; }

    [Required]
    public TimeSpan? CacheClearTimeUtc { get; init; }
}
