using Exchange.Application.Interfaces;
using Exchange.Application.Services;
using Exchange.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Exchange.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IExchangeRatesService, ExchangeRatesService>();
        services.AddSingleton<IExchangeInputValidator, ExchangeInputValidator>();
        return services;
    }
}
