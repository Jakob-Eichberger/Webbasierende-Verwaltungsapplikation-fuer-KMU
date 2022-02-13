using System.Text.Json.Serialization;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model.PayPalResult
{
    public class Payer
    {
        [JsonPropertyName("address")]
        public Address Address { get; set; }

        [JsonPropertyName("email_address")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("payer_id")]
        public string PayerId { get; set; }
    }


}
