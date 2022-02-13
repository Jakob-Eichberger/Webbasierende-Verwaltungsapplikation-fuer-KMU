using System.Text.Json.Serialization;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model.PayPalResult
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Address
    {
        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }
    }
}
