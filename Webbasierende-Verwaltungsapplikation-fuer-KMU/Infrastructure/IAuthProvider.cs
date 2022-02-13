using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure
{
    public interface IAuthProvider
    {
        public Rights? CheckUser(string username, string password, out int? Id, out string? FullName, out Role role);
    }
}
