using System.ComponentModel.DataAnnotations;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    public class PhoneNumberDto
    {
        [Required]
        [MaxLength(20, ErrorMessage = "die Telefonnummer muss maximal 20 ziffer enthalten")]
        public string Number { get; set; } = "";

        [Required]
        public PhoneNumberType Type { get; set; }
    }
}