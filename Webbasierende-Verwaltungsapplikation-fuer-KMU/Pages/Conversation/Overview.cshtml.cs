using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Conversation
{
    public class OverviewModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly BaseService _baseService;
        public List<Model.Conversation> Conversation = new();

        [TempData]
        public string ErrorMessage { get; set; } = null!;

        [TempData]
        public string SuccessMessage { get; set; } = null!;

        public OverviewModel(AuthService authService, BaseService baseService)
        {
            _authService = authService;
            _baseService = baseService;
        }

        public IActionResult OnGet()
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            Conversation = _baseService.GetTable<Model.Conversation>().Include(e => e.Messages).Where(e => (e.Messages.OrderByDescending(e => e.MessageSent).First().CreatedBy) == Model.MessageCreatedBy.Customer).ToList();
            return Page();
        }
    }
}
