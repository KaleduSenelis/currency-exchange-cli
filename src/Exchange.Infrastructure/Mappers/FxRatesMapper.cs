using Exchange.Application.Contracts;
using Exchange.Infrastructure.Clients.DTOs;
using System.Globalization;

namespace Exchange.Infrastructure.Mappers
{
    internal static class FxRatesMapper
    {
        internal static ExchangeResponse ToExchangeResponse(FxRates fxRates)
        {
            if (fxRates.FxRate is null)
                throw new InvalidOperationException("FxRate element is missing");

            if (fxRates.FxRate.Amounts?.Count != 2)
                throw new InvalidOperationException("Expected exactly two CcyAmt elements");

            var date = DateOnly.Parse(fxRates.FxRate.Date, CultureInfo.InvariantCulture);
            var mainCurrency = fxRates.FxRate.Amounts![0].Currency;
            var moneyCurrency = fxRates.FxRate.Amounts[1].Currency;
            var rate = decimal.Parse(fxRates.FxRate.Amounts[1].Amount, CultureInfo.InvariantCulture);

            return new ExchangeResponse(date, mainCurrency, moneyCurrency, rate);
        }
    }
}
