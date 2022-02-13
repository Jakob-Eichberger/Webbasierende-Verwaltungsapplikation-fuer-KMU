using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Dto;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class PartyService : BaseService
    {
        public readonly EMailService _emailService;

        public PartyService(Database db, EMailService emailService) : base(db)
        {
            _emailService = emailService;
        }

        public async Task UpdateEmailAsync(string LoginEmail, int UserId)
        {
            if (string.IsNullOrWhiteSpace(LoginEmail)) throw new ApplicationException("Login Email darf nicht leer sein!");
            if (!LoginEmail.IsEmailValid()) throw new ApplicationException($"Die angegeben Email '{LoginEmail}' ist keine Valide Email Adresse!");
            var party = _db.Party.Find(UserId);
            if (party is null) throw new ApplicationException("Der angegebene User existiert nicht mehr");
            if (_db.User.Where(i => i.LoginEMail == LoginEmail && i.Id != UserId).Any() || _db.Party.Where(i => i.EMail == LoginEmail && i.UserId != UserId).Any())
                throw new ApplicationException($"Die Email Adresse '{LoginEmail}'wird schon von einem anderen User verwendet!");
            if (party.EMail == LoginEmail) throw new ApplicationException("Die Login Email konnte nicht geändert werden da sich die neue Login Mail Addresse von der alten Login Mail Addresse nicht unterscheidet.");
            party.EMail = LoginEmail;
            await base.UpdateAsync<Party>(party);
            //await _emailService.SendEmail(party);
            // TODO: Call Mail Service and send an EMail
        }
        public async Task AddPhoneNumberAsync(PhoneNumberDto phoneNumberDto, int partyId)
        {
            var party = _db.Party.Find(partyId);
            if (party is null) throw new ApplicationException("Der angegebene User existiert nicht mehr.");
            party.PhoneNumbers.Add(new PhoneNumber()
            {
                Number = phoneNumberDto.Number,
                Type = phoneNumberDto.Type
            });
            await base.UpdateAsync<Party>(party);
        }

        public async Task DeletePhoneNumberAsync(int phoneNumberid)
        {
            var phoneNumber = _db.PhoneNumber.Find(phoneNumberid);
            if (phoneNumber is null) throw new ApplicationException("Der angegebene Telefonnummer konnte nicht entfernt werden.");
            await base.DeleteAsync<PhoneNumber>(phoneNumber);
        }

        internal async Task UpdateParty(PersonDto dto, int partyId)
        {
            var found = base._db.Party.SingleOrDefault(i => i.Id == partyId);
            if (found is null) throw new ApplicationException("Die Party existiert nicht mehr!");
            if (!(found is Employee || found is Customer)) throw new ApplicationException("Party hat keine Person zum updaten!");
            if (found is Employee)
            {
                (found as Employee)!.Person = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    GebDate = dto.GebDate,
                    Gender = dto.Gender
                };
                await base.UpdateAsync<Employee>((found as Employee)!);
            }
            if (found is Customer)
            {
                (found as Customer)!.Person = new Person
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    GebDate = dto.GebDate,
                    Gender = dto.Gender
                };
                await base.UpdateAsync<Customer>((found as Customer)!);
            }
        }
    }
}
