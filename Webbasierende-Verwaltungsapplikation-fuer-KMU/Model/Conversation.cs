using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Enum Class for MessageCreatedBy.
    /// </summary>
    public enum MessageCreatedBy { Customer = 1, Employee = 2 }

    /// <summary>
    /// Model Class for Message.
    /// </summary>
    public class Conversation
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public int? TicketId { get; set; }

        [JsonIgnore]
        public Ticket? Ticket { get; set; }

        [JsonIgnore]
        public int? OrderId { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }

        [JsonIgnore]
        public int CreatedByPartyId { get; set; }

        [JsonIgnore]
        public Party CreatedByParty { get; set; } = default!;

        public List<Message> Messages { get; private set; } = new();

        public List<Document> Documents { get; private set; } = new();


    }
}