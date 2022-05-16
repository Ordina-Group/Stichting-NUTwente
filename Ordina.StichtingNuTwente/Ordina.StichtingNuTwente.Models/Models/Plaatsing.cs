

using System.ComponentModel.DataAnnotations.Schema;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Plaatsing : BaseEntity
    {
        [ForeignKey("fkGastgezinId")]
        public virtual Gastgezin Gastgezin { get; set; }
        public int Amount { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public PlacementType PlacementType { get; set; }
        public DateTime DateTime { get; set; }
        public UserDetails Vrijwilliger { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public bool Active { get; set; }
    }
}
