using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    public class TimeRecordingElement
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
        public decimal Minutes { get; set; } = 0.0m;

        [DataType(DataType.Currency)]
        [MaxLength(100, ErrorMessage = "Max Length of 100 Characters for Price reached!")]
        public string PricePerHour { get; set; } = 0.0m.ToString();

        /// <summary>
        /// Party which has the Employee <see cref="Rights"/> which created the <see cref="TimeRecordingElement"/>.
        /// </summary>
        public Party Party { get; set; } = default!;

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = default!;
    }
}
