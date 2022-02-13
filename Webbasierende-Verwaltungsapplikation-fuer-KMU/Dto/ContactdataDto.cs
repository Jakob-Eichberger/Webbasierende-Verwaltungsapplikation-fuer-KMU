using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    public class ContactdataDto
    {
        [Required(ErrorMessage = "")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EMail { get; set; } = "";
    }
}
