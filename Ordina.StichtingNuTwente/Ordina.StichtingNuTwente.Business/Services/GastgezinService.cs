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
        private readonly IReactionService _reactionService;

        public GastgezinService(NuTwenteContext context, IReactionService reactionService)
        {
            _context = context;
            _reactionService = reactionService;
        }

        public Gastgezin? GetGastgezin(int id, string includeProperties = IGastgezinService.IncludeProperties)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            return gastgezinRepository.GetById(id, includeProperties);
        }

        public ICollection<Gastgezin> GetGastgezinnenForVrijwilliger(int vrijwilligerId, IEnumerable<Gastgezin>? gastgezinnen = null)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            if (gastgezinnen == null)
            {
                gastgezinnen = GetAllGastgezinnen();
            }
            var gastgezinnenForVrijwilliger = gastgezinnen.Where(g => !g.Deleted && (g.Begeleider != null && g.Begeleider.Id == vrijwilligerId) || (g.Buddy != null && g.Buddy.Id == vrijwilligerId));
            return gastgezinnenForVrijwilliger.ToList();
        }

        public ICollection<Gastgezin> GetAllGastgezinnen(string includeProperties = IGastgezinService.IncludeProperties)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);

            var gastgezinnen = gastgezinRepository.GetAll(includeProperties).Where(g => !g.Deleted);
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
            var gastgezin = plaatsing.Gastgezin;
            if ((gastgezin.Status == GastgezinStatus.Aangemeld || gastgezin.Status == GastgezinStatus.Bezocht) && plaatsing.PlacementType == PlacementType.Plaatsing)
            {
                gastgezin.Status = GastgezinStatus.Geplaatst;
                UpdateGastgezin(gastgezin, gastgezin.Id);
            }
            else if (plaatsing.Amount < 0 && gastgezin.Status == GastgezinStatus.Geplaatst)
            {
                var plaatsingen = GetPlaatsingen(gastgezin.Id, PlacementType.Plaatsing);
                var total = plaatsingen.Sum(p => p.Amount);
                if (total == 0)
                {
                    gastgezin.Status = GastgezinStatus.Bezocht;
                    UpdateGastgezin(gastgezin, gastgezin.Id);
                }
            }
        }
        public void UpdatePlaatsing(Plaatsing plaatsing)
        {
            var plaatsingRepository = new Repository<Plaatsing>(_context);
            plaatsingRepository.Update(plaatsing);
        }

        public Plaatsing GetPlaatsing(int id)
        {
            var plaatsingRepository = new Repository<Plaatsing>(_context);
            var plaatsing = plaatsingRepository.GetById(id, "Gastgezin");
            return plaatsing;
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

        public string GetPlaatsingTag(int gastgezinId, PlacementType placementType, Gastgezin? gastgezin)
        {
            string status = "";
            if (gastgezin == null) gastgezin = GetGastgezin(gastgezinId);
            if (gastgezin.Status == GastgezinStatus.OnHold)
            {
                status = "ON HOLD ";
            }
            if (gastgezin.Status == GastgezinStatus.NoodOpvang)
            {
                status = "NOOD ";
            }
            var plaatsingen = gastgezin.Plaatsingen.Where(p => p.Active == true).ToList(); ;
            int? PlaatsVolwassen = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) PlaatsVolwassen += plaatsingen.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);

            int? PlaatsKinderen = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) PlaatsKinderen += plaatsingen.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);

            int? PlaatsOnbekend = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) PlaatsOnbekend += plaatsingen.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);

            int? total = PlaatsVolwassen + PlaatsKinderen + PlaatsOnbekend;
            string calculation = "";
            if (!(total == 0 && status != ""))
            {
                calculation = total + "(" + PlaatsVolwassen + "v " + PlaatsKinderen + "k " + PlaatsOnbekend + "?)";
            }
            string tag = status + calculation;
            return tag;
        }

        public string GetPlaatsingenTag(List<Gastgezin> gastgezinnen, PlacementType placementType)
        {
            string tag = "";
            var plaatsingen = new List<Plaatsing>();

            gastgezinnen.ForEach(g => plaatsingen.AddRange(g.Plaatsingen));
            plaatsingen = plaatsingen.Where(p => p.Active == true).ToList();

            int? PlaatsVolwassen = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) PlaatsVolwassen += plaatsingen.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);

            int? PlaatsKinderen = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) PlaatsVolwassen += plaatsingen.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);

            int? PlaatsOnbekend = plaatsingen.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) PlaatsOnbekend += plaatsingen.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);

            int? total = PlaatsVolwassen + PlaatsKinderen + PlaatsOnbekend;
            tag = total + "(" + PlaatsVolwassen + "v " + PlaatsKinderen + "k " + PlaatsOnbekend + "?)";
            return tag;
        }

        public void UpdateNote(int gastgezinId, string note)
        {
            var gastgezinRepository = new Repository<Gastgezin>(_context);
            var gastgezin = gastgezinRepository.GetById(gastgezinId);
            if (note == null)
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
        public bool PlaatsingExists(int gastgezinId, Plaatsing plaatsing)
        {
            var plaastingen = GetPlaatsingen(gastgezinId, plaatsing.PlacementType, plaatsing.AgeGroup);
            if (plaastingen.FirstOrDefault(p => p.DateTime == plaatsing.DateTime && p.Amount == plaatsing.Amount) != null)
            {
                return true;
            }
            return false;
        }

        public void Delete(int gastgezinId, bool deleteForms)
        {
            var gastgezinInDb = GetGastgezin(gastgezinId);
            if (gastgezinInDb == null)
                return;

            gastgezinInDb.Deleted = true;
            UpdateGastgezin(gastgezinInDb, gastgezinId);
            var reactieRepository = new Repository<Reactie>(_context);
            var aanmeld = gastgezinInDb.AanmeldFormulier;
            if (aanmeld != null && deleteForms)
            {
                aanmeld.Deleted = true;
                reactieRepository.Update(aanmeld);
            }
            var intake = gastgezinInDb.IntakeFormulier;
            if (intake != null && deleteForms)
            {
                intake.Deleted = true;
                reactieRepository.Update(intake);
            }
            /*
            if (gastgezinInDb.PlaatsingsInfo != null)
            {
                var plaatsingsInfoRepository = new Repository<PlaatsingsInfo>(_context);
                plaatsingsInfoRepository.Delete(gastgezinInDb.PlaatsingsInfo);
            }
            if (gastgezinInDb.Plaatsingen != null && gastgezinInDb.Plaatsingen.Count > 0)
            {
                var plaatsingRepository = new Repository<Plaatsing>(_context);
                foreach (var plaatsing in gastgezinInDb.Plaatsingen)
                {
                    plaatsingRepository.Delete(plaatsing);
                }

            }

            var persoonRepository = new Repository<Persoon>(_context);
            var personen = persoonRepository.GetAll("Gastgezin").Where(p => p.Gastgezin != null && (p.Gastgezin.Id == gastgezinInDb.AanmeldFormulier?.Id || p.Gastgezin.Id == gastgezinInDb.IntakeFormulier?.Id));
            foreach(var persoon in personen)
            {
                persoon.Gastgezin = null;
                persoonRepository.Update(persoon);
            }

            gastgezinRepository.Delete(gastgezinInDb);
            if (gastgezinInDb.Comments != null && gastgezinInDb.Comments.Count > 0)
            {
                var commentRepository = new Repository<Comment>(_context);
                foreach (var comment in gastgezinInDb.Comments)
                {
                    commentRepository.Delete(comment);
                }
            }
            if (deleteForms)
            {
                if (gastgezinInDb.IntakeFormulier != null)
                {
                    var count = gastgezinRepository.GetAll("IntakeFormulier").Count(g => g.IntakeFormulier != null && g.IntakeFormulier.Id == gastgezinInDb.IntakeFormulier.Id);
                    if (count == 0)
                    {
                        var intake = gastgezinInDb.IntakeFormulier.Id;
                        _reactionService.Delete(intake);
                    }
                }
                if (gastgezinInDb.AanmeldFormulier != null)
                {
                    var count = gastgezinRepository.GetAll("AanmeldFormulier").Count(g => g.AanmeldFormulier != null && g.AanmeldFormulier.Id == gastgezinInDb.AanmeldFormulier.Id);
                    if (count == 0)
                    {
                        var aanmeld = gastgezinInDb.AanmeldFormulier.Id;
                        _reactionService.Delete(aanmeld);
                    }
                }
            }
            */
        }

        public void RejectBeingBuddy(Gastgezin gastgezin, string reason, UserDetails userDetails)
        {
            gastgezin.Buddy = null;
            if (gastgezin.Comments == null) gastgezin.Comments = new List<Comment>();

            var comment = new Comment(reason, userDetails, CommentType.BUDDY_REJECTION);

            gastgezin.Comments.Add(comment);
            UpdateGastgezin(gastgezin, gastgezin.Id);
        }
    }
}
