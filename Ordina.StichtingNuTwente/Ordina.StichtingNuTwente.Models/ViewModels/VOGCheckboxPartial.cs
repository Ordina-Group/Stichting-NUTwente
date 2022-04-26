using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class VOGCheckboxPartial
    {
        public bool? HasVOG { get; set; }
        public int GastgezinId { get; set; }

        public VOGCheckboxPartial(bool? hasVOG, int gastgezinIndex)
        {
            HasVOG = hasVOG;
            GastgezinId = gastgezinIndex;
        }
    }
}
