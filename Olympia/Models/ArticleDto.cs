using System.ComponentModel.DataAnnotations;

namespace lang_server.Models
{
    public class ArticleDto
    {
        [Key]
        public int ArtId { get; set; }
        public string Des { get; set; }
        public string Ref { get; set; }
        public int Eche { get; set; }

        public string Fam0001 { get; set; }
        public decimal Dos { get; set; }
        public decimal? prix { get; set; }
        public string? Sref1 { get; set; }
        public string? Sref2 { get; set; }
        public int Quantite { get; set; }

    }
}
