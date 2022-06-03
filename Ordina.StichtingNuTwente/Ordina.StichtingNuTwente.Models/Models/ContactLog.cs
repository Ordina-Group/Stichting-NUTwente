using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class ContactLog : BaseEntity
    {

        public DateTime DateTime { get; set; }
        public UserDetails Contacter { get; set; }
        public string Notes { get; set; }
    }
}
