using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    // Dit is een bitwise enum: https://www.alanzucconi.com/2015/07/26/enum-flags-and-bitwise-operators/
    [Flags] public enum VrijwilligerType
    {
        None            = 0,
        Vrijwilliger    = 1,
        GastGezin       = 2,

    }
}
