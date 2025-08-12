using Exchange.Application.Contracts;

namespace Exchange.Application.Interfaces;

public interface IExchangeRatesService
{
    Task<decimal> GetExchangedAmount(ExchangeInput exchangeInput, CancellationToken cancellationToken);
}
