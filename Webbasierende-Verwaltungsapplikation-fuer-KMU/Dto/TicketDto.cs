using System;
using System.ComponentModel.DataAnnotations;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{

    /// <summary>
    /// Modell Class for Ticket.
    /// </summary>
    public class TicketDto
    {
        public TicketDto()
        {
        }

        public TicketDto(string name, string description, DateTime created, Priority priority, string note, int? employeePartyId, int statusId)
        {
            Name = name;
            Description = description;
            Priority = priority;
            Note = note;
            EmployeePartyId = employeePartyId;
            StatusId = statusId;
        }

        [Required]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = "";

        [Required]
        [MaxLength(5000, ErrorMessage = "Max Length of 500 reached!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = "";

        [Required]
        public Priority Priority { get; set; }

        //Notizen, Firmenintern only
        [MaxLength(5000, ErrorMessage = "Max Length of 500 reached!")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; } = "";

        /// <summary>
        /// Party which has the Employee <see cref="Rights"/> which is currently working on the ticket.
        /// </summary>
        public int? EmployeePartyId { get; set; }

        [Required]
        public int StatusId { get; set; }

    }
}