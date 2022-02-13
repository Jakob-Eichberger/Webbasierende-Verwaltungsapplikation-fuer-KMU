using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Enum Class for Customer.
    /// </summary>
    public enum Gender { MALE = 1, FEMALE = 2, OTHER = 3 }

    /// <summary>
    /// Model Class for Customer.
    /// </summary>
    public class Customer : Party
    {

        public override string Fullname => (Person?.FirstName ?? "") + " " + (Person?.LastName ?? "");

        public Person? Person { get; set; } = null!;

        [Required(ErrorMessage = "")]
        public bool AcceptedTerms { get; set; }

        public override string ToString() => $"{Fullname}";
    }
}
