using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    [Flags]
    public enum GastgezinStatus
    {
        Aangemeld = 0,
        Bezocht = 1,
        Geplaatst = 2,
        Teruggetrokken = 3,
        OnHold = 4
    }
}
