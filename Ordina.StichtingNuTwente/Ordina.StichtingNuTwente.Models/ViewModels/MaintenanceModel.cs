using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class MaintenanceModel : BaseModel
    {
        public List<MaintenanceMessage> Messages { get; set; } = new List<MaintenanceMessage>();
    }
}