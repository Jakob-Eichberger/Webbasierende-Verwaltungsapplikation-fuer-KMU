using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;


namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Company
{
    public class ProfileModel : PageModel
    {
        public AuthService _authService;
        public PartyService _partyService;
        public UserService _userService;
        public CompanyService _companyService;
        private AddressService _addressService;

        public Model.Company Company { get; set; } = default!;

        public bool UserIsOwner { get; set; }

        public List<Model.Ticket> Tickets { get; set; } = new();

        [BindProperty]
        public string Name { get; set; } = default!;

        [BindProperty]
        public string Uid { get; set; } = default!;

        [TempData]
        public string ErrorMessage { get; set; } = null!;

        [TempData]
        public string SuccessMessage { get; set; } = null!;

        [BindProperty]
        public string LoginEmail { get; set; } = default!;

        [BindProperty]
        public string Email { get; set; } = default!;

        [BindProperty]
        public CompanyDto CompanyDto { get; set; } = default!;

        public ProfileModel(AuthService authService, PartyService partyService, UserService userService, CompanyService companyService, AddressService addressService)
        {
            _partyService = partyService;
            _authService = authService;
            _userService = userService;
            _companyService = companyService;
            _addressService = addressService;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (!(_authService.IsCompany || _authService.IsEmployee)) return RedirectToPage("/404");
            var found = _partyService.GetTable<Model.Company>()
                .Include(i => i.User)
                .Include(i => i.Address)
                .Include(i => i.PhoneNumbers)
                .Include(i => i.Conversations)
                .Include(i => i.Orders)
                .FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (_authService.IsCompany && found.Id != _authService.PartyId) return RedirectToPage("/404");
            Tickets.AddRange(_partyService.GetTable<Model.Ticket>().Include(i => i.Status).Include(i => i.Order).Where(i => i.EmployeePartyId == found.Id && i.Order.OrderStatus == Model.OrderStatus.Open).OrderByDescending(i => i.Priority).ThenByDescending(i => i.Status.Sequence));
            Company = found;
            UserIsOwner = found.Id == _authService.PartyId;
            CompanyDto = new() { CompanyName = found?.CompanyName ?? "", UID = found?.UID ?? "" };
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateLoginEmail(Guid guid)
        {
            var found = _userService.GetTable<Model.Company>().FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");

            if (!(_authService.IsEmployee || (_authService.IsCompany && found.Id == _authService.PartyId))) return RedirectToPage("/404");
            try
            {
                await _userService.UpdateLoginEmailAsync(LoginEmail, found!.UserId);
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Company/Profile", new { guid });
            }
            SuccessMessage = "Die Login Email wurde erfolgreich aktualisiert!";
            return RedirectToPage("/Company/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostUpdateEmail(Guid guid)
        {
            var found = _userService.GetTable<Model.Company>().FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (!(_authService.IsEmployee || (_authService.IsCompany && found.Id == _authService.PartyId))) return RedirectToPage("/404");
            try
            {
                await _partyService.UpdateEmailAsync(Email, found!.UserId);
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Company/Profile", new { guid });
            }
            SuccessMessage = "Die Email wurde erfolgreich aktualisiert!";
            return RedirectToPage("/Company/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostEditProfile(Guid guid)
        {
            try
            {
                var found = _partyService.GetTable<Model.Company>().FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (!(_authService.IsEmployee | _authService.PartyId == found.Id)) return RedirectToPage("/404");
                try
                {
                    await _companyService.UpdateCompany(CompanyDto, found!.UserId);
                }
                catch (ApplicationException ex)
                {
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Company/Profile", new { guid });
                }
                SuccessMessage = "Der Name wurde erfolgreich aktualisiert!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Company/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostDeletePhoneNumberAsync(Guid guid, Guid phoneGuid)
        {
            var found = _partyService.GetTable<Model.Company>().FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (!(_authService.IsEmployee || _authService.PartyId == found.Id)) return RedirectToPage("/404");

            var foundPhNr = _partyService.GetTable<Model.PhoneNumber>().FirstOrDefault(i => i.Guid == phoneGuid);
            if (foundPhNr is null) return RedirectToPage("/404");
            try
            {
                await _partyService.DeletePhoneNumberAsync(foundPhNr.Id);
                SuccessMessage = "Die Telefonnummer ist erfolgreich gelöscht!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Company/Profile", new { guid });
            }
            return RedirectToPage("/Company/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostDeleteAddressAsync(Guid guid, Guid addressGuid)
        {

            var found = _partyService.GetTable<Model.Company>().FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (!(_authService.IsEmployee | _authService.PartyId == found.Id)) return RedirectToPage("/404");

            var foundA = _partyService.GetTable<Model.Address>().FirstOrDefault(i => i.Guid == addressGuid);
            if (foundA is null) return RedirectToPage("/404");
            try
            {
                await _addressService.DeleteAsync(foundA.Id);
                SuccessMessage = "Die Adresse ist erfolgreich gelöscht!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return RedirectToPage("/Company/Profile", new { guid });
            }
            return RedirectToPage("/Company/Profile", new { guid });
        }
    }
}