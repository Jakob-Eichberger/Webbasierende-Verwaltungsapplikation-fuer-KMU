using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// Model Class for Tag.
    /// </summary>
    public class TagDto
    {
        [Required]
        [MaxLength(80, ErrorMessage = "Length needs to be between 1 and 80 Characters!")]
        [MinLength(1, ErrorMessage = "Length needs to be between 1 and 80 Characters!")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = "";

    }
}