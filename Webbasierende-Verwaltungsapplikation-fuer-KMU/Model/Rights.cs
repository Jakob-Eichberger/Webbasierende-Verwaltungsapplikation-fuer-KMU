using System.ComponentModel.DataAnnotations;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{
    public class Rights
    {
        [Required]
        public bool CanCreateNewConversation { get; set; } = true;

        [Required]
        public bool CanUploadFiles { get; set; } = true;
    }
}
