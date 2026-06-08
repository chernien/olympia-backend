using Olympia.Models;

namespace Olympia.Interfaces
{
    public interface IAuthRepository
    {
        public ICollection<Authentication> GetAuthentications();
        public Authentication GetAuthentication(int id);
        public Authentication GetAuthenticationByUsername(string username);
        public bool AuthenticationExist(int id);
        public bool Login(string username, string password);
        public bool LoginClient(string username, string password);
        public Authentication LoginContact(string username, string password);
        public bool CreateAuthentication(Authentication authentication);
    }
}
