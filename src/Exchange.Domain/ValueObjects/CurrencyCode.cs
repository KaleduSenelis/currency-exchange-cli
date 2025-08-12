namespace Exchange.Domain.ValueObjects;

public sealed record CurrencyCode
{
    private static readonly HashSet<string> SupportedCurrencies =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "EUR", "USD", "GBP", "SEK", "NOK", "CHF", "JPY", "DKK"
        };

    public static IReadOnlyCollection<string> GetSupportedCurrencies() =>
    SupportedCurrencies
        .OrderBy(c => c)
        .ToList()
        .AsReadOnly();

    public string Value { get; }

    private CurrencyCode(string currencyCode) => Value = currencyCode;

    public static bool TryCreate(string? input, out CurrencyCode? currencyCode, out string? errorMessage)
    {
        currencyCode = null;
        errorMessage = null;

        if (input is null || input.Length != 3)
        {
            errorMessage = "Currency code must be exactly 3 characters.";
            return false;
        }

        var upperCurrencyCode = input.ToUpperInvariant();

        if (!SupportedCurrencies.Contains(upperCurrencyCode))
        {
            errorMessage = $"Unsupported currency code: {input}";
            return false;
        }

        currencyCode = new CurrencyCode(upperCurrencyCode);
        return true;
    }
}
