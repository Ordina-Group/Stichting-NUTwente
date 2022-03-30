using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class MijnGastgezinnenModel
    {
        public MijnGastgezinnenModel()
        {
            MijnGastgezinnen = new List<MijnGastgezin>();
        }

        public List<MijnGastgezin> MijnGastgezinnen { get; set; }
    }

    public class MijnGastgezin
    {
        public string Naam { get; set; }

        public string Adres { get; set; }

        public string Woonplaats { get; set; }

        public string Telefoonnummer { get; set; }

        public string Email { get; set; }

        public DateTime Intake { get; set; }
    }
}
