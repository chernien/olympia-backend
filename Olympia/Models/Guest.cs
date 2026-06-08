using System.ComponentModel.DataAnnotations.Schema;
namespace Olympia.Models
{
    public class Guest
    {
        public int? Id { get; set; }
        public int CliId { get; set; }
        public string Num { get; set; }
        public int Cin { get; set; }
        public string Diva { get; set; }
        public string Nom { get; set; } 
        public string Prenom { get; set; }
        public string Tel { get; set; }
        public string Adresse { get; set; }
        public decimal Fidelite { get; set; }

        [NotMapped]
        public int? Dos { get; set; }
    }
}
