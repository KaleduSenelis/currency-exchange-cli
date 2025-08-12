using Exchange.Application.Contracts;
using Exchange.Application.Interfaces;
using Exchange.Infrastructure.Clients.DTOs;
using Exchange.Infrastructure.Mappers;
using System.Xml.Serialization;

namespace Exchange.Infrastructure.Clients;

public sealed class ExchangeRatesClient(HttpClient httpClient) : IExchangeRatesClient
{
    private const string RequestUri = "webservices/fxrates/fxrates.asmx/getFxRatesForCurrency";

    public async Task<ExchangeResponse> GetExchangeRate(string currency, CancellationToken cancellationToken)
    {
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("tp", "EU"),
            new KeyValuePair<string, string>("ccy", $"{currency}"),
            new KeyValuePair<string, string>("dtFrom", ""),
            new KeyValuePair<string, string>("dtTo", "")
        ]);

        var response = await httpClient.PostAsync(RequestUri, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var xml = await response.Content.ReadAsStringAsync(cancellationToken);

        var exchangeResponse = ParseXml(xml);

        return exchangeResponse;
    }

    private static ExchangeResponse ParseXml(string xml)
    {
        var serializer = new XmlSerializer(typeof(FxRates));

        using var reader = new StringReader(xml);

        var fxRates = (FxRates)serializer.Deserialize(reader)!;

        return FxRatesMapper.ToExchangeResponse(fxRates);
    }
}
