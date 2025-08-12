using Exchange.Application.Contracts;

namespace Exchange.Application.Interfaces;

public interface IExchangeInputValidator
{
    bool TryValidate(string? input, out ExchangeInput? exchangeInput, out string? errorMessage);
}
