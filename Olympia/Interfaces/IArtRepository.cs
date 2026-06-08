using lang_server.Models;
using Olympia.Models;

namespace Olympia.Interfaces
{
    public interface IArtRepository 
    {
        Art GetArtByNum(string Reference);
        //ICollection<Art> GetArts();
        ICollection<Art> GetArts(int dos);
        Art GetArtById(int id);
        string getDesArticle(int artId);
        public int GetQuantiteCommandeArticle(List<CommandeArticle> commandeArticles, int commandeId, int articleId, string sref1, string sref2);
        public decimal GetPrixCommandeArticle(List<CommandeArticle> commandeArticles, int commandeId, int articleId, string sref1, string sref2);
    }
}
