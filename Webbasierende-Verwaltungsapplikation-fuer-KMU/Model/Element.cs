using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for Element.
    /// </summary>
    public class Element
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "")]
        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [MinLength(10, ErrorMessage = "Description needs to be atleast 10 Characters long!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; } = default!;

        [MaxLength(500, ErrorMessage = "Max Length of 500 Characters for Ammount reached!")]
        public string Ammount { get; set; } = default!;

        [DataType(DataType.Currency)]
        [MaxLength(100, ErrorMessage = "Max Length of 100 Characters for Price reached!")]
        public string PricePerItem { get; set; } = default!;

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = default!;
    }
}