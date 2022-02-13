using System.ComponentModel.DataAnnotations;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    public abstract class PartyDto
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EMail { get; set; } = "";

        [Required]
        public Role Role { get; set; }
    }
}
