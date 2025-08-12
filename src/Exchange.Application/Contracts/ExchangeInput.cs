using Exchange.Domain.ValueObjects;

namespace Exchange.Application.Contracts;

public sealed record ExchangeInput(
    CurrencyCode MainCurrency,
    CurrencyCode MoneyCurrency,
    decimal Amount
);
