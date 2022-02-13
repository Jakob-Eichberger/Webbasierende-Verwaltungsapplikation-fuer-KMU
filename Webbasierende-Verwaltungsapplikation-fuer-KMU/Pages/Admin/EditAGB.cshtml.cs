using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Admin
{
    public class EditAGBModel : PageModel
    {
        private readonly OperatorService _operatorService;
        private readonly AuthService _authService;
        private readonly EMailService _eMailService;

        [BindProperty]
        public string TermsAndConditions { get; set; } = "";

        [TempData]
        public string ErrorMessage { get; set; } = default!;
        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public EditAGBModel(OperatorService operatorService, AuthService authService, EMailService eMailService)
        {
            _operatorService = operatorService;
            _authService = authService;
            _eMailService = eMailService;
        }

        public IActionResult OnGet()
        {
            try
            {
                if (!_authService.IsAdmin) return RedirectToPage("/404");
                TermsAndConditions = _operatorService.GetTable<Operator>().FirstOrDefault()?.TermsAndConditions ?? "";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (!_authService.IsAdmin) return RedirectToPage("/404");
                if (string.IsNullOrWhiteSpace(TermsAndConditions)) throw new ApplicationException("AGB's dürfen nicht leer sein!");
                var found = _operatorService.GetTable<Operator>().FirstOrDefault();
                if (found is null) throw new ApplicationException("Es wurde kein Operator object gefunden. Wenden Sie sich bitte an den Betreiber!");
                found.TermsAndConditions = TermsAndConditions;
                await _operatorService.UpdateAsync(found);
                EMailService.SendUpdatedTermsAndConditionsEMail();
                SuccessMessage = "AGB's wurden geändert.";
                return RedirectToPage("/Admin/Index");
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Admin/EditAGB");
            }
        }
    }
}
