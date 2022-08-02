using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IMailService
    {
        public Task<bool> ToekennenIntaker(Gastgezin gastgezin, Persoon persoon);
        public Task<bool> ToekennenBuddy(Gastgezin gastgezin, Persoon persoon);
        public Task<bool> VertrekVluchteling(Gastgezin gastgezin, Persoon persoon);
        public Task<bool> PlaatsingsReservering(Gastgezin gastgezin, Persoon persoon);
        public Task<bool> AanmeldenVrijwilliger(Persoon persoon);
        public Task<bool> VerwijderenGastgezin(Gastgezin gastgezin, Persoon persoon);
        public Task<bool> IntakeUitgevoerd(Gastgezin gastgezin);
        public Task<bool> AanmeldingVrijwilliger(Persoon persoon);
        public Task<bool> AanmeldingGastgezin(Persoon persoon);
        public Task<bool> PlaatsingVluchteling(Gastgezin gastgezin);

    }
}
