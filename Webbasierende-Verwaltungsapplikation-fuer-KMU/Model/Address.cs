using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for Address.
    /// </summary>
    public class Address
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public Party? Party { get; set; }

        [JsonIgnore]
        public int? PartyId { get; set; }

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string Country { get; set; } = default!;

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string State { get; set; } = default!;

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string ZipCode { get; set; } = default!;

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string City { get; set; } = default!;

        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string? Street { get; set; } = default!;

        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string? StairCase { get; set; }

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string HouseNumber { get; set; } = default!;

        [DataType(DataType.Text)]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        public string? DoorNumber { get; set; } = default!;

        public bool IsPrimary { get; set; } = false;

        public override string ToString() => $"{Country}, {State}, {ZipCode}, {City}, {Street}, {StairCase}, {HouseNumber}, {DoorNumber}";
        public string String => $"{Street} {HouseNumber}/{DoorNumber}\n{ZipCode} {City}\n{Country}";
    }
}