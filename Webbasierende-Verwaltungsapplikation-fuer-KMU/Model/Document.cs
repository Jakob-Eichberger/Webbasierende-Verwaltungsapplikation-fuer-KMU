using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for Document.
    /// </summary>
    public class Document
    {

        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        public string GetFullFileName { get { return OriginalName + MimeType; } }

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        [MaxLength(200, ErrorMessage = "File Name cant be more then 200 Characters long!")]
        public string OriginalName { get; set; } = default!;

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        public string BlobFileName { get; set; } = default!;

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        public string BlobContainer { get; set; } = default!;

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        public string MimeType { get; set; } = default!;

        [Required(ErrorMessage = "")]
        public long Size { get; set; } = default!;

        [Required(ErrorMessage = "")]
        public DateTime Created { get; set; }

        [Required(ErrorMessage = "")]
        public DateTime LastUpdated { get; set; }

        public int? PartyId { get; set; }
        public Party? Party { get; set; } = default!;

        public DocumentType? DocumentType { get; set; } = default!;

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public int? TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public int? MessageId { get; set; }
        public Message? Message { get; set; }

    }
}