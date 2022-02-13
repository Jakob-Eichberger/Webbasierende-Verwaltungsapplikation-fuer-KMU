using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for Company.
    /// </summary>
    public class Company : Party
    {
        [Required(ErrorMessage = "")]
        [DataType(DataType.Text)]
        public string CompanyName { get; set; } = "";

        public override string Fullname => CompanyName;

        [DataType(DataType.Text)]
        [Required]
        public string UID { get; set; } = default!;

        [Required(ErrorMessage = "")]
        public bool AcceptedTerms { get; set; }
        public override string ToString() => $"{Fullname} {UID}";

    }
}
