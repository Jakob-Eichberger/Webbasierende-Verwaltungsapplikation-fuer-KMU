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
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Employee
{
    public class ProfileModel : PageModel
    {
        public AuthService _authService;

        public PartyService _partyService;

        public UserService _userService;

        private AddressService _addressService;

        public Model.Employee Employee { get; set; } = default!;

        public List<Model.Ticket> Tickets { get; set; } = new();

        public List<SelectListItem> Genders { get; set; } = new();

        public bool UserIsOwner { get; set; } = false;

        [BindProperty]
        public string LoginEmail { get; set; } = default!;

        [BindProperty]
        public string Email { get; set; } = default!;

        [BindProperty]
        public PersonDto PersonDto { get; set; } = default!;

        [TempData]
        public string ErrorMessage { get; set; } = null!;
        [TempData]
        public string SuccessMessage { get; set; } = null!;

        public ProfileModel(AuthService authService, PartyService partyService, UserService userService, AddressService addressService)
        {
            _partyService = partyService;
            _authService = authService;
            _userService = userService;
            _addressService = addressService;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (!(_authService.IsEmployee || _authService.IsAdmin)) return RedirectToPage("/404");
            var found = _partyService.GetTable<Model.Employee>().Include(i => i.User).Include(i => i.Person).Include(I => I.Address).Include(i => i.PhoneNumbers).FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            Tickets.Clear();
            Tickets.AddRange(_partyService.GetTable<Model.Ticket>().Include(i => i.Status).Include(i => i.Order).Where(i => i.EmployeePartyId == found.Id && i.Order.OrderStatus == Model.OrderStatus.Open && i.Status.Sequence != 0).OrderByDescending(i => i.Priority).ThenByDescending(i => i.Status.Sequence));
            Employee = found;
            UserIsOwner = found.Id == _authService.PartyId;
            PersonDto = new() { LastName = found?.Person?.LastName ?? "", FirstName = found?.Person?.FirstName ?? "", GebDate = found?.Person?.GebDate ?? DateTime.MinValue, Gender = found?.Person?.Gender ?? 0 };
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateLoginEmail(Guid guid)
        {
            try
            {
                if (!_authService.IsAdmin) return RedirectToPage("/404");
                var found = _userService.GetTable<Model.Employee>().Include(i => i.User).FirstOrDefault(i => i.Guid == guid);

                if (found is null) return RedirectToPage("/404");
                try
                {
                    await _userService.UpdateLoginEmailAsync(LoginEmail, found!.User!.Id);
                }
                catch (ApplicationException ex)
                {
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Employee/Profile", new { guid });
                }
                SuccessMessage = "Die Login Email wurde erfolgreich aktualisiert!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Employee/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostUpdateEmail(Guid guid)
        {
            try
            {
                if (!_authService.IsAdmin) return RedirectToPage("/404");
                var found = _partyService.GetTable<Model.Employee>().Include(i => i.User).FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                try
                {
                    await _partyService.UpdateEmailAsync(Email, found!.User!.Id);
                }
                catch (ApplicationException ex)
                {
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Employee/Profile", new { guid });
                }
                SuccessMessage = "Die Email wurde erfolgreich aktualisiert!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Employee/Profile", new { guid });
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            foreach (var s in Enum.GetValues(typeof(Model.Gender)))
            {
                Genders.Add(new SelectListItem(s?.ToString()?.ToLower(), s?.ToString()));
            }
            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostEditProfile(Guid guid)
        {
            try
            {
                if (!_authService.IsEmployee) return RedirectToPage("/404");
                var found = _partyService.GetTable<Model.Employee>().Include(i => i.User).FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                try
                {
                    await _partyService.UpdateParty(PersonDto, found!.User!.Id);
                }
                catch (ApplicationException ex)
                {
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Employee/Profile", new { guid });
                }
                SuccessMessage = "Das Profile wurde erfolgreich aktualisiert!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Employee/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostDeletePhoneNumberAsync(Guid guid, Guid phoneGuid)
        {
            if (!_authService.IsEmployee) return RedirectToPage("/404");
            var found = _partyService.GetTable<Model.Employee>().Include(i => i.User).FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
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
                return RedirectToPage("/Employee/Profile", new { guid });
            }
            return RedirectToPage("/Employee/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostDeleteAddressAsync(Guid guid, Guid addressGuid)
        {

            var found = _partyService.GetTable<Model.Employee>().FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (!(_authService.IsEmployee)) return RedirectToPage("/404");

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
                return RedirectToPage("/Employee/Profile", new { guid });
            }
            return RedirectToPage("/Employee/Profile", new { guid });
        }
    }
}
