using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Ticket
{
    public class EditTicketModel : PageModel
    {
        public Model.Ticket Ticket { get; set; } = default!;

        [BindProperty]
        public Dto.TicketDto TicketDto { get; set; } = default!;

        public List<SelectListItem> Statuss { get; set; } = new();

        public List<SelectListItem> Employees { get; set; } = new();

        public List<SelectListItem> Prioritys { get; set; } = new();

        private readonly TicketService _ticketService;

        private readonly AuthService _authService;

        public EditTicketModel(TicketService ticketService, AuthService authService)
        {
            if (ticketService is null) throw new ApplicationException("");
            _ticketService = ticketService;
            _authService = authService;
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            Statuss.AddRange(_ticketService.GetTable<Status>().Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name }));
            Employees.AddRange(_ticketService.GetTable<Model.Employee>().Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Fullname }));
            foreach (int i in Enum.GetValues(typeof(Model.Priority)))
            {
                Prioritys.Add(new SelectListItem() { Value = i.ToString(), Text = Enum.GetName(typeof(Model.Priority), i) });
            }
            return Task.CompletedTask;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");

            Ticket = _ticketService.GetTable<Model.Ticket>().Include(e => e.Order).SingleOrDefault(t => t.Guid == guid)!;

            if (Ticket is null) return RedirectToPage("/404");
            if (Ticket.Order.OrderStatus != OrderStatus.Open) return RedirectToPage("/404");

            TicketDto = new Dto.TicketDto(
                name: Ticket.Name,
                description: Ticket.Description,
                created: Ticket.Created,
                priority: Ticket.Priority,
                note: Ticket.Note,
                employeePartyId: Ticket.EmployeePartyId,
                statusId: Ticket.StatusId
                );
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketService.Update(guid, TicketDto);
                    return RedirectToPage("/Ticket/Index", new { guid });
                }
                catch (ApplicationException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return OnGet(guid);
        }
    }
}

