using Olympia.Models;
using System.Collections.Generic;

namespace Olympia.Interfaces
{
    public interface ITarRemiseRepository
    {
        ICollection<TarRemise> GetTarRemise();
        TarRemise GetTarRemiseByRef(string reference);
        TarRemise GetTarRemiseByRefByRemcod(string reference, string remcod);


    }
}
