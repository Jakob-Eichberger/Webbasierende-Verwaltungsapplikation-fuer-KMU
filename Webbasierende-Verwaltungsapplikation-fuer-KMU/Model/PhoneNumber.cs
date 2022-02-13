using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    public enum PhoneNumberType { Other = 0, Mobile = 1, Home = 2 }

    public class PhoneNumber
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string? Number { get; set; }

        [JsonIgnore]
        [Required]
        public PhoneNumberType Type { get; set; }

        public override string ToString() => $"{Number}";

    }
}