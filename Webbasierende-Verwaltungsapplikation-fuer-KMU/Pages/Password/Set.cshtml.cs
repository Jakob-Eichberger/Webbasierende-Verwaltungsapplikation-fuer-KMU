using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.User
{
    public class SetPasswordModel : PageModel
    {
        public UserService _userService;

        [TempData]
        public string ErrorMessage { get; set; } = "";

        [TempData]
        public string SuccessMessage { get; set; } = "";

        [BindProperty]
        public PasswordDto Password { get; set; } = new();

        public int Id { get; set; } = default;
        public string Token { get; set; } = "";

        public SetPasswordModel(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult OnGet(int Id, string Token)
        {

            var found = _userService.GetTable<Model.User>().FirstOrDefault(i => i.Id == Id && i.PasswordResetToken == (Token ?? ""));
            if (found is null)
            {
                ErrorMessage = "Der Link ist abgelaufen.";
                return RedirectToPage("/Login");
            }
            if (found.Active is false)
            {
                ErrorMessage = "Der Account ist deaktiviert!";
                return RedirectToPage("/Login");
            }
            if (found.PasswordResetTokenGeneratedAt is null) return RedirectToPage("/404");
            if (found.PasswordResetToken is null) return RedirectToPage("/404");
            this.Id = Id;
            this.Token = Token;
            if (found.Active is true)
            {
                if (((TimeSpan)(DateTime.UtcNow - found.PasswordResetTokenGeneratedAt)).TotalMinutes >= 50000000000)
                {
                    ErrorMessage = "Der Link ist abgelaufen.";
                    return RedirectToPage("/Login");
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(int Id, string Token)
        {
            try
            {
                var found = _userService.GetTable<Model.User>().FirstOrDefault(i => i.Id == Id && i.PasswordResetToken == (Token ?? ""));
                if (found is null)
                {
                    ErrorMessage = "Der Link ist abgelaufen.";
                    return RedirectToPage("/Login");
                }
                if (found.Active is false)
                {
                    ErrorMessage = "Der Account ist deaktiviert!";
                    return RedirectToPage("/Login");
                }

                if (found.PasswordResetTokenGeneratedAt is null) return RedirectToPage("/404");
                if (found.PasswordResetToken is null) return RedirectToPage("/404");
                this.Id = Id;
                this.Token = Token;
                if (found.Active is true)
                {
                    if (((TimeSpan)(DateTime.UtcNow - found.PasswordResetTokenGeneratedAt)).TotalMinutes >= 60 )
                    {
                        ErrorMessage = "Der Link ist abgelaufen.";
                        return RedirectToPage("/Login");
                    }
                }
                if (String.IsNullOrWhiteSpace(Password.First) || String.IsNullOrWhiteSpace(Password.Second))
                {
                    ErrorMessage = "Passwort kann nicht leer sein!";
                    return RedirectToPage("/Password/Set", new { Id, Token });
                }
                if (Password.First != Password.Second)
                {
                    ErrorMessage = "Passwörter müssen gleich sein!";
                    return RedirectToPage("/Password/Set", new { Id, Token });
                }
                var hash = HashMethods.HashPassword(found.Secret, Password.First);
                await _userService.SetPasswordAsync(hash, found.Id);
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Password/Set", new { Id, Token });
            }
            SuccessMessage = "Neues Passwort erfolgreich gesetzt.";

            return RedirectToPage("/Login");
        }
    }
}
