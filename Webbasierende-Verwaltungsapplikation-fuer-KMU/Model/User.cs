using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for User.
    /// </summary>
    public class User
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        [EmailAddress]
        [MaxLength(255)]
        [DataType(DataType.EmailAddress)]
        public string LoginEMail { get; set; } = "";

        [JsonIgnore]
        [MaxLength(172)]
        public string Secret { get; set; } = null!;

        [JsonIgnore]
        [MaxLength(44)]
        public string? PasswordHash { get; set; } = null;

        public DateTime RegisteredAt { get; set; }

        public bool? Active { get; set; } = null;

        public Party? Party { get; set; }

        [JsonIgnore]
        public string? PasswordResetToken { get; set; }

        [JsonIgnore]
        public DateTime? PasswordResetTokenGeneratedAt { get; set; }

        public override string ToString() => LoginEMail;
    }
}
