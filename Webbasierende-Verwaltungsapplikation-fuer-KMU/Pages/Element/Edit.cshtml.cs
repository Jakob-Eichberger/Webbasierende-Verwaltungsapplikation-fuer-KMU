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
    public class EditModel : PageModel
    {
        public ElementService _elementService;
        public AuthService _authService;

        [TempData]
        public string ErrorMessage { get; set; } = default!;
        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public Guid ElementGuid { get; set; } = default!;

        public EditModel(ElementService elementService, AuthService authService)
        {
            _elementService = elementService;
            _authService = authService;
        }

        [BindProperty]
        public Dto.ElementDto ElementDto { get; set; } = new();

        public IActionResult OnGet(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            var found = _elementService.GetTable<Model.Element>().Include(e => e.Ticket).ThenInclude(e => e.Order).FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (found.Ticket.Order.OrderStatus != Model.OrderStatus.Open) RedirectToPage("/404");
            ElementGuid = guid;

            ElementDto.Description = found.Description;
            ElementDto.Ammount = found.Ammount;
            ElementDto.Price = found.PricePerItem;

            return Page();
        }

        public async Task<IActionResult> OnPost(Guid guid)
        {
            try
            {
                if (string.IsNullOrEmpty(ElementDto.Ammount) || ElementDto.Ammount == "0") throw new ApplicationException("Die Anzahl der Elementen darf nicht 0 sein !");
                if (!_authService.IsEmployee) return RedirectToPage("/404");
                var found = _elementService.GetTable<Model.Element>().Include(i => i.Ticket).ThenInclude(e => e.Order).FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (found.Ticket.Order.OrderStatus != Model.OrderStatus.Open) RedirectToPage("/404");
                ElementGuid = guid;

                if (ModelState.IsValid)
                {
                    await _elementService.UpdateAsync(ElementDto, guid);
                    SuccessMessage = "Element erfolgreich aktualisiert!";

                    return RedirectToPage("/Ticket/Index", new { guid = found.Ticket.Guid });
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
