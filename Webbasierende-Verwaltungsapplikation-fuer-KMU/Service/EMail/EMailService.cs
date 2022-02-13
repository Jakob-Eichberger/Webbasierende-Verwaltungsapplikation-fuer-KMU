using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service.EMail
{
    public class EMailService
    {
        private readonly BaseService _db;
        private readonly LinkGenerator _linkGenerator;

        public EMailService(BaseService db, LinkGenerator linkGenerator)
        {
            _db = db;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Function sends a E-Mail
        /// </summary>
        /// <param name="from">The E-Mail Adresse from whom the Mail was send.</param>
        /// <param name="to">The E-Mail Adresse to which the Mail was send.</param>
        /// <param name="subject">Subject Text</param>
        /// <param name="htmlContent">Body Text</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Throws a <see cref="ApplicationException"/> if paramter <paramref name="from"/> is null.</exception>
        /// <exception cref="ApplicationException">Throws a <see cref="ApplicationException"/> if paramter <paramref name="to"/> is null.</exception>
        /// <exception cref="ApplicationException">Throws a <see cref="ApplicationException"/> if paramter <paramref name="subject"/> is null or empty.</exception>
        /// <exception cref="ApplicationException">Throws a <see cref="ApplicationException"/> if paramter <paramref name="frohtmlContentm"/> is null or empty.</exception>
        public void SendEmail(EmailAddress to, string subject, string plainTextContent)
        {
            if (to is null) throw new ApplicationException($"Paramter {nameof(to)} or its property Email is null or empty.");
            if (string.IsNullOrWhiteSpace(subject)) throw new ApplicationException($"Paramter {nameof(subject)} is empty.");
            if (string.IsNullOrWhiteSpace(plainTextContent)) throw new ApplicationException($"Paramter {nameof(plainTextContent)} is empty.");
            var email = _db.GetTable<Operator>().FirstOrDefault()?.OfficeEmail ?? throw new ApplicationException("");
            plainTextContent += "\n\n\nDies ist eine automatisch versendete E-Mail. Bitte antworten Sie nicht auf diese Nachricht, da die E-Mail-Adresse nicht zum Empfang von E-Mails eingerichtet ist. ";

            new SmtpClient("SMTP SERVER")
            {
                Credentials = new NetworkCredential("username", "password"),
                EnableSsl = true
            }.Send(email, to.Email, subject, plainTextContent.Replace("<br/>", "\n")); ;
        }

        internal static void SendUpdatedTermsAndConditionsEMail()
        {
        }

        public void SendEmail(User to, string subject, string htmlContent)
        {
            var found = _db.GetTable<User>().Include(e => e.Party).FirstOrDefault(e => e.Id == to.Id);
            if (found is null) throw new ApplicationException($"Account mit der Id '{to.Id}' nicht gefunden!");
            if (found.Active == false) return;
            SendEmail(new EmailAddress(found!.Party!.EMail), subject, htmlContent);
        }

        /// <summary>
        /// Function sends a password reset or password activation link to the User. 
        /// </summary>
        /// <param name="user">The <see cref="User"/> to whom the rest link should be send.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Throws a <see cref="NullReferenceException"/> when the paramter <paramref name="user"/> is null.</exception>
        /// <exception cref="ApplicationException">Throws a <see cref="ApplicationException"/> when no User with was found.</exception>
        public void SendResetPasswordEmail(User user)
        {
            if (user is null) throw new ApplicationException($"Parameter {nameof(user)} can't be null here!");
            if (user.Active == false) return;
            var found = _db.GetTable<User>().Include(i => i.Party).FirstOrDefault(i => i.Id == user.Id);
            if (found is null) throw new ApplicationException($"User existiert nicht!");
            var link = _linkGenerator.GetPathByPage("/Password/Set", null, new { found.Id, Token = user.PasswordResetToken });
            string content = $"Email:{found.LoginEMail}\nReset link: {PageHelper.GetPageURL()}{link}";
            SendEmail(new EmailAddress(found.LoginEMail, found!.Party!.Fullname), "Password reset link.", content);
        }

        public void SendAccountActivationLink(User user)
        {
            if (user is null) throw new ApplicationException($"Parameter {nameof(user)} can't be null here!");
            if (user.Active == false) return;
            var found = _db.GetTable<User>().Include(i => i.Party).FirstOrDefault(i => i.Id == user.Id);
            if (found is null) throw new ApplicationException($"User existiert nicht!");
            var link = _linkGenerator.GetPathByPage("/Password/Set", null, new { found.Id, Token = user.PasswordResetToken });
            string content = $"Sehr geehrte Damen und Herren!\n\nFür die E-Mail Adresse '{found.LoginEMail}' wurde soeben ein Konto angelegt. \n\nFür das aktivieren des Kontos bitte diesen Link klicken: {PageHelper.GetPageURL()}{link}";
            SendEmail(new EmailAddress(found.LoginEMail, found!.Party!.Fullname), "Account Aktivierungslink.", content);
        }
    }
}
