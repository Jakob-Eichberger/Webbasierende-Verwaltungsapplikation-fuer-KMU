using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Element
{
    public class AddModel : PageModel
    {
        public ElementService _elementService;
        public AuthService _authService;

        [TempData]
        public string ErrorMessage { get; set; } = default!;
        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public Guid TicketGuid { get; set; } = default!;

        public AddModel(ElementService elementService, AuthService authService)
        {
            _elementService = elementService;
            _authService = authService;
        }

        [BindProperty]
        public Dto.ElementDto ElementDto { get; set; } = new();

        public IActionResult OnGet(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            var found = _elementService.GetTable<Model.Ticket>().Include(e => e.Order).FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (found.Order.OrderStatus != Model.OrderStatus.Open) RedirectToPage("/404");
            TicketGuid = guid;

            return Page();
        }

        public async Task<IActionResult> OnPost(Guid guid)
        {
            try
            {
                if (string.IsNullOrEmpty(ElementDto.Ammount) || ElementDto.Ammount == "0") throw new ApplicationException("Die Anzahl der Elementen darf nicht 0 sein !");
                if (!_authService.IsEmployee) return RedirectToPage("/404");
                var found = _elementService.GetTable<Model.Ticket>().Include(e => e.Order).FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (found.Order.OrderStatus != Model.OrderStatus.Open) RedirectToPage("/404");
                TicketGuid = guid;

                if (ModelState.IsValid)
                {
                    await _elementService.AddAsync(ElementDto, guid);
                    SuccessMessage = "Element erfolgreich hinzugefügt!";

                    return RedirectToPage("/Ticket/Index", new { guid });
                }
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
