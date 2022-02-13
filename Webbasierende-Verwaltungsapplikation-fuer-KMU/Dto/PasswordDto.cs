using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    public class PasswordDto
    {
        [MinLength(8, ErrorMessage = "Das Passwort muss länger als 8 Zeichnen sein!")]
        [MaxLength(50, ErrorMessage = "Das Passwort muss nicht länger als 50 Zeichnen sein")]
        [Required]
        public string First { get; set; } = default!;

        [MinLength(8)]
        [MaxLength(50)]
        [Required]
        public string Second { get; set; } = default!;
    }
}
