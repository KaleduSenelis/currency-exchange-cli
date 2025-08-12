using Exchange.Application.Interfaces;
using Exchange.Domain.ValueObjects;

namespace Exchange.Cli;

public sealed class App(
    IExchangeRatesService exchangeRatesService,
    IExchangeInputValidator exchangeInputValidator)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        PrintUsage();

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("> ");

            var input = Console.ReadLine()?.Trim();

            if (input is null)
            {
                continue;
            }

            switch (input.ToLowerInvariant())
            {
                case "":
                    continue;
                case "exit":
                    return;
                case "list":
                    PrintSupportedCurrencies();
                    continue;
                case "help":
                    PrintUsage();
                    continue;
            }

            if (!exchangeInputValidator.TryValidate(input, out var exchangeInput, out var errorMessage))
            {
                Console.WriteLine(errorMessage);
                continue;
            }

            try
            {
                var exchangedAmount = await exchangeRatesService.GetExchangedAmount(
                    exchangeInput!,
                    cancellationToken
                );

                Console.WriteLine(Math.Round(exchangedAmount, 4, MidpointRounding.AwayFromZero));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not calculate exchanged amount: {ex.Message}");
            }
        }
    }

    private void PrintUsage()
    {
        Console.WriteLine("Usage: Exchange <currency pair> <amount to exchange>");
        Console.WriteLine("Example: Exchange EUR/DKK 1");
        Console.WriteLine();
        Console.WriteLine("Available commands:");
        Console.WriteLine(" list - Show supported currencies");
        Console.WriteLine(" help - Show this help message");
        Console.WriteLine(" exit - Quit the application");
    }

    private void PrintSupportedCurrencies()
    {
        Console.WriteLine("Supported currencies:");
        foreach (var currency in CurrencyCode.GetSupportedCurrencies())
        {
            Console.WriteLine($"- {currency}");
        }
    }
}
