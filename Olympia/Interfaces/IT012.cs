using Olympia.Models;

namespace Olympia.Interfaces
{
    public interface IT012
    {
        Tar GetLibByNum(string Fam);
        ICollection<T012> GetLibs();
        ICollection<T012> GetLibs(string Fam);
        T012 GetLibById(int T012Id);
    }
}
