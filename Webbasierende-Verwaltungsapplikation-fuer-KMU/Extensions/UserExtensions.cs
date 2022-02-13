using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Class returns a new SendGrid EmailAddress Object.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static SendGrid.Helpers.Mail.EmailAddress GetMailAddress(this User u)
        {
            return new SendGrid.Helpers.Mail.EmailAddress(u.LoginEMail, u.Party?.Fullname ?? "");
        }
    }
}
