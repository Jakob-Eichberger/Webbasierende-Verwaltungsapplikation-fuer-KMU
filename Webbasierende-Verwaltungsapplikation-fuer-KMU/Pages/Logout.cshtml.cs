using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly AuthService _service;

        public LogoutModel(AuthService service)
        {
            _service = service;
        }

        public async Task<IActionResult> OnGet()
        {
            await _service.Logout();
            return LocalRedirect("/");
        }
    }
}
