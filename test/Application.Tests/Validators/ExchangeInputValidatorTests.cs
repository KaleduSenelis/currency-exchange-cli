using Exchange.Application.Validators;
using FluentAssertions;

namespace Application.Tests.Validators;

public class ExchangeInputValidatorTests
{
    private readonly ExchangeInputValidator _sut;

    public ExchangeInputValidatorTests()
    {
        _sut = new ExchangeInputValidator();
    }

    [Theory]
    [InlineData("Exchange EUR/USD 1")]
    [InlineData("exchange eur/usd 3,45")]
    [InlineData("EXCHANGE Eur/Usd 8.888")]
    public void TryValidate_ReturnsTrue_ForValidInput(string? input)
    {
        //Act
        var result = _sut.TryValidate(input, out var exchangeInput, out var errorMessage);

        //Assert
        result.Should().BeTrue();
        errorMessage.Should().BeNull();
        exchangeInput.Should().NotBeNull();
        exchangeInput.MainCurrency.Value.Should().Be("EUR");
        exchangeInput.MoneyCurrency.Value.Should().Be("USD");
        exchangeInput.Amount.Should().BeGreaterThan(0m);
    }

    [Theory]
    [InlineData("Exchange EUR-DKK 1")] 
    [InlineData("Exchange EUR/DKK")]
    [InlineData("Exchange EUR/DKK/USD 2")]
    [InlineData("Exchange EUR/DKK -1")]
    public void TryValidate_ReturnsFalse_ForInvalidInput(string? input)
    {
        //Act
        var result = _sut.TryValidate(input, out var exchangeInput, out var errorMessage);

        //Assert
        result.Should().BeFalse();
        exchangeInput.Should().BeNull();
        errorMessage.Should().NotBeNullOrWhiteSpace();
    }
}
