using Olympia.Models;
using System.Collections.Generic;

namespace Olympia.Interfaces
{
    public interface IGuestRepository
    {
        ICollection<Guest> GetGuests();
        bool UpdateGuests(List<Guest> guests);
        List<Guest> GetGuestsByNum(string num);

        Guest GetGuestById(int id);
        bool CreateGuest(Guest guest);
        bool DeleteGuest(Guest guest);
        bool GuestExistsByCliIdAndCin(int cliId, int cin);
        bool GuestExistsByCliIdAndTel(int cliId, string tel);

        bool GuestExists(int id);
        bool Save();
    }
}
