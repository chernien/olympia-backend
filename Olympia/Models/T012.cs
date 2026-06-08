

namespace Olympia.Models
{

    public partial class T012
    {
        public int T012Id { get; set; }

        public byte Cebin { get; set; }
        /// <summary>
        /// Dossier
        /// </summary>
        public decimal Dos { get; set; }
        /// <summary>
        /// Numéro table de gtftab
        /// </summary>
        public decimal Tabno { get; set; }
        /// <summary>
        /// Numéro famille article
        /// </summary>
        public decimal Famno { get; set; }
        /// <summary>
        /// Famille statistique article
        /// </summary>
        public string Fam { get; set; }
        /// <summary>
        /// Auteur de la création
        /// </summary>
        public string Usercr { get; set; }
        /// <summary>
        /// Auteur de la modification
        /// </summary>
        public string Usermo { get; set; }
        /// <summary>
        /// Utilisateur date et heure de création
        /// </summary>
        public DateTime? Usercrdh { get; set; }
        /// <summary>
        /// Utilisateur date et heure de modification
        /// </summary>
        public DateTime? Usermodh { get; set; }
        /// <summary>
        /// =2 si note&lt;&gt; 0
        /// </summary>
        public decimal Cenote { get; set; }
        /// <summary>
        /// Numéro de note
        /// </summary>
        public decimal Note { get; set; }
        /// <summary>
        /// Libellé
        /// </summary>
        public string Lib { get; set; }
        /// <summary>
        /// Questionnaire implicite dynamique
        /// </summary>
        public string Question { get; set; }
    }
}