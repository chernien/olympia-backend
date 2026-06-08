using Olympia.Interfaces;
using Olympia.Models;
using System.Collections.Generic;
using System.Linq;

namespace Olympia.Repository
{
    public class GuestRepository : IGuestRepository
    {
        private readonly ERP210OLYMPIA_FContext _context;

        public GuestRepository(ERP210OLYMPIA_FContext context)
        {
            _context = context;
        }

        public ICollection<Guest> GetGuests()
        {
            return _context.Guest.OrderBy(g => g.Id).ToList();
        }

        public Guest GetGuestById(int id)
        {
            return _context.Guest.FirstOrDefault(g => g.Id == id);
        }

        public bool GuestExistsByCliIdAndCin(int cliId, int cin)
        {
            return _context.Guest.Any(g => g.CliId == cliId && g.Cin == cin);
        }

        public bool GuestExistsByCliIdAndTel(int cliId, string tel)
        {
            return _context.Guest.Any(g => g.CliId == cliId && g.Tel == tel);
        }

        public bool CreateGuest(Guest guest)
        {
            _context.Guest.Add(guest);
            return Save();
        }

        public bool DeleteGuest(Guest guest)
        {
            _context.Guest.Remove(guest);
            return Save();
        }

        public bool GuestExists(int id)
        {
            return _context.Guest.Any(g => g.Id == id);
        }
        public List<Guest> GetGuestsByNum(string num)
        {
            return _context.Guest.Where(g => g.Num == num).ToList();
        }

        public bool UpdateGuests(List<Guest> guests)
        {
            _context.Guest.UpdateRange(guests);
            return _context.SaveChanges() > 0;
        }
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
