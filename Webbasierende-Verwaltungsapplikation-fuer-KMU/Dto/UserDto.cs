using System.ComponentModel.DataAnnotations;
namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{

    /// <summary>
    /// Model Class for User.
    /// </summary>
    public class UserDto
    {
        [Required(ErrorMessage = "Login Email darf nicht leer sein!")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string LoginEMail { get; set; } = "";
    }
}
