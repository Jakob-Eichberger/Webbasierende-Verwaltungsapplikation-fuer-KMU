using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Enum for Priority values.
    /// </summary>
    public enum Priority { Lowest = 1, Low = 2, Medium = 3, High = 4, Highest = 5 }

    /// <summary>
    /// Modell Class for Ticket.
    /// </summary>
    public class Ticket
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "")]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "")]
        [MaxLength(5000, ErrorMessage = "Max Length of 500 reached!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = "";

        public List<Tag> Tags { get; private set; } = new();

        [Required(ErrorMessage = "")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastUpdated { get; set; }

        [Required(ErrorMessage = "")]
        public Priority Priority { get; set; }

        [Required(ErrorMessage = "")]
        public bool SendNotification { get; set; } = true;

        public List<TimeRecordingElement> TimeRecordingElements { get; private set; } = new();

        public List<Element> Elements { get; private set; } = new();

        [MaxLength(5000, ErrorMessage = "Max Length of 500 reached!")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; } = "";

        /// <summary>
        /// Party which has the Employee <see cref="Rights"/> which is currently working on the ticket.
        /// </summary>
        public int? EmployeePartyId { get; set; }

        /// </summary>
        /// Party which has the Employee which is currently working on the ticket.
        /// </summary>
        public Party? EmployeeParty { get; set; } = default!;

        [Required]
        public int OrderId { get; set; }

        [Required]
        public Order Order { get; set; } = default!;

        [Required]
        public int StatusId { get; set; }
        [Required]
        public Status Status { get; set; } = default!;

        public List<Document> Documents { get; private set; } = new(); //DateienHochladen Prop 

        public List<Conversation> Messages { get; private set; } = new();

        public override string ToString() => $"{Id}, {Name}, {Description}, {String.Join<Tag>(",", Tags)}, {Note}, {Status}";
    }
}