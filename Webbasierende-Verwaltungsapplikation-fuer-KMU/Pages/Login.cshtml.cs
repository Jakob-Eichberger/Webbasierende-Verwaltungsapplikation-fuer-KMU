using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages
{
    public class LoginModel : PageModel
    {
        public readonly AuthService _authService;

        public LoginModel(AuthService service)
        {
            _authService = service;
        }

        [BindProperty]
        [FromQuery]
        public string? ReturnUrl { get; set; }

        [BindProperty]
        public string Email { get; set; } = default!;
        [BindProperty]
        public string Password { get; set; } = default!;

        [TempData]
        public string? ErrorMessage { get; set; } = default!;

        [TempData]
        public string? SuccessMessage { get; set; } = default!;

        public async Task<IActionResult> OnPost()
        {
            try
            {
                bool result = await _authService.TryLoginAsync(Email, Password);
                if (!result)
                {
                    ErrorMessage = "Benutzername oder Passwort ungueltig.";
                    return RedirectToPage("/Login");
                }
                return LocalRedirect(!string.IsNullOrEmpty(ReturnUrl) ? ReturnUrl : "/");
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Login");
            }
        }
    }
}
