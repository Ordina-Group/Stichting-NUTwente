using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Reactie : BaseEntity
    {
        public ICollection<Antwoord> Antwoorden { get; set; }

        public DateTime DatumIngevuld { get; set; }

        public int FormulierId { get; set; }

        //public string UserId { get; set; }

    }
}
