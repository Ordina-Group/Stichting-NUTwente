using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class VerwijderdePlaatsingen
    {
        public List<PlaatsingViewModel> plaatsingen;
        public int total;
        public List<plaatsingStats> plaatsingStats;
    }
    public class plaatsingStats
    {
        DepartureDestination destination;

    }
}
