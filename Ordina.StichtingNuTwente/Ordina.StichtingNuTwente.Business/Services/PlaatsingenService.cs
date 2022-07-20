using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class PlaatsingenService : IPlaatsingenService
    {
        private readonly IRepository<Plaatsing> PlaatsingenRepository;

        public PlaatsingenService(IRepository<Plaatsing> plaatsingenRepository)
        {
            PlaatsingenRepository = plaatsingenRepository;
        }

        public ICollection<Plaatsing> GetAllPlaatsingen(PlacementType? placementType)
        {
            var plaatsingen = PlaatsingenRepository.GetAll("Gastgezin,Gastgezin.Contact,Vrijwilliger").Where(p => p.PlacementType == placementType);
            return plaatsingen.ToList();
        }
    }
}
