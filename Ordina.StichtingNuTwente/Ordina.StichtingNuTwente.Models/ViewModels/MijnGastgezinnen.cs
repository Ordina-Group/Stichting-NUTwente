using Ordina.StichtingNuTwente.Models.Models;
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
            MijnGastgezinnen = new List<GastGezin>();
        }

        public List<GastGezin> MijnGastgezinnen { get; set; }
    }

    public class GastGezin
    {
        public int Id { get; set; }

        public string Naam { get; set; }

        public string Adres { get; set; }

        public string Woonplaats { get; set; }

        public string Telefoonnummer { get; set; }

        public string Email { get; set; }

        public string Begeleider { get; set; }

        public int BegeleiderId { get; set; }

        public int BuddyId { get; set; }

        public DateTime Intake { get; set; }

        public int AanmeldFormulierId { get; set; }

        public int IntakeFormulierId { get; set; }

        public string? PlaatsingTag { get; set; }

        public string? ReserveTag { get; set; }

        public string? Note { get; set; }

        public PlaatsingsInfo? PlaatsingsInfo { get; set; }

        public bool? HasVOG { get; set; }

        public GastgezinStatus? Status { get; set; }
    }

    public class Vrijwilliger
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
    }

    public class AlleGastgezinnenModel : BaseModel
    {
        public AlleGastgezinnenModel()
        {
            Vrijwilligers = new List<Vrijwilliger>();

            GastgezinnenZonderBegeleider = new List<GastGezin>();
            GastgezinnenMetBegeleider = new List<GastGezin>();
        }

        public List<Vrijwilliger> Vrijwilligers { get; set; }

        public List<GastGezin> GastgezinnenZonderBegeleider { get; set; }
        public List<GastGezin> GastgezinnenMetBegeleider { get; set; }
    }
}