using Exchange.Application.Contracts;

namespace Exchange.Application.Interfaces;

public interface IExchangeRatesClient
{
    Task<ExchangeResponse> GetExchangeRate(string currency, CancellationToken cancellationToken);
}
