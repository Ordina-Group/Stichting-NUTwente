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
    public class GastgezinService : IGastgezinService
    {
        private readonly NuTwenteContext _context;
        public GastgezinService(NuTwenteContext context)
        {
            _context = context;
        }

        public Gastgezin? GetGastgezin(int id)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);

            return gastgezinRepository.GetById(id, "Contact,Contact.Reactie,Vluchtelingen,Begeleider,Plaatsingen,Plaatsingen.Vrijwilliger,IntakeFormulier,PlaatsingsInfo,AanmeldFormulier");
        }

        public Gastgezin? GetGastgezinForReaction(int formID)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            var gastgezin = gastgezinRepository.GetAll("Contact.Reactie,Vluchtelingen,Begeleider,PlaatsingsInfo,AanmeldFormulier").FirstOrDefault(g => g.Contact.Reactie.Id == formID);
            if(gastgezin == null)
            {
                var persoonRepository = new Repository<Persoon>(_context);
                var persoon = persoonRepository.GetAll("Reactie").FirstOrDefault(p => p.Reactie.Id == formID);
                gastgezin = new Gastgezin()
                {
                    AanmeldFormulier = persoon.Reactie,
                    Contact = persoon,
                    Status = GastgezinStatus.Aangemeld
                };

                var success = Save(gastgezin);
                if (!success)
                    gastgezin = null;
            }
            return gastgezin;
        }

        public ICollection<Gastgezin> GetGastgezinnenForVrijwilliger(Persoon vrijwilliger)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);

            var gastgezinnen = gastgezinRepository.GetAll("Contact,Vluchtelingen,Begeleider,Contact.Adres,Contact.Reactie,IntakeFormulier,PlaatsingsInfo,AanmeldFormulier").Where(g => g.Begeleider != null && g.Begeleider.Id == vrijwilliger.Id);
            return gastgezinnen.ToList();
        }

        public ICollection<Gastgezin> GetAllGastgezinnen()
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);

            var gastgezinnen = gastgezinRepository.GetAll("Contact,Vluchtelingen,Begeleider,Contact.Adres,Contact.Reactie,PlaatsingsInfo,AanmeldFormulier");
            return gastgezinnen.ToList();
        }

        public bool Save(Gastgezin gastgezin)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            gastgezin.Id = -1;
            var dbModel = gastgezinRepository.Create(gastgezin);
            return dbModel.Id > 0;
        }

        public Gastgezin UpdateGastgezin(Gastgezin gastgezin, int id)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            gastgezinRepository.Update(gastgezin);
            return gastgezin;
        }

        public void AddPlaatsing(Plaatsing plaatsing)
        {
            var plaatsingRepository = new Repository<Plaatsing>(_context);
            plaatsingRepository.Create(plaatsing);
        }
        public void UpdatePlaatsing(Plaatsing plaatsing)
        {
            var plaatsingRepository = new Repository<Plaatsing>(_context);
            plaatsingRepository.Update(plaatsing);
        }

        public Plaatsing GetPlaatsing(int id)
        {
            throw new NotImplementedException();
        }

        public List<Plaatsing> GetPlaatsingen(int? gastGezinId = null, PlacementType? type = null, AgeGroup? ageGroup = null)
        {
            var plaatsingRepository = new Repository<Plaatsing>(_context);
            var plaatsingen = plaatsingRepository.GetAll("Gastgezin");
            if (gastGezinId != null)
            {
                plaatsingen = plaatsingen.Where(p => p.Gastgezin.Id == gastGezinId);
            }
            if (type != null)
            {
                plaatsingen = plaatsingen.Where(p => p.PlacementType == type);
            }
            if (ageGroup != null)
            {
                plaatsingen = plaatsingen.Where(p => p.AgeGroup == ageGroup);
            }
            //List<Plaatsing> plaatsingen = new();
            return plaatsingen.ToList();
        }

        public string GetPlaatsingTag(int gastgezinId, PlacementType placementType)
        {
            var plaatsingen = GetPlaatsingen(gastgezinId);
            int? PlaatsVolwassen = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == placementType).Sum(p => p.Amount);
            int? PlaatsKinderen = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == placementType).Sum(p => p.Amount);
            int? PlaatsOnbekend = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == placementType).Sum(p => p.Amount);
            int? total = PlaatsVolwassen + PlaatsKinderen + PlaatsOnbekend;
            string tag = total + "(" + PlaatsVolwassen + "v " + PlaatsKinderen + "k " + PlaatsOnbekend + "?)";
            return tag;
        }

        public void UpdateNote(int gastgezinId, string note)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            var gastgezin = gastgezinRepository.GetById(gastgezinId);
            if(note == null)
            {
                note = "";
            }
            gastgezin.Note = note;
            gastgezinRepository.Update(gastgezin);
        }

        public void UpdateVOG(bool hasVOG, int gastgezinId)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            var gastgezin = gastgezinRepository.GetById(gastgezinId);
            if (hasVOG == null)
            {
                hasVOG = false;
            }
            gastgezin.HasVOG = hasVOG;
            gastgezinRepository.Update(gastgezin);
        }
    }
}
