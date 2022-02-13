using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.register
{
    public class CompanyModel : PageModel
    {
        [BindProperty]
        public Dto.CompanyDto CompanyDto { get; set; } = new();

        [BindProperty]
        public Dto.AddressDto AddressDto { get; set; } = new();

        [BindProperty]
        public Dto.PasswordDto PasswordDto { get; set; } = new();

        [BindProperty]
        public Dto.PhoneNumberDto PhoneNumberDto { get; set; } = new();

        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> PhoneNumberTypes { get; set; } = new();

        public readonly AuthService _authService;
        public readonly CompanyService _companyService;
        public readonly UserService _userService;

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = default!;

        public CompanyModel(AuthService authService, CompanyService companyService, UserService userService)
        {
            _authService = authService;
            _companyService = companyService;
            _userService = userService;
        }

        public void OnGet()
        {
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            Countries.Add(new SelectListItem("Österreich", "Austria", true));
            foreach (var s in Enum.GetValues(typeof(Model.PhoneNumberType)))
                PhoneNumberTypes.Add(new SelectListItem(s.ToString(), s.ToString()));

            return Task.CompletedTask;
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (_authService.IsEmployee)
                {
                    ModelState.Remove(nameof(PasswordDto.First));
                    ModelState.Remove(nameof(PasswordDto.Second));
                }
                if (PasswordDto.First != PasswordDto.Second) throw new ApplicationException("Die Passwörter stimmen nicht überein!");
                if (ModelState.IsValid)
                {
                    ModelState.Clear();

                    CompanyDto.Role = Model.Role.Customer;
                    await _companyService.AddCompanyAsync
                        (
                        companyDto: CompanyDto,
                        address: AddressDto,
                        phoneNumber: PhoneNumberDto,
                        password: PasswordDto.First
                        );
                    if (_authService.IsEmployee)
                    {
                        _userService.ResetPassword(CompanyDto.EMail);
                        SuccessMessage = $"Unternehmen '{CompanyDto.CompanyName}' wurde erfolgreich angelegt!";
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
