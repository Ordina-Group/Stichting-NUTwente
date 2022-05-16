using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class DatabaseIntegrityModel : BaseModel
    {
        public List<DatabaseIntegrityTest> Inconsistencies { get; set; } = new List<DatabaseIntegrityTest>();

        public List<DatabaseIntegrityTest> Statistics { get; set; } = new List<DatabaseIntegrityTest>();
    }
}
