using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.mainpages
{
    public class UserModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly PartyService _partyService;
        public readonly OrderService _orderService;
        public bool CanCreateNewConversation = false;
        public Party Party { get; set; } = default!;

        [TempData]
        public string ErrorMessage { get; set; } = null!;

        [TempData]
        public string SuccessMessage { get; set; } = null!;
        public UserModel(AuthService authService, PartyService partyService, OrderService orderService)
        {
            _orderService = orderService;
            _partyService = partyService;
            _authService = authService;
        }

        public IActionResult OnGet()
        {
            try
            {
                if (!_authService.IsCustomerOrCompany) return RedirectToPage("/404");
                Party = _partyService.GetTable<Party>()?.Include(e => e.Orders)?.Include(e => e.Conversations)?.ThenInclude(e => e.Messages)?.ThenInclude(e => e.SendByParty)?.FirstOrDefault(e => e.Id == _authService.PartyId) ?? throw new ApplicationException("User wurde nicht gefunden.");
                CanCreateNewConversation = _authService?.Rights?.CanCreateNewConversation ?? false;
            }
            catch (ApplicationException ex) { ErrorMessage = ex.Message; }
            return Page();
        }
    }
}
