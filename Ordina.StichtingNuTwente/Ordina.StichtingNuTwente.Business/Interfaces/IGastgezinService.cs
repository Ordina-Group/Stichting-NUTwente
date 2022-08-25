using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Interfaces
{
    public interface IGastgezinService
    {
        public const string IncludeProperties = "Contact,Contact.Adres,Contact.Reactie,Vluchtelingen,Intaker,Buddy,PlaatsingsInfo,AanmeldFormulier,IntakeFormulier,Plaatsingen,Plaatsingen.Vrijwilliger,Comments,ContactLogs";
        public Gastgezin? GetGastgezin(int id, string includeProperties = IncludeProperties);
        public ICollection<Gastgezin> GetGastgezinnenForVrijwilliger(int vrijwilligerId, IEnumerable<Gastgezin>? gastgezinnen = null);
        public ICollection<Gastgezin> GetAllGastgezinnen(string includeProperties = IncludeProperties);
        public ICollection<Gastgezin> GetDeletedGastgezinnen(string includeProperties = IncludeProperties);
        public Gastgezin UpdateGastgezin(Gastgezin gastgezin, int? id = null);
        public void CheckOnholdGastgezinnen();
        public void UpdateNote(int gastgezinId, string note);
        public void UpdateVOG(bool hasVOG, int gastgezinId);
        public void Restore(int gastgezinId);
        public void Delete(int gastgezinId, bool deleteForms, UserDetails user, string comment);
        public void RejectBeingBuddy(Gastgezin gastgezin, string reason, UserDetails userDetails);
        public bool IntakerOrBuddyChange(List<IntakerOrBuddyChangeModel> intakerOrBuddyChangeModels, List<UserDetails> vrijwilligers);
        public void FillBaseModel(BaseModel model, UserDetails user);
        public BeschikbareGastgezinnenModel BeschikBeschikbareGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string[]? filters = null, string statusFilter = "", UserDetails? user = null);
        public AlleGastgezinnenModel AlleGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string statusFilter = "", UserDetails? user = null, List<UserDetails> vrijwilligers = null);
    }
}
