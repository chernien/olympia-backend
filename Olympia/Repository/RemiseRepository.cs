using System.Collections.Generic;
using System.Linq;
using Olympia.Interfaces;
using Olympia.Models;

namespace Olympia.Repositories
{
    public class RemiseRepository : IRemiseRepository
    {
        private readonly ERP210OLYMPIA_FContext _context;

        public RemiseRepository(ERP210OLYMPIA_FContext context)
        {
            _context = context;
        }

        public ICollection<Tre> GetRemises()
        {
            return _context.Tre.Where(a => a.Dos == 2).ToList();
        }

        public Tre GetRemiseByRefByRemcod(string reference, string remcod)
        {
            return _context.Tre.FirstOrDefault(t => t.Ref == reference && t.Remcod == remcod);
        }
    }
}
