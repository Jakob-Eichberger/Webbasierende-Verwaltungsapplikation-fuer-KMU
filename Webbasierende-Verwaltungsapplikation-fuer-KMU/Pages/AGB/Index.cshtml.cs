using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.AGB
{
    public class IndexModel : PageModel
    {
        public readonly BaseService _baseService;

        public string AgbText { get; set; } = default!;

        public IndexModel(BaseService baseService)
        {
            _baseService = baseService;
        }

        public IActionResult OnGet()
        {
            AgbText = _baseService.GetTable<Operator>()?.FirstOrDefault()?.TermsAndConditions ?? "Keine AGBs vorhanden!";
            return Page();
        }
    }
}
