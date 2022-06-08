using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class GastgezinMaintenanceViewModel
    {
        public GastgezinMaintenanceViewModel()
        {
            Gastgezin = new();
            Vrijwilligers = new();
        }
        public GastgezinViewModel Gastgezin { get; set; }
        public List<Vrijwilliger> Vrijwilligers { get; set; }

    }
}
