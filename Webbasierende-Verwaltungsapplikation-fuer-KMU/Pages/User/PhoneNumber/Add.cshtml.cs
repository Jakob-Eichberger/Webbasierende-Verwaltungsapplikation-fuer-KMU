using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.PhoneNumber
{
    public class AddModel : PageModel
    {
        [BindProperty]
        public Dto.PhoneNumberDto PhoneNumber { get; set; } = new Dto.PhoneNumberDto();

        public List<SelectListItem> PhoneNumberTypes { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public readonly AuthService _authService;
        private readonly PartyService _partyService;

        public AddModel(AuthService authService, PartyService partyService)
        {
            _authService = authService;
            _partyService = partyService;
        }

        public IActionResult OnGet(Guid guid)
        {
            var found = _partyService.GetTable<Party>().FirstOrDefault(e => e.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (!(_authService.IsEmployee || found.Id == _authService.PartyId)) return RedirectToPage("/404");
            return Page();
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            foreach (var s in Enum.GetValues(typeof(Model.PhoneNumberType)))
                PhoneNumberTypes.Add(new SelectListItem(s.ToString(), s.ToString()));

            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync(Guid guid)
        {
            try
            {
                var found = _partyService.GetTable<Party>().FirstOrDefault(e => e.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (!(_authService.IsEmployee || found.Id == _authService.PartyId)) return RedirectToPage("/404");

                if (!PhoneNumber.Number.IsPhoneNumberValid())
                    throw new ApplicationException("Die Telephonnummer ist nicht gültig.");

                if (ModelState.IsValid)
                {
                    string successMessage = "Die Telefonnummer wurde erfolgreich hinzugefügt.";
                    await _partyService.AddPhoneNumberAsync(PhoneNumber, found.Id);
                    switch (found.Role)
                    {
                        case Model.Role.Employee:
                            SuccessMessage = successMessage;
                            return RedirectToPage("/Employee/Profile", new { guid = guid });
                        case Model.Role.Company:
                            SuccessMessage = successMessage;
                            return RedirectToPage("/Company/Profile", new { guid = guid });
                        case Model.Role.Customer:
                            SuccessMessage = successMessage;
                            return RedirectToPage("/Customer/Profile", new { guid = guid });
                    }

                }

            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/User/PhoneNumber/Add", new { guid });
        }
    }
}
