using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class GastgezinDetailViewModel
    {
        public GastgezinDetailViewModel()
        {
            GastGezin = new GastgezinViewModel();
            Plaatsingen = new List<PlaatsingViewModel>();
            Reserveringen = new List<PlaatsingViewModel>();
            PlaatsingsGeschiedenis = new List<PlaatsingViewModel>();
            PlaatsingDTO = new PlaatsingDTO();
            PlaatsingStats = new PlaatsingStats();
        }

        public GastgezinViewModel GastGezin { get; set; }
        public List<PlaatsingViewModel> Plaatsingen { get; set; }
        public List<PlaatsingViewModel> Reserveringen { get; set; }
        public List<PlaatsingViewModel> PlaatsingsGeschiedenis { get; set; }
        public PlaatsingDTO PlaatsingDTO { get; set; }
        public PlaatsingStats PlaatsingStats { get; set; }
    }
}
