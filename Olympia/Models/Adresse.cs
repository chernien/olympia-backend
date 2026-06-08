using lang_server.Models;

namespace Olympia.Models
{
    public class Adresse
    {
        public int Adresse_Id { get; set; }
        public int Commande_Id { get; set; }
        public string? Adresse_Facturation { get; set; }
        public string? Adresse_Livraison { get; set; }

    }
}
