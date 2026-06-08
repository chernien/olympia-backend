using lang_server.Models;
using System.ComponentModel.DataAnnotations;

namespace Olympia.Models
{
    public class T2
    {
        [Key]
        public int T2_ID { get; set; }

        public decimal DOS { get; set; }
        public string TIERS { get; set; }
        public string USERCR { get; set; }
        public string USERMO { get; set; }

        public string CONTACT { get; set; }

        public string NOM { get; set; }


        public string TEL { get; set; }

        public string TELGSM { get; set; }

        public string FAX { get; set; }

        public string EMAIL { get; set; }


        public string QUESTION { get; set; }

        public string NOMABR { get; set; }

        public string LOGIN { get; set; }

        public string WEBPASS { get; set; }
        // 🔽 nouveaux champs
        public string U_DEPOT { get; set; }

        public string U_ETB { get; set; }

    }
}
