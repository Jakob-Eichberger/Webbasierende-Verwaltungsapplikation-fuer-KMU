using System;
using System.ComponentModel.DataAnnotations;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    public class MessageDto
    {

        [Required]
        [MaxLength(5000, ErrorMessage = "Max Length of 5000 reached!")]
        [MinLength(1, ErrorMessage = "Text needs to be atleast 1 character long")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; } = "";
    }
}
