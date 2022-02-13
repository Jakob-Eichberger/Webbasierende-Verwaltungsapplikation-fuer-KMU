using System;
using System.Text.Json.Serialization;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model.PayPalResult
{
    public class Result
    {
        [JsonPropertyName("intent")]
        public string Intent { get; set; }

        [JsonPropertyName("create_time")]
        public DateTime CreateTime { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("payer")]
        public Payer Payer { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("update_time")]
        public DateTime UpdateTime { get; set; }
    }


}
