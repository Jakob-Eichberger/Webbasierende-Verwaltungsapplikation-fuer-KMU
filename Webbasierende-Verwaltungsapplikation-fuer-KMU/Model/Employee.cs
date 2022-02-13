using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for Employee.
    /// </summary>
    public class Employee : Party
    {

        public Person? Person { get; set; } = null!;

        public override string Fullname => (Person?.FirstName ?? "") + " " + (Person?.LastName ?? "");

        [MaxLength(100, ErrorMessage = "Max Length of 100 Characters for HourlyRate reached!")]
        [DataType(DataType.Currency)]
        public string HourlyRate { get; set; } = default!;
    }
}
