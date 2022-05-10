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
        public ICollection<Persoon>? Vluchtelingen { get; set; }
        public Persoon Contact { get; set; }
        public UserDetails? Begeleider { get; set; }
        public UserDetails? Buddy { get; set; }
        public GastgezinStatus? Status { get; set; }
        public Reactie? IntakeFormulier { get; set; }
        public Reactie? AanmeldFormulier { get; set; }
        public ICollection<Plaatsing>? Plaatsingen { get; set; }
        public bool? HasVOG { get; set; }
        public string? Note { get; set; }
        public int? MaxAdults { get; set; }
        public int? MaxChildren { get; set; }
        [ForeignKey("fkPlaatsingsId")]
        public virtual PlaatsingsInfo? PlaatsingsInfo { get; set; }

        public bool BekekenDoorBuddy { get; set; }
        public bool BekekenDoorIntaker { get; set; }

        public List<Comment>? Comments { get; set; }
    }
}
