

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
    }

    [Flags]
    public enum AgeGroup
    {
        Volwassene = 0,
        Kind = 1,
        Onbekend = 2
    }

    [Flags]
    public enum PlacementType
    {
        Reservering = 0,
        Plaatsing = 1
    }
}
