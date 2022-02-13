using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.User
{
    public class AccountLoeschenModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public AccountLoeschenModel(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        public IActionResult OnGet()
        {
            if (!_authService.IsCustomerOrCompany) return RedirectToPage("/404");
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                if (!_authService.IsCustomerOrCompany) return RedirectToPage("/404");
                _userService.DeleteAccount(_authService.PartyId ?? -1);
                _authService.Logout();
                SuccessMessage = "Ihr Account wurde erfolgreich gelöscht.";
                return RedirectToPage("/Login");
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/User/AccountLoeschen");
            }
        }
    }
}
