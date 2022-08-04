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

        public Plaatsing CheckAge(Plaatsing plaatsing)
        {
            if (plaatsing.Age != -1)
            {
                if (plaatsing.Age > 120)
                {
                    plaatsing.Age = 120;
                }
                else if (plaatsing.Age > 17)
                {
                    plaatsing.AgeGroup = AgeGroup.Volwassene;
                }
                else if (plaatsing.Age < 18 && plaatsing.Age >= 0)
                {
                    plaatsing.AgeGroup = AgeGroup.Kind;
                }
                else if (plaatsing.Age < -1)
                {
                    plaatsing.Age = -1;
                }
            }
            return plaatsing;
        }
    }
}
