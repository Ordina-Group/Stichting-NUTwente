using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Gastgezin: BaseEntity
    {
        public ICollection<Persoon> Vluchtelingen { get; set; }
        public Persoon Contact { get; set; }
        public UserDetails? Begeleider { get; set; }
        public GastgezinStatus Status { get; set; }
        public Reactie? IntakeFormulier { get; set; }
    }
}
