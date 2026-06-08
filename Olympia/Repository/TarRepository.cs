using Olympia.Interfaces;
using Olympia.Models;

namespace Olympia.Repository
{
    public class TarRepository : ITarRepository
    {
        private readonly ERP210OLYMPIA_FContext _context;
        public TarRepository(ERP210OLYMPIA_FContext context)
        {
            _context = context;
        }
        public Tar GetArtById(int id)
        {
            return _context.Tar.Where(a => a.TarId == id && a.Dos == 2).FirstOrDefault();

        }

        public Tar GetTarByNum(string Reference)
        {
            return _context.Tar.Where(a => a.Ref == Reference && a.Dos == 2).FirstOrDefault();
        }

        public ICollection<Tar> GetTars()
        {
            return _context.Tar.Where(a => a.Dos == 2).ToList();
        }

        public ICollection<Tar> GetTars(string Reference)
        {
            return _context.Tar.Where(t => t.Ref.Trim() == Reference.Trim() && t.Dos == 2).ToList();
        }
       
    }
}
