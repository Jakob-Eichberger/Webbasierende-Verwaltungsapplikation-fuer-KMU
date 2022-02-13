using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    public enum Role { Admin = 4, Employee = 3, Company = 2, Customer = 1 }

    public abstract class Party
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; } = default!;

        public abstract string Fullname { get; }

        public List<Order> Orders { get; private set; } = new();

        [Required(ErrorMessage = "")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EMail { get; set; } = "";

        public List<PhoneNumber> PhoneNumbers { get; private set; } = new();

        public List<Address> Address { get; private set; } = new();

        public List<Conversation> Conversations { get; private set; } = new();

        [JsonIgnore]
        public List<Document> Documents { get; private set; } = new();

        [JsonIgnore]
        [Required]
        public Rights Rights { get; set; } = new();

        [JsonIgnore]
        [Required]
        public Role Role { get; set; }

        public override string ToString() => $"{Fullname}, {EMail}, {User}, {String.Join<Address>(",", Address ?? new())}, {String.Join<PhoneNumber>(",", PhoneNumbers ?? new())}";
    }
}
