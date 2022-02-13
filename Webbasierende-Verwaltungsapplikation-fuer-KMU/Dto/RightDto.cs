using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto
{
    public class RightDto
    {
        [Required]
        public bool CanCreateNewConversation { get; set; }

        [Required]
        public bool CanUploadFiles { get; set; }
    }
}
