using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages
{
    public class DevelopmentModel : PageModel
    {
        readonly PartyService _partyService;
        public List<Model.Party> userList = new();
        readonly AuthService _authService;
        readonly TicketService _ticketService;
        readonly OrderService _orderService;
        readonly EMailService _emailService;

        public DevelopmentModel(PartyService partyService, AuthService authService, TicketService ticketService, OrderService orderService, EMailService emailService)
        {
            _ticketService = ticketService;
            _partyService = partyService;
            _authService = authService;
            _orderService = orderService;
            this._emailService = emailService;
        }

        [BindProperty]
        public string OrderId { get; set; } = "";
        [BindProperty]
        public string Mail { get; set; } = "";

        public IActionResult OnGet()
        {
            userList = _partyService.GetTable<Party>().Include(e => e.User).OrderByDescending(e => e.Role).ToList();
            return Page();
        }

        public IActionResult OnPostSignIn(string email, string passwort)
        {
            _authService?.Logout();
            _authService?.TryLoginAsync(email!, passwort!);
            return RedirectToPage("/Development");
        }

        public async Task<IActionResult> OnPostSetAllTicketsToClosed()
        {


            if (int.TryParse(OrderId, out int id))
            {
                var tickets = _ticketService.GetTable<Model.Ticket>().Include(i => i.Status).Where(i => i.OrderId == id).ToList();
                foreach (var item in tickets)
                {
                    item.Status = _ticketService.GetTable<Status>().SingleOrDefault(i => i.Sequence == 0)!;
                    await _ticketService.Update(item);
                }
            }
            else
            {
                throw new ApplicationException("ned a int");
            }
            return Page();
        }


        public IActionResult OnPostSetOrderClosed()
        {
            if (int.TryParse(OrderId, out int id))
            {
                var order = _ticketService.GetTable<Model.Order>().FirstOrDefault(e => e.Id == id);
                if (order is not null)
                {
                    _orderService.SetOrderPayed(order.Guid);
                }
            }
            else
            {
                throw new ApplicationException("ned a int");
            }
            return Page();
        }

        public IActionResult OnPostSendEMail()
        {
            _emailService.SendEmail(new SendGrid.Helpers.Mail.EmailAddress(Mail), "Test", "this  is a test");
            return Page();
        }
    }
}
