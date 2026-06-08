using lang_server.Models;

namespace Olympia.Models
{
    public class CLI
    {
        public int CLI_ID { get; set; }
        public string RUE { get; set; }
        public decimal DOS { get; set; }
        public decimal U_RDV { get; set; }
        public string LOC { get; set; }
        public string VIL { get; set; }
        public string PAY { get; set; }
        public string CPOSTAL { get; set; }
        public string REGIONCOD { get; set; }
        public string TEL { get; set; }
        public string FAX { get; set; }
        public string WEB { get; set; }
        public string EMAIL { get; set; }
        public string NOM { get; set; }
        public string TIERS { get; set; }
        public string REPR_0001 { get; set; }
        public string REMCOD { get; set; }
        public List<Commande>? Commandes { get; set; }

    }
}
