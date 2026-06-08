using lang_server.Models;
using Olympia.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lang_server.Models
{
    public class Commande
    {

        [Key]
        public int Commande_Id { get; set; }
        public int ClientId { get; set; }
        public int GuestId { get; set; }
        public int Type { get; set; }

        public CLI Client { get; set; }
        public T2 CONTACT { get; set; }
        public int ContactId { get; set; }
        public int Dos { get; set; }

        [Column("ETB")]
        public string? Etb { get; set; }

        [Column("DEPOT")]
        public string? DEPOT { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal? Total { get; set; }
        public List<CommandeArticle> CommandeArticles { get; set; }
        public string? Status { get; set; }
        public string? Tiers { get; set; }
        public char? CE4 { get; set; }
        public string? Pino { get; set; }
        public string? Reglement { get; set; }
        public Commande()
        {
            CommandeArticles = new List<CommandeArticle>();
        }
       
    }
}
