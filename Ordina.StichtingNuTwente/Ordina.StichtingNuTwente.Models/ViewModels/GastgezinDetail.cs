using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class GastgezinViewModel
    {
        public GastgezinViewModel()
        {
            GastGezin = new GastGezin();
            PlaatsingsGeschiedenis = new List<PlaatsingViewModel>();
            PlaatsingDTO = new PlaatsingDTO();
            PlaatsingStats = new PlaatsingStats();
        }

        public GastGezin GastGezin { get; set; }
        public List<PlaatsingViewModel> PlaatsingsGeschiedenis { get; set; }
        public PlaatsingDTO PlaatsingDTO { get; set; }
        public PlaatsingStats PlaatsingStats { get; set; }
    }

    public class PlaatsingViewModel
    {
        public PlaatsingViewModel(Plaatsing plaatsing)
        {
            Gastgezin = plaatsing.Gastgezin;
            Amount = plaatsing.Amount;
            AgeGroup = plaatsing.AgeGroup;
            PlacementType = plaatsing.PlacementType;
            DateTime = plaatsing.DateTime;
            Vrijwilliger = plaatsing.Vrijwilliger;
            Age = plaatsing.Age;
            Gender = plaatsing.Gender;
        }

        public Gastgezin Gastgezin { get; set; }

        public int Amount { get; set; }

        public AgeGroup AgeGroup { get; set; }

        public PlacementType PlacementType { get; set; }

        public DateTime DateTime { get; set; }

        public UserDetails Vrijwilliger { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }
    }

    public class PlaatsingStats
    {
        public int PlaatsVolwassen { get; set; }
        public int PlaatsKind { get; set; }
        public int PlaatsOnbekend { get; set; }
        public int ResVolwassen { get; set; }
        public int ResKind { get; set; }
        public int ResOnbekend { get; set; }
    }

    public class PlaatsingDTO
    {
        public int GastgezinId { get; set; }

        public int Amount { get; set; }

        public int AgeGroup { get; set; }

        public int PlacementType { get; set; }
    }
}
