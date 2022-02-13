using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    public class Message
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "")]
        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [MinLength(1, ErrorMessage = "Text needs to be atleast 1 character long")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; } = "";

        [Required(ErrorMessage = "")]
        [DataType(DataType.Date)]
        public DateTime MessageSent { get; private set; } = DateTime.Now;

        [Required(ErrorMessage = "")]
        public MessageCreatedBy CreatedBy { get; set; }

        [JsonIgnore]
        public int SendByPartyId { get; set; } = default!;

        [JsonIgnore]
        public Party SendByParty { get; set; } = default!;

        public List<Document> Documents { get; private set; } = new();
    }
}
