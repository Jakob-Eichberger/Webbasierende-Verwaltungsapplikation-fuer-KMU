using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{


    /// <summary>
    /// Model Class for Customer.
    /// </summary>
    public class CustomerDto : PartyDto
    {
        public PersonDto? Person { get; set; } = null!;

        [Required]
        public bool AcceptedTerms { get; set; }

    }
}
