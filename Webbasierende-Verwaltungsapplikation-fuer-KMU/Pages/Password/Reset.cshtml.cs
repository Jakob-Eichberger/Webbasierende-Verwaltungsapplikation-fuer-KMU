using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.User
{
    public class PasswordResetModel : PageModel
    {
        [TempData]
        public string ErrorMessage { get; set; } = "";

        [TempData]
        public string? SuccessMessage { get; set; } = default!;

        [BindProperty]
        public string Email { get; set; } = "";

        public UserService _userService;

        public PasswordResetModel(UserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostResetPassword()
        {
            try
            {
                Email = Regex.Replace(Email, @"\s+", "");
                if (string.IsNullOrWhiteSpace(Email) || !Email.IsEmailValid())
                {
                    ErrorMessage = "Email nicht valide!";
                    return RedirectToPage("/Password/Reset");
                }
                _userService.ResetPassword(Email);
                if (String.IsNullOrWhiteSpace(ErrorMessage)) SuccessMessage = "E-Mail Versendet";
                return RedirectToPage("/Login");
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
