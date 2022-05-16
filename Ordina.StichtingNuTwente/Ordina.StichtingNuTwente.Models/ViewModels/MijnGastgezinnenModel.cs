using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class MijnGastgezinnenModel : BaseModel
    {
        public MijnGastgezinnenModel()
        {
            MijnGastgezinnen = new List<GastgezinViewModel>();
        }

        public List<GastgezinViewModel> MijnGastgezinnen { get; set; }
        public string? GastgezinnenVan {  get; set; }
    }
}