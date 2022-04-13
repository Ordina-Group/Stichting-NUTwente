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
            MijnGastgezinnen = new List<GastGezin>();
            Vrijwilligers = new List<Vrijwilliger>();
        }

        public List<GastGezin> MijnGastgezinnen { get; set; }

        public List<Vrijwilliger> Vrijwilligers { get; set; }
    }

    public class GastGezin
    {
        public int Id { get; set; }

        public string Naam { get; set; }

        public string Adres { get; set; }

        public string Woonplaats { get; set; }

        public string Telefoonnummer { get; set; }

        public string Email { get; set; }

        public DateTime Intake { get; set; }

        public int AanmeldFormulierId { get; set; }

        public int IntakeFormulierId { get; set; }
    }

    public class Vrijwilliger
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
    }

    public class AlleGastgezinnenModel
    {
        public AlleGastgezinnenModel()
        {
            AlleGastgezinnen = new List<GastGezin>();
        }

        public List<GastGezin> AlleGastgezinnen { get; set; }
    }

}
