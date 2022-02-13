using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.messeges
{
    public class NewModel : PageModel
    {
        public List<SelectListItem> Orders { get; set; } = new();

        [BindProperty]
        public MessageDto MessageDto { get; set; } = new();

        [BindProperty]
        public string OrderId { get; set; } = default!;

        [TempData]
        public string ErrorMessage { get; set; } = null!;
        [TempData]
        public string SuccessMessage { get; set; } = null!;

        private AuthService _authService;
        private ConversationService _conversationService;

        public NewModel(AuthService authService, ConversationService conversationService)
        {
            _authService = authService;
            _conversationService = conversationService;
        }

        public IActionResult OnGet()
        {
            if (!_authService.IsCustomerOrCompany) return RedirectToPage("/404");
            return Page();
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            if (!_authService.IsCustomerOrCompany) return Task.CompletedTask;

            foreach (var Order in _conversationService.GetTable<Model.Order>().Where(i => i.CustomerId == _authService.PartyId))
            {
                Orders.Add(new SelectListItem()
                {
                    Text = $"({Order.Id}) {Order.Name}",
                    Value = Order.Id.ToString()
                });
            }
            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (!_authService.IsCustomerOrCompany) return RedirectToPage("/404");
                if (ModelState.IsValid)
                {
                    int? id = int.TryParse(OrderId, out int result) ? result : null;
                    await _conversationService.NewConversation(MessageDto, id);
                    SuccessMessage = "Neue Anfrage erfolgreich gesendet";
                    return RedirectToPage("/Party/Index");
                }
            }
            catch (ApplicationException e)
            {
                ErrorMessage = e.Message;
            }
            return Page();
        }
    }
}


