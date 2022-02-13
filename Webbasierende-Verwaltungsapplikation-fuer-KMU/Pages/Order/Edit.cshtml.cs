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
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Order
{
    public class EditModel : PageModel
    {

        protected readonly OrderService _orderService;
        public List<SelectListItem> BillingAddress { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [BindProperty]
        public OrderDto OrderDto { get; set; } = default!;

        public Guid _guid = default!;
        private readonly AuthService _authService;

        public EditModel(OrderService orderService, AuthService authService)
        {
            _authService = authService;
            _orderService = orderService;
        }

        public IActionResult OnGet(Guid guid)
        {

            if (!_authService.IsEmployee) return RedirectToPage("/404");

            _guid = guid;
            var order = _orderService.GetTable<Model.Order>().Include(i => i.BillingAddress).Include(i => i.DeliveryAddress).SingleOrDefault(g => g.Guid == guid);

            if (order is null) return RedirectToPage("/404");
            if (order.OrderStatus != Model.OrderStatus.Open) return RedirectToPage("/404");

            OrderDto = new OrderDto
            {
                Name = order.Name,
                Description = order.Description,
                BillingAddressId = order.BillingAddress.Id,
                Note = order.Note,
                DeliveryAddressId = order.DeliveryAddress.Id
            };

            return Page();
        }


        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            //int partyId = 1;

            //TODO Load all Parties that have the rolle "Employee"
            // TODO: get Id from User and load thouse addresses
            //var billingAddress = _orderService.GetTable<Address>().Where(x => x.PartyId == partyId).ToList();
            var billingAddress = _orderService.GetTable<Address>().ToList();
            BillingAddress = billingAddress.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = ($"{c.Country} {c.ZipCode} {c.City} {c.Street} {c?.HouseNumber ?? ""} {(c?.StairCase ?? "")} ")  // Anzeige für den user
            }).ToList();

            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPost(Guid guid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");

            _guid = guid;
            if (ModelState.IsValid)
            {
                try
                {
                    await _orderService.UpdateAsync(guid, OrderDto);
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
