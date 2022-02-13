using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Service
{
    public class AuthService
    {
        private readonly IAuthProvider _authProvider;
        private readonly HttpContext _context;

        /// <summary>
        /// Constructor. This should not be used since ASP initializes it for us. 
        /// </summary>
        /// <param name="contextAccessor"></param>
        /// <param name="authProvider"></param>
        public AuthService(IHttpContextAccessor contextAccessor, IAuthProvider authProvider)
        {
            _authProvider = authProvider;
            _context = contextAccessor.HttpContext ?? throw new ApplicationException("No http context");
        }

        /// <summary>
        /// Trys to check if the <paramref name="email"/> and <paramref name="password"/> belong to a valid User in the database. 
        /// </summary>
        /// <param name="email">The username that the User entered.</param>
        /// <param name="password">The password that the User entered.</param>
        /// <returns>Returns true if the <paramref name="email"/> and the <paramref name="password"/> where found in the Database. Retruns false if the combination of <paramref name="email"/> and <paramref name="password"/> where not found.</returns>
        public async Task<bool> TryLoginAsync(string email, string password)
        {
            var rights = _authProvider.CheckUser(email, password, out int? id, out string? Fullname, out Role role);
            if (rights is null) return false;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Fullname! ),
                new Claim(ClaimTypes.Role, role!.ToString()),
                new Claim(nameof(PartyId), id!.ToString()!),
                new Claim(nameof(Rights), System.Text.Json.JsonSerializer.Serialize(rights))
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties authProperties = new()
            {
                ExpiresUtc = role == Role.Employee ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
            };
            ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
            await _context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            return true;
        }

        /// <summary>
        /// Logs a user out.
        /// </summary>
        /// <returns>Returns a task.</returns>
        public Task Logout() => _context.SignOutAsync();

        /// <summary>
        /// Returns the value of <see cref="Party.Fullname"/> of the currently signed in <see cref="User"/>.
        /// </summary>
        public string Username => _context.User.Identity?.Name ?? "";

        /// <summary>
        /// Returns the Id of a logged in user. Returns null if <see cref="UserIsAuthenticated"/> is false.
        /// </summary>
        public int? PartyId
        {
            get
            {
                var value = _context?.User?.Claims?.FirstOrDefault(e => e.Type == nameof(PartyId))?.Value;
                return Int32.TryParse(value, out int id) ? id : default;
            }
        }

        /// <summary>
        /// Returns true if a User is authenticated. If the user is not authenticated this function returns false.
        /// </summary>
        public bool UserIsAuthenticated => _context?.User?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Checks if the currently signed in User has the Role "<paramref name="role"/>"
        /// </summary>
        public bool HasRole(Role role) => _context.User.IsInRole(role.ToString());

        /// <summary>
        /// Returns the <see cref="Rights"/> <see cref="object"/> of an Authenticated <see cref="User"/>.
        /// </summary>
        public Rights? Rights
        {
            get
            {
                var info = _context.User.Claims.FirstOrDefault(e => e.Type == nameof(Rights))?.Value ?? default!;
                return !string.IsNullOrWhiteSpace(info) ? System.Text.Json.JsonSerializer.Deserialize<Rights>(info) : default;
            }
        }

        /// <summary>
        /// Checks if a <see cref="User"/> has the Role "<see cref="Role.Employee"/>".
        /// </summary>
        /// <returns></returns>
        public bool IsEmployee => HasRole(Role.Employee);

        /// <summary>
        /// Checks if a user has the Role "<see cref="Role.Admin"/>".
        /// </summary>
        /// <returns>Returns true if the User has the Role "<see cref="Role.Admin"/> else it returns false.
        /// </returns>
        public bool IsAdmin => HasRole(Role.Admin);

        /// <summary>
        /// Checks if a User has the Role "<see cref="Role.Company"/>" or the Role "<see cref="Role.Customer"/>".
        /// </summary>
        /// <returns>Returns true if the User has the Role "<see cref="Role.Company"/>" or the Role <see cref="Role.Customer"/> else it returns false</returns>
        public bool IsCustomerOrCompany => HasRole(Role.Company) || HasRole(Role.Customer);

        public bool IsCustomer => HasRole(Role.Customer);

        public bool IsCompany => HasRole(Role.Company);
    }
}
