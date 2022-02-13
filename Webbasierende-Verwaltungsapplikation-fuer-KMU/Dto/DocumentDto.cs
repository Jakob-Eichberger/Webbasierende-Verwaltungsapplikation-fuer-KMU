using System;
using System.ComponentModel.DataAnnotations;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// Model Class for Document.
    /// </summary>
    public class DocumentDto
    {

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

        [Required(ErrorMessage = "")]
        public DocumentType DocumentType { get; set; } = default!;

    }
}