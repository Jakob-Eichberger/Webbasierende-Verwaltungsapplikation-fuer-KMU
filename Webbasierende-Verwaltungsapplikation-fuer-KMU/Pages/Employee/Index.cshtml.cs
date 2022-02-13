using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages
{
    public class EmployeeIndexModel : PageModel
    {
        readonly AuthService _authService;
        readonly TicketService _ticketService;

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public List<Model.Ticket> TicketList { get; set; } = new();

        public EmployeeIndexModel(AuthService authService, TicketService ticketService)
        {
            _authService = authService;
            _ticketService = ticketService;
        }

        public IActionResult OnGet()
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            TicketList = _ticketService.GetTable<Model.Ticket>()
                .Include(i => i.Status)
                .Include(i => i.Order)
                .Where(i => i.EmployeePartyId == _authService.PartyId && i.Order.OrderStatus == Model.OrderStatus.Open && i.Status.Sequence != 0)
                .OrderByDescending(i => i.Priority)
                .ThenByDescending(i => i.Status.Sequence)
                .ToList();
            return Page();
        }
    }
}
