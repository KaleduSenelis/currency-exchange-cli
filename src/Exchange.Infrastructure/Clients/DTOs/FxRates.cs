using System.Xml.Serialization;

namespace Exchange.Infrastructure.Clients.DTOs
{
    [XmlRoot("FxRates", Namespace = "http://www.lb.lt/WebServices/FxRates")]
    public class FxRates
    {
        [XmlElement("FxRate")]
        public FxRate FxRate { get; set; } = null!;
    }

    public class FxRate
    {
        [XmlElement("Tp")]
        public string Type { get; set; } = null!;

        [XmlElement("Dt")]
        public string Date { get; set; } = null!;

        [XmlElement("CcyAmt")]
        public List<CcyAmt> Amounts { get; set; } = new();
    }

    public class CcyAmt
    {
        [XmlElement("Ccy")]
        public string Currency { get; set; } = null!;

        [XmlElement("Amt")]
        public string Amount { get; set; } = null!;
    }
}
