using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Admin
{
    public class IndexModel : PageModel
    {
        [TempData]
        public string ErrorMessage { get; set; } = default!;
        [TempData]
        public string SuccessMessage { get; set; } = default!;

        [BindProperty]
        public string OrderId { get; set; } = "";

        private protected BlobService _blob;

        public IndexModel(BlobService blob)
        {
            _blob = blob;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostDeleteAllFiles()
        {
            await _blob.DeleteBlobContainer();
            return Page();
        }



    }
}
