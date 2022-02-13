using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// Model Class for Element.
    /// </summary>
    public class ElementDto
    {
        [Required]
        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [MinLength(10, ErrorMessage = "Description needs to be atleast 10 Characters long!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = default!;

        [MaxLength(500, ErrorMessage = "Max Length of 500 Characters for Ammount reached!")]
        [Required]
        public string Ammount { get; set; } = default!;

        [DataType(DataType.Currency)]
        [MaxLength(100, ErrorMessage = "Max Length of 100 Characters for Price reached!")]
        [Required]
        public string Price { get; set; } = default!;
    }
}