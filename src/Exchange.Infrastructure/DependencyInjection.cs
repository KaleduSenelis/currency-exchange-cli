using Exchange.Application.Interfaces;
using Exchange.Infrastructure.Clients;
using Exchange.Infrastructure.Clients.Options;
using Exchange.Infrastructure.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Exchange.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<ExchangeRatesClientOptions>()
            .BindConfiguration("ExchangeRatesClientOptions")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient<IExchangeRatesClient, ExchangeRatesClient>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ExchangeRatesClientOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddStandardResilienceHandler();

        services.Decorate<IExchangeRatesClient, CachedExchangeRatesClient>();

        services.AddMemoryCache();
        services.AddHostedService<CacheClearJob>();

        return services;
    }
}
