using Microsoft.AspNetCore.Mvc.Filters;
using Olympia.Interfaces;
using Olympia.Models;

namespace HelpDesk.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ERP210OLYMPIA_FContext _context;
        public AuthRepository(ERP210OLYMPIA_FContext context)
        {
            _context = context;
        }
        public ICollection<Authentication> GetAuthentications()
        {
            return _context.Authentication.OrderBy(c => c.Id).ToList();
        }
        public Authentication GetAuthentication(int id)
        {
            return _context.Authentication.Where(a => a.Id == id).FirstOrDefault();
        }
        public Authentication GetAuthenticationByUsername(string username)
        {
            return _context.Authentication.Where(a => a.Username == username).FirstOrDefault();
        }
        public bool AuthenticationExist(int id)
        {
            return _context.Authentication.Any(a => a.Id == id);
        }
        public bool Login(string username, string password)
        {
            return _context.Authentication.Any(c => c.Username.Trim() == username.Trim() && c.Password.Trim() == password.Trim() && c.Role=="ROLE_COMM".Trim());
        }
        public bool LoginClient(string username, string password)
        {
            return _context.Authentication.Any(c => c.Username.Trim() == username.Trim() && c.Password.Trim() == password.Trim() && c.Role == "CLIENT" );
        }
        public Authentication LoginContact(string username, string password)
        {
            var user = _context.Authentication
                .FirstOrDefault(c => c.Username.Trim() == username.Trim()
                                  && c.Role == "ROLE_CONTACT");

            if (user == null)
                return null;

            if (user.Password.Trim() != password.Trim())
                return null;

            return user;
        }

        public bool CreateAuthentication(Authentication authentication)
        {
            _context.Add(authentication);
            return Save();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
