using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IGastgezinService
    {
        public bool Save(Gastgezin gastgezin);
        public Gastgezin? GetGastgezinForReaction(int formID);
        public Gastgezin? GetGastgezin(int id);
        public ICollection<Gastgezin> GetGastgezinnenForVrijwilliger(Persoon vrijwilliger);
        public ICollection<Gastgezin> GetAllGastgezinnen();
        public Gastgezin UpdateGastgezin(Gastgezin gastgezin, int id);
    }
}
