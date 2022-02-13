using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// Model Class for Employee.
    /// </summary>
    public class EmployeeDto : PartyDto
    {

        public PersonDto? Person { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "Max Length of 500 Characters for HourlyRate reached!")]
        [DataType(DataType.Currency)]
        public string HourlyRate { get; set; } = default!;
    }
}
