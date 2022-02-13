using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Order
{
    public class AddModel : PageModel
    {
        public OrderService _orderService;

        public AuthService _authService;

        [BindProperty]
        public OrderDto OrderDto { get; set; } = default!;

        public List<SelectListItem> BillingAddress { get; set; } = new();
        public List<SelectListItem> DeliveryAddress { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        public AddModel(OrderService orderService, AuthService authService)
        {
            _authService = authService;
            _orderService = orderService;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            var found = _orderService.GetTable<Model.Party>()?.FirstOrDefault(i => i.Guid == guid && (i.Role == Model.Role.Company || i.Role == Model.Role.Customer));
            if (found is null) return RedirectToPage("/404");
            return Page();
        }
        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            if (!_authService.IsEmployee) return Task.CompletedTask;
            if (!Guid.TryParse(RouteData?.Values["guid"]?.ToString() ?? "", out Guid guid)) return Task.CompletedTask;
            var found = _orderService.GetTable<Model.Party>().Include(i => i.Address).FirstOrDefault(e => e.Guid == guid)?.Address ?? default!;
            if (found is null) return Task.CompletedTask;

            BillingAddress = found.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = ($"{c.Country} {c.ZipCode} {c.City} {c.Street} {c?.HouseNumber ?? ""} {(c?.StairCase ?? "")} ")  // Anzeige für den user
            }).ToList();
            DeliveryAddress = found.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = ($"{c.Country} {c.ZipCode} {c.City} {c.Street} {c?.HouseNumber ?? ""} {(c?.StairCase ?? "")} ")  // Anzeige für den user
            }).ToList();

            return Task.CompletedTask;
        }
        public async Task<IActionResult> OnPost()
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            if (ModelState.IsValid)
            {
                Guid guid = Guid.Empty;
                Model.Party? found = default;
                try
                {
                    if (!Guid.TryParse(RouteData?.Values["guid"]?.ToString() ?? "", out guid)) return RedirectToPage("/404");
                    found = _orderService.GetTable<Model.Party>()?.FirstOrDefault(i => i.Guid == guid && (i.Role == Model.Role.Company || i.Role == Model.Role.Customer));
                    if (found is null)
                    {
                        ErrorMessage = "Der User mi der Guid '' wurde nicht gefunden.";
                        return RedirectToPage("/404");
                    };
                    await _orderService.AddAsync(OrderDto, found.Guid);
                }
                catch (ApplicationException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
                if (found?.Role == Model.Role.Customer) return RedirectToPage("/Customer/Profile", new { guid });
                if (found?.Role == Model.Role.Company) return RedirectToPage("/Company/Profile", new { guid });
                return RedirectToPage("/Index");
            }


            return Page();
        }
    }
}
