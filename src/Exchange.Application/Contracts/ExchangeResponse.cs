namespace Exchange.Application.Contracts;

public sealed record ExchangeResponse(
    DateOnly Date,
    string MainCurrency,
    string MoneyCurrency,
    decimal ExchangeRate);
