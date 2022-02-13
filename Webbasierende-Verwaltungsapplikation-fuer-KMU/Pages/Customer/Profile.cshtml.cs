using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Customer
{
    public class ProfileModel : PageModel
    {

        public readonly AuthService _authService;
        public readonly PartyService _partyService;
        public readonly UserService _userService;
        public readonly AddressService _addressService;


        public Model.Customer Customer { get; set; } = default!;

        [BindProperty]
        public Dto.PersonDto PersonDto { get; set; } = default!;

        public bool UserIsOwner { get; set; }

        [BindProperty]
        public string LoginEmail { get; set; } = default!;

        [BindProperty]
        public string Email { get; set; } = default!;

        [TempData]
        public string ErrorMessage { get; set; } = null!;

        [TempData]
        public string SuccessMessage { get; set; } = null!;

        public List<SelectListItem> Genders { get; set; } = new();


        public ProfileModel(AuthService authService, PartyService partyService, UserService userService, AddressService addressService)
        {
            _authService = authService;
            _partyService = partyService;
            _userService = userService;
            _addressService = addressService;
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            foreach (var s in Enum.GetValues(typeof(Model.Gender)))
            {
                Genders.Add(new SelectListItem(s?.ToString()?.ToLower(), s?.ToString()));
            }
            return Task.CompletedTask;
        }

        public IActionResult OnGet(Guid guid)
        {
            if (!(_authService.IsCustomer || _authService.IsEmployee)) return RedirectToPage("/404");

            var found = _partyService.GetTable<Model.Customer>()
                .Include(c => c.Address)
                .Include(c => c.Person)
                .Include(c => c.User)
                .Include(c => c.Orders)
                .Include(c => c.PhoneNumbers)
                .Include(c => c.Conversations)
                .FirstOrDefault(i => i.Guid == guid);

            if (found is null) return RedirectToPage("/404");
            if (_authService.IsCustomer && found.Id != _authService.PartyId) return RedirectToPage("/404");

            Customer = found;

            PersonDto = new Dto.PersonDto
            {
                FirstName = Customer?.Person?.FirstName!,
                LastName = Customer?.Person?.LastName!,
                GebDate = (DateTime)(Customer?.Person?.GebDate!),
                Gender = Customer.Person.Gender
            };

            UserIsOwner = found.Id == _authService.PartyId;

            Genders.ForEach(g => g.Selected = g.Text == Customer.Person.Gender.ToString());

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateLoginEmail(Guid guid)
        {
            try
            {
                var found = _userService.GetTable<Model.Employee>().Include(i => i.User).FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (!(_authService.IsEmployee | _authService.PartyId == found.Id)) return RedirectToPage("/404");
                try
                {
                    await _userService.UpdateLoginEmailAsync(LoginEmail, found!.User!.Id);
                }
                catch (ApplicationException ex)
                {
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Customer/Profile", new { guid });
                }
                SuccessMessage = "Die Login Email wurde erfolgreich aktualisiert!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Customer/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostUpdateEmail(Guid guid)
        {
            try
            {
                var found = _partyService.GetTable<Model.Employee>().Include(i => i.User).FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (!(_authService.IsEmployee | _authService.PartyId == found.Id)) return RedirectToPage("/404");
                try
                {
                    await _partyService.UpdateEmailAsync(Email, found!.User!.Id);
                }
                catch (ApplicationException ex)
                {
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Customer/Profile", new { guid });
                }
                SuccessMessage = "Die Email wurde erfolgreich aktualisiert!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Customer/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostEditProfile(Guid guid)
        {
            try
            {
                var found = _partyService.GetTable<Model.Customer>().FirstOrDefault(i => i.Guid == guid);
                if (found is null) return RedirectToPage("/404");
                if (!(_authService.IsEmployee || _authService.PartyId == found.Id)) return RedirectToPage("/404");
                try
                {
                    await _partyService.UpdateParty(PersonDto, found!.UserId);
                }
                catch (ApplicationException ex)
                {
                    ErrorMessage = ex.Message;
                    return RedirectToPage("/Customer/Profile", new { guid });
                }
                SuccessMessage = "Das Profile wurde erfolgreich aktualisiert!";
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return RedirectToPage("/Customer/Profile", new { guid });
        }

        public async Task<IActionResult> OnPostDeletePhoneNumberAsync(Guid guid, Guid phoneGuid)
        {
            var found = _partyService.GetTable<Model.Customer>().FirstOrDefault(i => i.Guid == guid);
            if (found is null) return RedirectToPage("/404");
            if (!(_authService.IsEmployee | _authService.PartyId == found.Id)) return RedirectToPage("/404");

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
                return RedirectToPage("/Customer/Profile", new { guid });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAddressAsync(Guid guid, Guid addressGuid)
        {

            var found = _partyService.GetTable<Model.Customer>().FirstOrDefault(i => i.Guid == guid);
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
                return RedirectToPage("/Customer/Profile", new { guid });
            }
            return RedirectToPage("/Customer/Profile", new { guid });
        }
    }
}
