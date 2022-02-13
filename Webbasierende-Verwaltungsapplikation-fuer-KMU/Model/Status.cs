using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    /// <summary>
    /// Model Class for Status.
    /// </summary>
    public class Status
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = Guid.NewGuid();

        [MaxLength(50, ErrorMessage = "Name needs to be less then 50 Characters  long!")]
        [MinLength(1, ErrorMessage = "Name needs to be more then 1 Character  long!")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "")]
        public int Sequence { get; set; }


        public override string ToString() => Name;
    }

}