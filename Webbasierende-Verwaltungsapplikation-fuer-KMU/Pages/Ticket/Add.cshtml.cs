using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Ticket
{
    public class AddModel : PageModel
    {
        private readonly TicketService _ticketService;

        private readonly AuthService _authService;
        [BindProperty]
        public Dto.TicketDto TicketDto { get; set; } = default!;

        public List<SelectListItem> Statuss { get; set; } = new();

        public List<SelectListItem> Employees { get; set; } = new();

        public List<SelectListItem> Prioritys { get; set; } = new();

        [FromRoute]
        public Guid guid { get; set; } = Guid.Empty;


        public AddModel(TicketService ticketService, AuthService authService)
        {
            _ticketService = ticketService;
            _authService = authService;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            var found = _ticketService.GetTable<Model.Order>().FirstOrDefault(e => e.Guid == guid);
            if (found is null || found.OrderStatus != Model.OrderStatus.Open) return RedirectToPage("/404");
            return Page();
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            if (!_authService.IsEmployee) return Task.CompletedTask;
            Statuss.AddRange(_ticketService.GetTable<Status>().OrderByDescending(i => i.Sequence).Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name }));
            Employees.AddRange(_ticketService.GetTable<Model.Employee>().Where(i => i.Role == Role.Employee).Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Fullname }));
            foreach (int i in Enum.GetValues(typeof(Model.Priority)))
            {
                Prioritys.Add(new SelectListItem() { Value = i.ToString(), Text = Enum.GetName(typeof(Model.Priority), i) });
            }
            return Task.CompletedTask;
        }
        public async Task<IActionResult> OnPost(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");

            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketService.Add(TicketDto, guid);
                    return RedirectToPage("/Order/Index", new { guid });
                }
                catch (ApplicationException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return Page();
        }
    }
}
