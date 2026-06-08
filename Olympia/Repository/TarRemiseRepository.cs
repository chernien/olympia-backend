using System.Collections.Generic;
using System.Linq;
using Olympia.Interfaces;
using Olympia.Models;

namespace Olympia.Repositories
{
    public class TarRemiseRepository : ITarRemiseRepository
    {
        private readonly ERP210OLYMPIA_FContext _context;

        public TarRemiseRepository(ERP210OLYMPIA_FContext context)
        {
            _context = context;
        }

        public ICollection<TarRemise> GetTarRemise()
        {
            return _context.AbTarRemise.Where(a => a.Dos == 2).ToList();
        }

        public TarRemise GetTarRemiseByRef(string reference)
        {
            return _context.AbTarRemise.FirstOrDefault(a => a.Ref.Trim() == reference.Trim());
        }
        public TarRemise GetTarRemiseByRefByRemcod(string reference, string remcod)
        {
            return _context.AbTarRemise.Where(a => a.Ref.Trim() == reference.Trim() && a.Remcod.Trim() == remcod.Trim()).FirstOrDefault();
        }
    }
}
