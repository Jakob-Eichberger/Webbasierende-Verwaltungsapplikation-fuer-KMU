using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Person
    {
        public Person()
        {
        }

        public Person(string LastName, string FirstName, Gender Gender, DateTime GebDate)
        {
            this.LastName = LastName;
            this.FirstName = FirstName;
            this.Gender = Gender;
            this.GebDate = GebDate;
        }

        [DataType(DataType.Text)]
        //[Required(ErrorMessage = "")]
        public string LastName { get; set; } = default!;

        [DataType(DataType.Text)]
        //[Required(ErrorMessage = "")]
        public string FirstName { get; set; } = default!;

        public Gender Gender { get; set; } = Gender.OTHER;

        [DataType(DataType.Date)]
        //[Required(ErrorMessage = "Geburtsdatum darf nicht leer sein.")]
        public DateTime GebDate { get; set; } = DateTime.MinValue;

    }
}
