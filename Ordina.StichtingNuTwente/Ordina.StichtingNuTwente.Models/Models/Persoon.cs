using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Persoon :BaseEntity
    {
        public Persoon()
        {
            Naam = "";
            GeboorteDatum = "";
            Geboorteplaats = "";
            Email = "";
            Telefoonnummer = "";
            Mobiel = "";
            Nationaliteit = "";
            Talen = "";
        }

        public string Naam { get; set; }

        public string GeboorteDatum  { get; set; }

        public string Geboorteplaats { get; set; }

        public string Email { get; set; }

        public string Telefoonnummer { get; set; }
        public string Telefoonnummer2 { get; set; }

        public string Mobiel { get; set; }

        public string Nationaliteit { get; set; }

        public string Talen { get; set; }

        [ForeignKey("fkReactieId")]
        public virtual Reactie? Reactie { get; set; }

        [ForeignKey("fkAdresId")]
        public virtual Adres? Adres { get; set; }

        [ForeignKey("fkGastgezinId")]
        public virtual Gastgezin? Gastgezin { get; set; }
    }
}
