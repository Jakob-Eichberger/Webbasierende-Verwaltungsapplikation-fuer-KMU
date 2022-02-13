using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure
{
    public class DbAuthProvider : IAuthProvider
    {
        private readonly Database _db;

        public DbAuthProvider(Database db)
        {
            _db = db;
        }

        public Rights? CheckUser(string email, string password, out int? id, out string? fullName, out Role role)
        {
            role = default;
            id = default;
            fullName = default;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return default;

            User? found = _db
                .User
                .Include(e => e!.Party)
                .ThenInclude(i => i!.Rights)
                .SingleOrDefault(u => u.LoginEMail == email);

            if (found is null || (found.PasswordHash != HashMethods.HashPassword(found.Secret, password))) return default!;

            if (found!.Party is null)
            {
                throw new ApplicationException("Login nicht möglich. Bitte kontaktieren Sie den Administrator!");
            }

            role = found.Party.Role;
            fullName = found.Party.Fullname;
            id = found.Party.Id;
            return found.Party.Rights;
        }
    }
}
