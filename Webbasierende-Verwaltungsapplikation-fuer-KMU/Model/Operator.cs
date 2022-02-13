using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    public class Operator
    {
        public int Id { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = ("Length must be alteast one character long."))]
        [MaxLength(200, ErrorMessage = ("Cant be more then 200 characters long."))]
        public string Name { get; set; } = default!;

        [Required]
        public string UID { get; set; } = default!;

        [Required]
        [MinLength(1, ErrorMessage = ("Length must be alteast one character long."))]
        [DataType(DataType.MultilineText)]
        public string TermsAndConditions { get; set; } = default!;

        [Required]
        [DataType(DataType.EmailAddress)]
        public string OfficeEmail { get; set; } = default!;

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = default!;

        [Required]
        public string Owner { get; set; } = default!;

        public int? OfficeAddressId { get; set; } = default;

        public Address? OfficeAddress { get; set; } = default;
    }
}
