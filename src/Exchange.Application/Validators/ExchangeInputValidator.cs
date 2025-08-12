using Exchange.Application.Contracts;
using Exchange.Application.Interfaces;
using Exchange.Domain.ValueObjects;
using System.Globalization;

namespace Exchange.Application.Validators;

public class ExchangeInputValidator : IExchangeInputValidator
{
    public bool TryValidate(string? input, out ExchangeInput? exchangeInput, out string? errorMessage)
    {
        exchangeInput = null;
        errorMessage = null;

        var inputArguments = input!.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (inputArguments.Length != 3)
        {
            errorMessage = HelpMessage();
            return false;
        }

        var commandArgument = inputArguments[0];
        var currencyPairArgument = inputArguments[1];
        var amountArgument = inputArguments[2];

        if (!commandArgument.Equals("exchange", StringComparison.InvariantCultureIgnoreCase))
        {
            errorMessage = HelpMessage();
            return false;
        }

        if (!currencyPairArgument.Contains('/'))
        {
            errorMessage = HelpMessage();
            return false;
        }

        var currencyPair = currencyPairArgument.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (currencyPair.Length != 2)
        {
            errorMessage = "Currency pair must contain exactly two currencies.";
            return false;
        }

        var rawAmount = amountArgument.Replace(',', '.');

        var successfulParse = decimal.TryParse(
            rawAmount,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var amount
        );

        if (!successfulParse || amount < 0)
        {
            errorMessage = "Amount must be a valid, non-negative number.";
            return false;
        }

        if (!CurrencyCode.TryCreate(currencyPair[0], out var mainCurrency, out var errorMainCurrency))
        {
            errorMessage = errorMainCurrency; 
            return false; 
        }

        if (!CurrencyCode.TryCreate(currencyPair[1], out var moneyCurrency, out var errorMoneyCurrency))
        { 
            errorMessage = errorMoneyCurrency; 
            return false; 
        }

        exchangeInput = new ExchangeInput(mainCurrency!, moneyCurrency!, amount);
        return true;
    }

    private string HelpMessage() =>
        "Usage: Exchange <currency pair> <amount to exchange>\n" +
        "Example: Exchange EUR/DKK 1";
}
