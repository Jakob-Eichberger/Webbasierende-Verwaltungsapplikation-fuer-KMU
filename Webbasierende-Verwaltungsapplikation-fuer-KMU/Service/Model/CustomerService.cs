using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class CustomerService : BaseService
    {
        public readonly EMailService _emailService;

        public CustomerService(Database db, EMailService eMailService) : base(db)
        {
            _emailService = eMailService;
        }

        public async Task AddCustomerAsync(CustomerDto customerDto, AddressDto address, PhoneNumberDto phoneNumber, string password = null)
        {
            if (customerDto.Person is null) throw new ApplicationException("Person darf nicht leer sein. ");
            if (base.GetTable<User>().Where(u => u.LoginEMail == customerDto.EMail).Any() || base.GetTable<Party>().Where(p => p.EMail == customerDto.EMail).Any())
                throw new ApplicationException($"Die E-Mail '{customerDto.EMail}' existiert bereits.");
            string secret = HashMethods.GenerateSecret();
            string? hash = (password is null) ? password : HashMethods.HashPassword(secret, password);
            Customer customer = new Customer()
            {
                AcceptedTerms = customerDto.AcceptedTerms,
                EMail = customerDto.EMail,
                Person = new Person
                (
                    LastName: customerDto.Person.LastName,
                    FirstName: customerDto.Person.FirstName,
                    Gender: customerDto.Person.Gender,
                    GebDate: customerDto.Person.GebDate
                ),
                User = new User()
                {
                    LoginEMail = customerDto.EMail,
                    Secret = secret,
                    RegisteredAt = DateTime.UtcNow,
                    Active = password is null ? null : true,
                    PasswordHash = hash
                },
                Role = customerDto.Role,
                Rights = new Rights() { CanUploadFiles = true, CanCreateNewConversation = true }
            };
            customer.Address.Add(new Address()
            {
                City = address.City,
                Country = address.Country,
                DoorNumber = address.DoorNumber,
                HouseNumber = address.HouseNumber,
                IsPrimary = true,
                StairCase = address.StairCase,
                State = address.State,
                Street = address.Street,
                ZipCode = address.ZipCode
            });
            customer.PhoneNumbers.Add(new PhoneNumber()
            {
                Number = phoneNumber.Number,
                Type = phoneNumber.Type
            });

            await base.AddAsync<Customer>(customer);

            return;
        }
    }
}
