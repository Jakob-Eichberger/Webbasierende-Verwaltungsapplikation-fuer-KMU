using System;
using System.ComponentModel.DataAnnotations;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class PersonDto
    {
        [DataType(DataType.Text)]
        public string LastName { get; set; } = default!;

        [DataType(DataType.Text)]
        public string FirstName { get; set; } = default!;

        public Gender Gender { get; set; } = Gender.OTHER;

        [DataType(DataType.Date)]
        public DateTime GebDate { get; set; } = DateTime.Now;

    }
}
