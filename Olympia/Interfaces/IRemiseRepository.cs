using Olympia.Models;
using System.Collections.Generic;

namespace Olympia.Interfaces
{
    public interface IRemiseRepository
    {
        ICollection<Tre> GetRemises();
        Tre GetRemiseByRefByRemcod(string reference, string remcod);
    }
}
