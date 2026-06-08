namespace lang_server.Models
{

    public class CommandeDto
    {
        public int ClientId { get; set; }

        public int ContactId { get; set; }
        public int GuestId { get; set; }
        public int Type { get; set; }
        public int Dos { get; set; }
        public string? Etb { get; set; }   // 🔥 AJOUT
        public string? DEPOT { get; set; }   // 🔥 AJOUT


        public string? Tiers { get; set; }
        public char? CE4 { get; set; }
        public List<ArticleDto>? Articles { get; set; }
        public string? Reglement { get; set; }
    }

}
