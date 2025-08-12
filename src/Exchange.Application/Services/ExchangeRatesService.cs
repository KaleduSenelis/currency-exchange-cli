using Exchange.Application.Contracts;
using Exchange.Application.Interfaces;
using Exchange.Domain.ValueObjects;

namespace Exchange.Application.Services;

public class ExchangeRatesService(
    IExchangeRatesClient client) : IExchangeRatesService
{
    private const string BaseCurrencyCode = "EUR";

    public async Task<decimal> GetExchangedAmount(
        ExchangeInput exchangeInput,
        CancellationToken cancellationToken)
    {
        if (exchangeInput.MainCurrency == exchangeInput.MoneyCurrency)
        {
            return exchangeInput.Amount;
        }

        var mainCurrencyRate = await GetExchangeRate(exchangeInput.MainCurrency, cancellationToken);
        var moneyCurrencyRate = await GetExchangeRate(exchangeInput.MoneyCurrency, cancellationToken);

        var exchangedAmount = exchangeInput.Amount * (moneyCurrencyRate / mainCurrencyRate);

        return exchangedAmount;
    }

    private async Task<decimal> GetExchangeRate(CurrencyCode currencyCode, CancellationToken cancellationToken)
    {
        if (currencyCode.Value == BaseCurrencyCode)
        {
            return 1m;
        }

        var exchangeRateResponse = await client.GetExchangeRate(currencyCode.Value, cancellationToken);

        return exchangeRateResponse.ExchangeRate;
    }
}
