using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Gastgezin: BaseEntity
    {
        public ICollection<Vrijwilliger> Vrijwilligers { get; set; }

    }
}
