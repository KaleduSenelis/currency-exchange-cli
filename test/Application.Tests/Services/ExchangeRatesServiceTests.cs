using Exchange.Application.Contracts;
using Exchange.Application.Interfaces;
using Exchange.Application.Services;
using Exchange.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Application.Tests.Services
{
    public class ExchangeRatesServiceTests
    {
        private readonly Mock<IExchangeRatesClient> _mockClient;
        private readonly ExchangeRatesService _sut;

        public ExchangeRatesServiceTests()
        {
            _mockClient = new Mock<IExchangeRatesClient>();
            _sut = new ExchangeRatesService(_mockClient.Object);
        }

        private readonly string _eur = "EUR";
        private readonly string _dkk = "DKK";
        private readonly string _usd = "USD";
        private readonly decimal _eurDkkRate = 7.45m;
        private readonly decimal _eurUsdRate = 1.17m;

        [Fact]
        public async Task GetExchangeAmount_WithTwoNonEurCurrencies_CallsClientTwice()
        {
            //Arrange
            SetupClient(_eur, _dkk, _eurDkkRate);
            SetupClient(_eur, _usd, _eurUsdRate);

            var exchangeInput = new ExchangeInput(Code(_usd), Code(_dkk), default);

            //Act
            var result = await _sut.GetExchangedAmount(exchangeInput, CancellationToken.None);

            //Assert
            _mockClient.Verify(c => c.GetExchangeRate(_dkk, It.IsAny<CancellationToken>()), Times.Once);
            _mockClient.Verify(c => c.GetExchangeRate(_usd, It.IsAny<CancellationToken>()), Times.Once);
            _mockClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetExchangeAmount_WithOneNonEurCurrency_CallsClientOnlyForNonEurCurrency()
        {
            //Arrange
            SetupClient(_eur, _dkk, _eurDkkRate);

            var exchangeInput = new ExchangeInput(Code(_eur), Code(_dkk), default);

            //Act
            var result = await _sut.GetExchangedAmount(exchangeInput, CancellationToken.None);

            //Assert
            _mockClient.Verify(c => c.GetExchangeRate(_dkk, It.IsAny<CancellationToken>()), Times.Once);
            _mockClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetExchangeAmount_WithTwoNonEurCurrencies_CalculatesExchangedAmountCorrectly()
        {
            //Arrange
            var amount = 4m;
            SetupClient(_eur, _dkk, _eurDkkRate);
            SetupClient(_eur, _usd, _eurUsdRate);

            var exchangeInput = new ExchangeInput(Code(_usd), Code(_dkk), amount);

            //Act
            var result = await _sut.GetExchangedAmount(exchangeInput, CancellationToken.None);

            //Assert
            result.Should().Be(amount * (_eurDkkRate / _eurUsdRate));
        }

        [Fact]
        public async Task GetExchangeAmount_WithTwoSameCurrencies_ReturnsSameAmount()
        {
            //Arrange
            var amount = 5m;
            SetupClient(_eur, _usd, _eurUsdRate);

            var exchangeInput = new ExchangeInput(Code(_usd), Code(_usd), amount);

            //Act
            var result = await _sut.GetExchangedAmount(exchangeInput, CancellationToken.None);

            //Assert
            result.Should().Be(amount);
        }

        [Fact]
        public async Task GetExchangeRate_ReactsToCancellationToken()
        {
            var cts = new CancellationTokenSource();

            _mockClient
                .Setup(c => c.GetExchangeRate(_usd, It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cts.Token);
                    return new ExchangeResponse(DateOnly.FromDateTime(DateTime.UtcNow), _eur, _usd, _eurUsdRate);
                });

            var input = new ExchangeInput(Code(_usd), Code(_eur), 1m);

            cts.Cancel();

            var act = async () => await _sut.GetExchangedAmount(input, cts.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();
        }


        private void SetupClient(string mainCurrency, string moneyCurrency, decimal exchangeRate)
        {
            _mockClient
                .Setup(client => client.GetExchangeRate(moneyCurrency, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ExchangeResponse(
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    mainCurrency,
                    moneyCurrency,
                    exchangeRate));
        }

        private static CurrencyCode Code(string currency)
        {
            CurrencyCode.TryCreate(currency, out var currencyCode, out var _).Should().BeTrue();
            return currencyCode!;
        }
    }
}
