using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.register
{
    public class KundenregisterModel : PageModel
    {
        [BindProperty]
        public Dto.CustomerDto CustomerDto { get; set; } = new();

        [BindProperty]
        public Dto.PersonDto PersonDto { get; set; } = new();

        [BindProperty]
        public Dto.AddressDto AddressDto { get; set; } = new();

        [BindProperty]
        public Dto.PhoneNumberDto PhoneNumberDto { get; set; } = new();

        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Genders { get; set; } = new();
        public List<SelectListItem> PhoneNumberTypes { get; set; } = new();

        public readonly AuthService _authService;
        public readonly CustomerService _customerService;
        public readonly EMailService _emailService;
        public readonly UserService _userService;

        public KundenregisterModel(AuthService authService, CustomerService customerService, EMailService emailService, UserService userService)
        {
            _authService = authService;
            _customerService = customerService;
            _emailService = emailService;
            _userService = userService;
        }

        [BindProperty]
        public Dto.PasswordDto PasswordDto { get; set; } = new();

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public void OnGet()
        {
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            Countries.Add(new SelectListItem("Österreich", "Österreich", true));
            foreach (var s in Enum.GetValues(typeof(Model.Gender)))
                Genders.Add(new SelectListItem(s.ToString(), s.ToString()));
            foreach (var s in Enum.GetValues(typeof(Model.PhoneNumberType)))
                PhoneNumberTypes.Add(new SelectListItem(s.ToString(), s.ToString()));

            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (_authService.IsEmployee)
                {
                    ModelState.Remove("First");
                    ModelState.Remove("Second");
                }
                if (PasswordDto.First != PasswordDto.Second) throw new ApplicationException("Die Passwörter stimmen nicht überein!");
                if (ModelState.IsValid)
                {
                    CustomerDto.Person = PersonDto;
                    CustomerDto.Role = Model.Role.Customer;
                    await _customerService.AddCustomerAsync
                        (
                        customerDto: CustomerDto,
                        address: AddressDto,
                        phoneNumber: PhoneNumberDto,
                        password: PasswordDto.First
                        );
                    SuccessMessage = $"Kunde {PersonDto.FirstName} {PersonDto.LastName} mit der Email '{CustomerDto.EMail}' erfolgreich hinzugefügt.";
                    if (_authService.IsEmployee)
                    {
                        _userService.ResetPassword(CustomerDto.EMail);
                        SuccessMessage = $"Kunde '{CustomerDto.Person.LastName} {CustomerDto.Person.FirstName}' wurde erfolgreich angelegt!";
                        return RedirectToPage("/Employee/Index");
                    }
                    else
                    {
                        SuccessMessage = $"Ihr Account wurde erfolgreich angelegt!";
                        return RedirectToPage("/Login");
                    }
                }
                else return Page();
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}