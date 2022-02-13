using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{

    /// <summary>
    /// Type of Document.
    /// </summary>
    /// <example>
    /// Rechnung, Auftragsbestätigung, Kosten Voranschlag ...
    /// </example>
    public class DocumentTypeDto
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        [MaxLength(100, ErrorMessage = "DocumentType Name cant be more then 100 Characters long!")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "")]
        public bool CanDelet { get; set; } = true;
    }
}