using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IPlaatsingenService
    {
        public ICollection<Plaatsing> GetAllPlaatsingen(PlacementType? placementType);
        public Plaatsing CheckAge(Plaatsing plaatsing);
    }
}
