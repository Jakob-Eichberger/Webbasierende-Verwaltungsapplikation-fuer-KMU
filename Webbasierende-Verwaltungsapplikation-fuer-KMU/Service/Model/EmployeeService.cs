using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class EmployeeService : BaseService
    {
        private UserService _userService;

        public EmployeeService(Database db, UserService userService) : base(db)
        {
            _userService = userService;
        }

        public void AddEmployee(EmployeeDto employeeDto, string Phone)
        {
            if (employeeDto.Person is null) throw new ApplicationException("Person darf nicht leer sein.");
            if (base.GetTable<User>().Where(i => i.LoginEMail == employeeDto.EMail).Any() || base.GetTable<Party>().Where(i => i.EMail == employeeDto.EMail).Any()) throw new ApplicationException("Die E-Mail existiert schon.");
            string secret = HashMethods.GenerateSecret();
            var employee = new Employee()
            {
                EMail = !employeeDto.EMail.IsEmailValid() ? throw new ApplicationException("EMail ist keine valide EMail.") : employeeDto.EMail,
                Person = new Person()
                {
                    FirstName = !string.IsNullOrWhiteSpace(employeeDto.Person.FirstName) ? employeeDto.Person.FirstName : throw new ApplicationException("Vorname darf nicht leer sein."),
                    LastName = !string.IsNullOrWhiteSpace(employeeDto.Person.LastName) ? employeeDto.Person.LastName : throw new ApplicationException("Nachname darf nicht leer sein."),
                    GebDate = employeeDto.Person.GebDate,
                    Gender = employeeDto.Person.Gender
                },
                Role = employeeDto.Role,
                HourlyRate = employeeDto.HourlyRate,
                User = new User()
                {
                    LoginEMail = employeeDto.EMail,
                    Secret = secret,
                    RegisteredAt = DateTime.UtcNow,
                    Active = null
                }
            };

            employee.PhoneNumbers.Add(new PhoneNumber()
            {
                Number = Phone.IsPhoneNumberValid() ? Phone : throw new ApplicationException("Telefonnummer darf nicht leer sein."),
                Type = PhoneNumberType.Mobile
            });

            base.Add<Employee>(employee);
            _userService.ResetPassword(employeeDto.EMail);
            return;
        }
    }
}
