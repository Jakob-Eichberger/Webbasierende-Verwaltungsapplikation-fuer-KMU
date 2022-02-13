using System.Text.Json.Serialization;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model.PayPalResult
{
    public class Name
    {
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }
    }


}
