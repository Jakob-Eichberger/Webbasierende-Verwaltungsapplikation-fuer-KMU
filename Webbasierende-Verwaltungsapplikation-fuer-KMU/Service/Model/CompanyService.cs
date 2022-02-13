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
    public class CompanyService : BaseService
    {
        public readonly EMailService _emailService;

        public CompanyService(Database db, EMailService eMailService) : base(db)
        {
            _emailService = eMailService;
        }


        public async Task AddCompanyAsync(CompanyDto companyDto, AddressDto address, PhoneNumberDto phoneNumber, string? password = null)
        {
            if (companyDto is null) throw new ApplicationException("Company darf nicht leer sein.");
            if (base.GetTable<User>().Where(u => u.LoginEMail == companyDto.EMail).Any() || base.GetTable<Party>().Where(p => p.EMail == companyDto.EMail).Any())
                throw new ApplicationException($"Die E-Mail '{companyDto.EMail}' existiert bereits.");
            string secret = HashMethods.GenerateSecret();
            string? hash = (password is null) ? password : HashMethods.HashPassword(secret, password);
            Company company = new Company()
            {
                EMail = companyDto.EMail,

                User = new User()
                {
                    LoginEMail = companyDto.EMail,
                    Secret = secret,
                    RegisteredAt = DateTime.UtcNow,
                    Active = password is null ? null : true,
                    PasswordHash = hash
                },
                Role = companyDto.Role,
                Rights = new Rights() { CanUploadFiles = true, CanCreateNewConversation = true }
            };
            company.Address.Add(new Address()
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
            company.PhoneNumbers.Add(new PhoneNumber()
            {
                Number = phoneNumber.Number,
                Type = phoneNumber.Type
            });

            await base.AddAsync<Company>(company);

            return;
        }

        public async Task UpdateCompany(CompanyDto companyDto, int partyId)
        {
            var found = base._db.Company.SingleOrDefault(i => i.Id == partyId);
            if (found is null) throw new ApplicationException("Die Party existiert nicht mehr!");

            found.CompanyName = companyDto.CompanyName;
            found.UID = companyDto.UID;

            await base.UpdateAsync<Company>((found));
        }
    }
}
