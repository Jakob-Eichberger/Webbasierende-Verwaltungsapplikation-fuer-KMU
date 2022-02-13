using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{

    public enum OrderStatus { Open = 0, AwaitingPayment = 1, Closed = 2 }

    /// <summary>
    /// Model Class for Order.
    /// </summary>
    public class Order
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Open;

        //[Required(ErrorMessage = "")]
        [MinLength(5, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [MaxLength(80, ErrorMessage = "Length needs to be between 5 and 80 Characters!")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = "";

        //[Required(ErrorMessage = "")]
        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [DataType(DataType.Text)]
        public string Description { get; set; } = "";

        [JsonIgnore]
        public List<Ticket> Tickets { get; private set; } = new();

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        [JsonIgnore]
        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [DataType(DataType.Text)]
        public string Note { get; set; } = "";

        [JsonIgnore]
        public int CustomerId { get; set; }

        [JsonIgnore]
        public Party Customer { get; set; } = default!; //Kunde

        [JsonIgnore]
        public List<Document> Documents { get; private set; } = new();

        [JsonIgnore]
        public List<Conversation> Conversations { get; private set; } = new();

        [Required]
        public Address DeliveryAddress { get; set; } = default!;

        [Required]
        public Address BillingAddress { get; set; } = default!;

        public override string ToString() => $"{Id}, {Name}, {Enum.GetName(OrderStatus)}, {Description}, {Note} {DeliveryAddress} {BillingAddress}";

    }
}