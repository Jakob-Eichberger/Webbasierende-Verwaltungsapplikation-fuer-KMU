using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Pages.Employee
{
    public class AddModel : PageModel
    {
        private readonly AuthService _authService;
        private readonly EmployeeService _employeeService;

        [TempData]
        public string ErrorMessage { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = default!;

        [BindProperty]
        public EmployeeDto EmployeeDto { get; set; } = new();

        [BindProperty]
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = default!;

        public AddModel(AuthService authService, EmployeeService employeeService)
        {
            _authService = authService;
            _employeeService = employeeService;
        }

        public IActionResult OnGet()
        {

            if (!_authService.IsAdmin) return RedirectToPage("/404");
            EmployeeDto.Person = new PersonDto();
            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                if (!_authService.IsAdmin) return RedirectToPage("/404");
                if (!EmployeeDto.EMail.IsEmailValid()) throw new ApplicationException($"Die Email '{EmployeeDto.EMail}' ist nicht valide!");
                if (!Phone.IsPhoneNumberValid()) throw new ApplicationException($"Die Nummer '{Phone}' ist nicht valide!");

                if (_employeeService.GetTable<Model.User>().Where(i => i.LoginEMail == EmployeeDto.EMail).Any() || _employeeService.GetTable<Model.Party>().Where(i => i.EMail == EmployeeDto.EMail).Any()) throw new ApplicationException($"Die Email '{EmployeeDto.EMail}' existiert bereits!");

                if (ModelState.IsValid)
                {
                    EmployeeDto.Role = Model.Role.Employee;
                    _employeeService.AddEmployee(EmployeeDto, Phone);
                    SuccessMessage = $"Employee '{EmployeeDto?.Person?.FirstName ?? ""} {EmployeeDto?.Person?.LastName ?? ""}' mit der EMail '{EmployeeDto?.EMail}' wurde erfolgreich hinzugefügt.";
                    return RedirectToPage("/Admin/Index");
                }
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }
    }
}
