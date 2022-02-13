using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.Model
{
    public class UserService : BaseService
    {
        public EMailService _eMailService;

        public UserService(Database db, EMailService eMailService) : base(db)
        {
            _eMailService = eMailService;
        }

        public async Task UpdateLoginEmailAsync(string LoginEmail, int UserId)
        {
            if (string.IsNullOrWhiteSpace(LoginEmail)) throw new ApplicationException("Login Email darf nicht leer sein!");
            if (!LoginEmail.IsEmailValid()) throw new ApplicationException($"Die angegeben Email '{LoginEmail}' ist keine Valide Email Adresse!");
            var user = _db.User.Find(UserId);
            if (user is null) throw new ApplicationException("Der angegebene User existiert nicht mehr");
            if (_db.User.Where(i => i.LoginEMail == LoginEmail && i.Id != UserId).Any() || _db.Party.Where(i => i.EMail == LoginEmail && i.UserId != UserId).Any())
                throw new ApplicationException($"Die Email Adresse '{LoginEmail}'wird schon von einem anderen User verwendet!");
            if (user.LoginEMail == LoginEmail) throw new ApplicationException("Die Login Email konnte nicht geändert werden da sich die neue Login Mail Addresse von der alten Login Mail Addresse nicht unterscheidet.");
            user.LoginEMail = LoginEmail;
            await base.UpdateAsync<User>(user);
            // TODO: Call Mail Service and send an EMail
        }

        public void ResetPassword(string loginEmail)
        {
            var found = base._db.User.SingleOrDefault(i => i.LoginEMail == loginEmail);
            if (found is not null)
            {
                found.PasswordResetToken = HashMethods.GenerateURLSafeSecret();
                found.PasswordResetTokenGeneratedAt = DateTime.UtcNow;
                base.Update<User>(found);
                if (found.Active is true)
                {
                    _eMailService.SendResetPasswordEmail(found);
                }
                else if (found.Active is null)
                {
                    _eMailService.SendAccountActivationLink(found);
                }
            }
        }

        public async Task SetPasswordAsync(string passwordHash, int userId)
        {
            var found = base._db.User.FirstOrDefault(i => i.Id == userId);
            if (found is null || string.IsNullOrWhiteSpace(passwordHash)) throw new ApplicationException("User existiert nicht!");
            found.PasswordResetToken = null;
            found.PasswordResetTokenGeneratedAt = DateTime.MinValue;
            found.PasswordHash = passwordHash;
            if (found.Active is null) found.Active = true;
            await base.UpdateAsync<User>(found);
        }

        public async void DeleteAccount(int partyId)
        {
            try
            {
                var found = base.GetTable<User>().Include(e => e.Party).Single(e => e.Party!.Id == partyId);
                found.Active = false;
                found.PasswordHash = "";
                await base.UpdateAsync<User>(found);
            }
            catch (ArgumentNullException)
            {
                throw new ApplicationException($"Es konnt kein User zu der Id '{partyId}' gefunden werden! ");
            }
        }
    }
}
