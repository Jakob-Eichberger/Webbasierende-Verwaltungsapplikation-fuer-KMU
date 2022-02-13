using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Search
{
    public class IndexModel : PageModel
    {
        readonly BaseService _baseService;
        readonly AuthService _authService;
        public IndexModel(BaseService baseService, AuthService authService)
        {
            _authService = authService;
            _baseService = baseService;
        }

        [BindProperty]
        public string SearchTerm { get; set; } = "";

        public List<Model.Order> Orders { get; set; } = new();
        public List<Model.Ticket> Tickets { get; set; } = new();
        public List<Model.Party> Parties { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; } = "";

        public IActionResult OnGet()
        {
            if (!(_authService.IsEmployee || _authService.IsAdmin)) return RedirectToPage("/404");
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!(_authService.IsEmployee || _authService.IsAdmin)) return RedirectToPage("/404");
            if (string.IsNullOrWhiteSpace(SearchTerm)) { return Page(); }
            await Task.Run(() =>
             {
                 Parties = _baseService
                 .GetTable<Party>()
                 .Include(e => e.Address)
                 .Include(e => e.PhoneNumbers)
                 .ToList()
                 .Where(e => e.ToString().ToLower().Contains(SearchTerm.ToLower()))
                 .ToList();

                 Orders = _baseService.GetTable<Model.Order>()
                 .Include(e => e.DeliveryAddress)
                 .Include(e => e.BillingAddress)
                 .ToList()
                 .Where(e => e.ToString().ToLower().Contains(SearchTerm.ToLower()))
                 .ToList();

                 Tickets = _baseService.GetTable<Model.Ticket>()
                 .Include(e => e.Tags)
                 .Include(e => e.Status)
                 .ToList()
                 .Where(e => e.ToString().ToLower().Contains(SearchTerm.ToLower()))
                 .ToList();
             });
            ErrorMessage = !Orders.Any() && !Tickets.Any() && !Parties.Any() ? $"Zu dem Suchbegriff '{SearchTerm}' wurden keine Ergebnisse gefunden." : "";
            return Page();
        }

    }
}
