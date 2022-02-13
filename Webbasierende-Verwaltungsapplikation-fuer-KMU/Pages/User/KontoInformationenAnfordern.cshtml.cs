using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.User
{
    public class KontoInformationenAnfordernModel : PageModel
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public KontoInformationenAnfordernModel(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            var found = _userService.GetTable<Model.User>()
                .Include(e => e.Party)?.ThenInclude(e => e!.Address)
                .Include(e => e.Party)?.ThenInclude(e => e!.PhoneNumbers)
                .Include(e => e.Party)?.ThenInclude(e => e!.Conversations)
                .Include(e => e.Party)?.ThenInclude(e => e!.Orders)
                .FirstOrDefault(e => e!.Party!.Id == _authService.PartyId);
            return File(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(found, Formatting.Indented))), "application/octet-stream", $"Report_{DateTime.Now:d}.json");
        }
    }
}
