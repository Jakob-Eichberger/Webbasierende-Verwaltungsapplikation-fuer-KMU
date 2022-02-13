using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// Model Class for Address.
    /// </summary>
    public class AddressDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Das obere Feld muss ausgefüllt sein!")]
        [DataType(DataType.Text)]
        [MinLength(2, ErrorMessage = "Der Name des Landes muss zwischen 2 und 80 Buchstaben lang sein!")]
        [MaxLength(80, ErrorMessage = "Der Name des Landes muss zwischen 2 und 80 Buchstaben lang sein!")]
        public string Country { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Das obere Feld muss ausgefüllt sein!")]
        [DataType(DataType.Text)]
        [MinLength(4, ErrorMessage = "Der Name des Bundeslandes muss zwischen 4 und 80 Buchstaben lang sein!")]
        [MaxLength(80, ErrorMessage = "SDer Name des Bundeslandes muss zwischen 4 und 80 Buchstaben lang sein!")]
        public string State { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Das obere Feld muss ausgefüllt sein!")]
        [DataType(DataType.Text)]
        [MinLength(4, ErrorMessage = "Die Postleitzahl muss zwischen 4 und 80 Ziffer lang sein!")]
        [MaxLength(80, ErrorMessage = "Die Postleitzahl muss zwischen 4 und 80 Ziffer lang sein!")]
        public string ZipCode { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Das obere Feld muss ausgefüllt sein!")]
        [DataType(DataType.Text)]
        [MinLength(4, ErrorMessage = "Der Name der Stadt muss zwischen 4 und 80 Buchstaben lang sein!")]
        [MaxLength(80, ErrorMessage = "Der Name der Stadt muss zwischen 4 und 80 Buchstaben lang sein!")]
        public string City { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Das obere Feld muss ausgefüllt sein!")]
        [DataType(DataType.Text)]
        [MinLength(3, ErrorMessage = "Der Name der Straße muss zwischen 3 und 80 Buchstaben lang sein!")]
        [MaxLength(80, ErrorMessage = "Der Name der Stadt muss zwischen 3 und 80 Buchstaben lang sein!")]
        public string Street { get; set; } = default!;

        [DataType(DataType.Text)]
        [MinLength(1, ErrorMessage = "Die Stiegenummer muss zwischen 1 und 80 Ziffer lang sein!")]
        [MaxLength(80, ErrorMessage = "Die Stiegenummer muss zwischen 1 und 80 Ziffer lang sein!")]
        public string? StairCase { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Das obere Feld muss ausgefüllt sein!")]
        [DataType(DataType.Text)]
        [MinLength(1, ErrorMessage = "Die Hausnummer muss zwischen 1 und 80 Ziffer lang sein!")]
        [MaxLength(80, ErrorMessage = "Die Hausnummer muss zwischen 1 und 80 Ziffer lang sein!")]
        public string HouseNumber { get; set; } = default!;

        [DataType(DataType.Text)]
        [MinLength(1, ErrorMessage = "Die Türnummer muss zwischen 1 und 80 Ziffer lang sein!")]
        [MaxLength(80, ErrorMessage = "Die Türnummer muss zwischen 1 und 80 Ziffer lang sein!")]
        public string? DoorNumber { get; set; } = default!;

        public bool IsPrimary { get; set; } = false;


    }
}