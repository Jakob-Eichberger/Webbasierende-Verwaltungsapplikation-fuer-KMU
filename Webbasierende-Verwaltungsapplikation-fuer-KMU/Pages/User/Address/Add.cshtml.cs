using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.User.Address
{
    public class AddModel : PageModel
    {
        //[FromRoute(Name ="guid")]
        //public Guid UserGuid { get; set; }

        [BindProperty]
        public Dto.AddressDto AddressDto { get; set; } = new Dto.AddressDto();

        [TempData]
        public string ErrorMessage { get; set; } = default!;
        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public readonly AuthService _authService;
        private readonly AddressService _addressService;

        public Guid PartyGuid { get; private set; }

        public string PartyName { get; private set; } = "";

        public AddModel(AuthService authService, AddressService addressService)
        {
            _authService = authService;
            _addressService = addressService;
        }

        public IActionResult OnGet(Guid guid)
        {
            var found = _addressService.GetTable<Model.Party>().FirstOrDefault(i => i.Guid == guid);
            if (!(_authService.IsEmployee || found?.Id == _authService.PartyId)) return RedirectToPage("/404");
            PartyGuid = guid;
            PartyName = found!.Role.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid guid)
        {
            try
            {
                var found = _addressService.GetTable<Model.Party>().FirstOrDefault(i => i.Guid == guid);
                if (found is null) throw new ApplicationException("Der Benutzer existiert nicht");
                if (!(_authService.IsEmployee || found?.Id == _authService.PartyId)) return RedirectToPage("/404");
                if (ModelState.IsValid)
                {
                    await _addressService.AddAsync(AddressDto, found!.Id);
                    SuccessMessage = "Adresse wurde erfolgreich hinzugefügt.";
                    return RedirectToPage($"/{found!.Role}/Profile", new { guid });
                }
                else return Page();
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/User/Address/Add", new { guid });
            }
        }
    }
}
