using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// Model Class for Company.
    /// </summary>
    public class CompanyDto : PartyDto
    {
        [Required]
        [DataType(DataType.Text)]
        public string CompanyName { get; set; } = "";

        [Required]
        [DataType(DataType.Text)]
        public string UID { get; set; } = default!;
    }
}
