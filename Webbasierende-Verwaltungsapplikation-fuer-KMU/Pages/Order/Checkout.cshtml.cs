using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Order
{
    public class PayModel : PageModel
    {
        public OrderService _orderService;
        public AuthService _authService;

        [TempData]
        public string ErrorMessage { get; set; } = null!;

        [TempData]
        public string SuccessMessage { get; set; } = null!;

        public (List<(decimal ticketTotal, Model.Ticket ticket)> listOfTickets, decimal orderTotal) OrderSummary { get; set; } = new();

        public Model.Order Order { get; set; } = default!;

        public PayModel(OrderService orderService, AuthService authService)
        {
            _orderService = orderService;
            _authService = authService;
        }

        public IActionResult OnGet(Guid guid)
        {
            try
            {
                var found = _orderService.GetTable<Model.Order>().SingleOrDefault(e => e.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                Order = found;
                if (!(_authService.PartyId == found.Id || _authService.IsCustomerOrCompany)) return RedirectToPage("/404");
                if (found.OrderStatus != Model.OrderStatus.AwaitingPayment)
                {
                    ErrorMessage = $"Auftrag mit guid '{guid}' wurde schon bezahlt bzw kann noch nicht bezahlt werden, da jener noch nicht abgeschlossen ist!";
                    return RedirectToPage("/Order/Index", new { guid });
                }
                OrderSummary = _orderService.GetInvoiceSummary(found);
                if (OrderSummary.orderTotal == 0)
                {
                    ErrorMessage = $"Eine Rechnung mit dem Auftragswert '0' kann  nicht bezahlt werden!";
                    return RedirectToPage("/Order/Index", new { guid });
                }
            }
            catch (ArgumentNullException)
            {
                ErrorMessage = $"Es wurden keine oder mehrere Aufträge mit der Guid '{guid}' gefunden. Bitte wenden Sie sich an den Kundendienst oder versuchen Sie es etwas später erneut!";
                return RedirectToPage("/Order/Index", new { guid });
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = "Es ist ein Fehler aufgetreten: " + ex.Message;
                return RedirectToPage("/Order/Index", new { guid });
            }
            return Page();
        }

        public IActionResult OnGetOrderDetails(Guid guid)
        {
            try
            {
                var found = _orderService.GetTable<Model.Order>().SingleOrDefault(e => e.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                return new JsonResult(new { Total = _orderService.GetInvoiceTotal(found) });
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = $"Es ist ein Fehler aufgetreten: '{ex.Message}'";
                return RedirectToPage("/Order/Index", new { guid });
            }
        }

        public IActionResult OnPostCompleted(Guid guid, [FromBody] Model.PayPalResult.Result data)
        {
            _orderService.SetOrderPayed(guid);
            return new NoContentResult();
        }


    }
}
