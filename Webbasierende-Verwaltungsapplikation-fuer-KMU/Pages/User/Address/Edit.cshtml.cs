using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.User.Address
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Dto.AddressDto AddressDto { get; set; } = default!;

        public Guid PartyGuid { get; private set; }

        public string PartyName { get; set; } = "";

        [TempData]
        public string ErrorMessage { get; set; } = default!;
        [TempData]
        public string SuccessMessage { get; set; } = default!;

        private readonly AddressService _addressService;
        private readonly AuthService _authService;

        public EditModel(AddressService addressService, AuthService authService)
        {
            _addressService = addressService;
            _authService = authService;
        }

        public IActionResult OnGet(Guid guid)
        {
            var found = _addressService.GetTable<Model.Address>().Where(a => a.Guid == guid).Include(a => a.Party).FirstOrDefault();
            if (found is null) return RedirectToPage("/404");
            if (!(_authService.IsEmployee || found.PartyId == _authService.PartyId)) return RedirectToPage("/404");
            AddressDto = new Dto.AddressDto()
            {
                City = found.City,
                Country = found.Country,
                DoorNumber = found.DoorNumber ?? "",
                HouseNumber = found.HouseNumber,
                IsPrimary = found.IsPrimary,
                StairCase = found.StairCase ?? "",
                State = found.State,
                Street = found.Street ?? "",
                ZipCode = found.ZipCode
            };
            PartyGuid = found.Party!.Guid;
            PartyName = found.Party!.Role.ToString();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid guid)
        {
            try
            {
                var found = _addressService.GetTable<Model.Address>().Where(a => a.Guid == guid).Include(a => a.Party).FirstOrDefault();
                if (!(_authService.IsEmployee || found?.PartyId == _authService.PartyId)) return RedirectToPage("/404");
                if (ModelState.IsValid)
                {
                    await _addressService.UpdateAsync(dto: AddressDto, guid: guid);
                    SuccessMessage = "Adresse wurde erfolgreich geändert.";
                    return RedirectToPage($"/{found!.Party!.Role}/Profile", new { found.Party.Guid });
                }
                else return Page();
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/User/Address/Edit", new { guid });
            }
        }
    }
}
