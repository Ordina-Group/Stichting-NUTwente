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
        public void AddPlaatsing(Plaatsing plaatsing);
        public void UpdatePlaatsing(Plaatsing plaatsing);
        public Plaatsing GetPlaatsing(int id);
        public List<Plaatsing> GetPlaatsingen(int? gastGezinId = null, PlacementType? type = null, AgeGroup? ageGroup = null);
        public string GetPlaatsingTag(int gastgezinId, PlacementType placementType, Gastgezin? gastgezin = null);
        public void UpdateNote(int gastgezinId, string note);
        public void UpdateVOG(bool hasVOG, int gastgezinId);
        public bool PlaatsingExists(int gastgezinId, Plaatsing plaatsing);
        public void Delete(int gastgezinId);
        public string GetPlaatsingenTag(List<Gastgezin> gastgezinnen, PlacementType placementType);
    }
}
