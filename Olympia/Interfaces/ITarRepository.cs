using Olympia.Models;

namespace Olympia.Interfaces
{
    public interface ITarRepository
    {
        Tar GetTarByNum(string Reference);
        ICollection<Tar> GetTars();
        ICollection<Tar> GetTars(string Reference);
        Tar GetArtById(int id);

    }
}
