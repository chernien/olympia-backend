using lang_server.Models;
using Olympia.Interfaces;
using Olympia.Models;

namespace Olympia.Repository
{
    public class ArtRepository : IArtRepository

    {
        private readonly ERP210OLYMPIA_FContext _context;
        public ArtRepository(ERP210OLYMPIA_FContext context)
        {
            _context = context;
        }
        public Art GetArtById(int id)
        {
            return _context.Art.Where(a => a.ArtId == id && a.Dos == 2).FirstOrDefault();
        }

        public Art GetArtByNum(string Reference)
        {
            return _context.Art.Where(a => a.Ref == Reference && a.Dos == 2).FirstOrDefault();

        }

        //public ICollection<Art> GetArts()
        //{
        //    return _context.Art.Where(a => a.Dos == 2 && a.U_ACTIF == 2).ToList();

        //}

        public ICollection<Art> GetArts(int dos)
        {
            return _context.Art
                .Where(a => a.Dos == dos && a.U_ACTIF == 2)
                .ToList();
        }
        public int GetQuantiteCommandeArticle(List<CommandeArticle> commandeArticles, int commandeId, int articleId, string sref1, string sref2)
        {
            var quantite = commandeArticles
            .Where(ca => ca.CommandeId == commandeId && ca.ArticleId == articleId && ca.Sref1.Trim() == sref1.Trim() && ca.Sref2.Trim() == sref2.Trim())
            .Select(ca => ca.Quantite)
            .SingleOrDefault();
            return quantite;
        }

        public decimal GetPrixCommandeArticle(List<CommandeArticle> commandeArticles, int commandeId, int articleId, string sref1, string sref2)
        {
            var quantite = commandeArticles
            .Where(ca => ca.CommandeId == commandeId && ca.ArticleId == articleId && ca.Sref1.Trim() == sref1.Trim() && ca.Sref2.Trim() == sref2.Trim())
            .Select(ca => ca.Quantite)
            .SingleOrDefault();

            var total = _context.Commande
            .Where(ca => ca.Commande_Id == commandeId)
            .Select(ca => (decimal)ca.Total)
            .SingleOrDefault();
            if (quantite == 0)
            {
                quantite = 1;
            }
            var prix = (total / quantite);
            return prix;
        }

        public string getDesArticle(int artId)
        {
            var article = _context.Art.Where(a => a.ArtId == artId).FirstOrDefault();
            return article.Des;
        }
    }
}
