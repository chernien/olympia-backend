using Olympia.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace lang_server.Models
{
    public class CommandeArticle
    {
        public int CommandeArticle_Id { get; set; }
        public int CommandeId { get; set; }
        public int? Dos { get;set; }

        [Column("ETB")]
        public string? Etb { get; set; }

        [NotMapped]
        public Commande Commande { get; set; }
        public int ArticleId { get; set; }
        [NotMapped]
        public virtual Art Article { get; set; }
        public int Quantite { get; set; }
        public int Fquantite { get; set; }
        public int Eche { get; set; }

        public int Stock { get; set; }
        public int ContactId { get; set; }

        public decimal Fidelity { get; set; }

        public string? Ref { get; set; }
        public string? Sref1 { get; set; }
        public string? Sref2 { get; set; }
        public string? Tarif { get; set; }

        public decimal? PRIX { get; set; }
        public string? CE4 { get; set; }
        public decimal U_PTS { get; set; }

    }
}
