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
        public Gastgezin()
        {
            VrijwilligerOpmerkingen = "";
            ContactLogs = new List<ContactLog>();
        }
        public ICollection<Persoon>? Vluchtelingen { get; set; }
        public Persoon Contact { get; set; }
        public UserDetails? Begeleider { get; set; }
        public UserDetails? Buddy { get; set; }
        public GastgezinStatus? Status
        {
            get
            {
                if (OnHold)
                    return GastgezinStatus.OnHold;
                var plaatsingen = Plaatsingen?.Where(p => (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering) && p.Active).Sum(p => p.Amount);
                if (plaatsingen > 0)
                    return GastgezinStatus.Geplaatst;
                var reserveringen = Plaatsingen?.Where(p => (p.PlacementType == PlacementType.Reservering) && p.Active).Sum(p => p.Amount);
                if (reserveringen > 0)
                    return GastgezinStatus.Gereserveerd;
                if (IntakeFormulier != null)
                    return GastgezinStatus.Bezocht;
                return GastgezinStatus.Aangemeld;
            }
        }
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

        public bool Deleted { get; set; }

        public string VrijwilligerOpmerkingen { get; set; }
        public List<ContactLog> ContactLogs { get; set; }

        public bool OnHold { get; set; }
        public bool NoodOpvang { get; set; }
    }
}
