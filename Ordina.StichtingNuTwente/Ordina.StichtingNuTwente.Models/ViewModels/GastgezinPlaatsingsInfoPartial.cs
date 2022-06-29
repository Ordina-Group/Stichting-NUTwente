using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class GastgezinPlaatsingsInfoPartial
    {
        public PlaatsingsInfo? PlaatsingsInfo { get; set; }
        public int Index { get; set; }
        public string? Notities { get; set; }
        public int GastgezinId { get; }
        public string VrijwilligerOpmerking { get; }
        public string CoordinatorOpmerking { get; }
        public int Columns { get; set; }

        public GastgezinPlaatsingsInfoPartial(PlaatsingsInfo plaatsingsInfo, int index, int columns, string? notities, int gastgezinId = -1, string vrijwilligerOpmerking = "", string coordinatorOpmerking = "")
        {
            Columns = columns;
            PlaatsingsInfo = plaatsingsInfo;
            Index = index;
            Notities = notities;
            GastgezinId = gastgezinId;
            VrijwilligerOpmerking = vrijwilligerOpmerking;
            CoordinatorOpmerking = coordinatorOpmerking;
        }


    }
}
