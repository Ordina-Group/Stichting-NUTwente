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
        public void AddPlaatsing(Plaatsing plaatsing);
        public void UpdatePlaatsing(Plaatsing plaatsing);
        public void RemoveReserveringPlaatsingen(DateTime departureDate, int plaatsingId, string departureReason, UserDetails user, DepartureDestination? departureDestination = null, string? departureComment = null);
        public Plaatsing GetPlaatsing(int id);
        public List<Plaatsing> GetPlaatsingen(int? gastGezinId = null, PlacementType? type = null, AgeGroup? ageGroup = null);
        public string GetPlaatsingTag(PlacementType placementType, Gastgezin gastgezin);
        public bool PlaatsingExists(int gastgezinId, Plaatsing plaatsing);
        public string GetPlaatsingenTag(List<Gastgezin> gastgezinnen, PlacementType placementType);
        public void RemoveReserveringOnHold(Gastgezin gastgezin, UserDetails user);
    }
}
