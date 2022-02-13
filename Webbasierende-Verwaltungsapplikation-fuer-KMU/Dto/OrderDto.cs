using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// Model Class for Order.
    /// </summary>
    public class OrderDto
    {
        //[Required(ErrorMessage = "")]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = default!;

        //[Required(ErrorMessage = "")]
        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [DataType(DataType.Text)]
        public string Description { get; set; } = default!;

        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [DataType(DataType.Text)]
        public string Note { get; set; } = default!;

        public int BillingAddressId { get; set; } = default!;

        [Required]
        public int DeliveryAddressId { get; set; }

    }
}