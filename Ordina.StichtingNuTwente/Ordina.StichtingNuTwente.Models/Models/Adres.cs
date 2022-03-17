using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Adres : BaseEntity
    {
        public Adres()
        {
            Straat = "";
            Postcode = "";
            Woonplaats = "";
        }

        public string Straat { get; set; }

        public string Postcode { get; set; }   

        public string Woonplaats { get; set; }

        [ForeignKey("fkReactieId")]
        public virtual Reactie? Reactie { get; set; }
    }
}
