using System;
using System.ComponentModel.DataAnnotations;
namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for Tag.
    /// </summary>
    public class Tag
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 1 and 80 Characters!")]
        [MinLength(1, ErrorMessage = "Length needs to be between 1 and 80 Characters!")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = "";

        [Required]
        public int TicketId { get; set; } = default!;

        [Required]
        public Ticket Ticket { get; set; } = default!;

        public override string ToString() => $"{Name}";
    }
}