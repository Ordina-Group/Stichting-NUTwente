using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class GastgezinStatsViewModel
    {

        public int AantalVluchtelingenGeplaatst { get; set; }
        public int VerdeeldOverAantalGastgezinnen { get; set; }

        public GastgezinStatsViewModel(int aantalVluchtelingenGeplaatst, int verdeeldOverAantalGastgezinnen)
        {
            AantalVluchtelingenGeplaatst = aantalVluchtelingenGeplaatst;
            VerdeeldOverAantalGastgezinnen = verdeeldOverAantalGastgezinnen;
        }

    }
}
