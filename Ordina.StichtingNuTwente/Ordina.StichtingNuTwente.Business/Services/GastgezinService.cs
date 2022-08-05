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
        private readonly IReactionService _reactionService;
        private readonly IRepository<Gastgezin> GastgezinRepository;
        private readonly IRepository<Plaatsing> PlaatsingsRepository;
        private readonly IRepository<Reactie> ReactieRepository;
        private readonly IPlaatsingenService _plaatsingenService;

        public GastgezinService(IReactionService reactionService, IRepository<Gastgezin> gastgezinRepository, IRepository<Plaatsing> plaatsingsRepository, IRepository<Reactie> reactieRepository, IPlaatsingenService plaatsingenService)
        {
            _reactionService = reactionService;
            GastgezinRepository = gastgezinRepository;
            this.PlaatsingsRepository = plaatsingsRepository;
            ReactieRepository = reactieRepository;
            _plaatsingenService = plaatsingenService;
        }

        public Gastgezin? GetGastgezin(int id, string includeProperties = IGastgezinService.IncludeProperties)
        {
            return GastgezinRepository.GetById(id, includeProperties);
        }

        public ICollection<Gastgezin> GetGastgezinnenForVrijwilliger(int vrijwilligerId, IEnumerable<Gastgezin>? gastgezinnen = null)
        {
            if (gastgezinnen == null)
            {
                gastgezinnen = GetAllGastgezinnen();
            }
            var gastgezinnenForVrijwilliger = gastgezinnen.Where(g => !g.Deleted && (g.Intaker != null && g.Intaker.Id == vrijwilligerId) || (g.Buddy != null && g.Buddy.Id == vrijwilligerId));
            return gastgezinnenForVrijwilliger.ToList();
        }

        public ICollection<Gastgezin> GetAllGastgezinnen(string includeProperties = IGastgezinService.IncludeProperties)
        {
            var gastgezinnen = GastgezinRepository.GetAll(includeProperties).Where(g => !g.Deleted);
            return gastgezinnen.ToList();
        }

        public ICollection<Gastgezin> GetDeletedGastgezinnen(string includeProperties = IGastgezinService.IncludeProperties)
        {
            var gastgezinnen = GastgezinRepository.GetAll(includeProperties).Where(g => g.Deleted);
            return gastgezinnen.ToList();
        }

        public bool Save(Gastgezin gastgezin)
        {
            gastgezin.Id = -1;
            var dbModel = GastgezinRepository.Create(gastgezin);
            return dbModel.Id > 0;
        }

        public Gastgezin UpdateGastgezin(Gastgezin gastgezin, int? id)
        {
            GastgezinRepository.Update(gastgezin);
            return gastgezin;
        }

        public void AddPlaatsing(Plaatsing plaatsing)
        {
            PlaatsingsRepository.Create(_plaatsingenService.CheckAge(plaatsing));
        }
        public void UpdatePlaatsing(Plaatsing plaatsing)
        {
            PlaatsingsRepository.Update(_plaatsingenService.CheckAge(plaatsing));
        }

        public Plaatsing GetPlaatsing(int id)
        {
            var plaatsing = PlaatsingsRepository.GetById(id, "Gastgezin");
            return plaatsing;
        }

        public List<Plaatsing> GetPlaatsingen(int? gastGezinId = null, PlacementType? type = null, AgeGroup? ageGroup = null)
        {
            var plaatsingen = PlaatsingsRepository.GetAll("Gastgezin");
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
            var gastgezinStatus = gastgezin.Status;

            if (gastgezin.OnHold)
            {
                status = "ON HOLD ";
            }
            else if (gastgezin.NoodOpvang)
            {
                status = "NOOD ";
            }
            var plaatsingen = gastgezin.Plaatsingen.Where(p => p.Active == true);
            int? total = plaatsingen.Where(p => p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) total += plaatsingen.Where(p => p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);
            string tag = "";
            if (total == 0 && status != "")
            {
                tag = status;
            }
            else
            {
                tag = status + total;
            }
            return tag;
        }

        public void CheckOnholdGastgezinnen()
        {
            var gastGezinnen = GetAllGastgezinnen().Where(g => g.OnHold == true && g.OnHoldTill != null && g.OnHoldTill.Value.Subtract(DateTime.Now).TotalHours <= 0);

            foreach (var gastGezin in gastGezinnen)
            {
                gastGezin.OnHold = false;
                gastGezin.OnHoldTill = null;
                UpdateGastgezin(gastGezin, gastGezin.Id);
            }
        }

        public string GetPlaatsingenTag(List<Gastgezin> gastgezinnen, PlacementType placementType)
        {
            var plaatsingen = new List<Plaatsing>();

            gastgezinnen.ForEach(g => plaatsingen.AddRange(g.Plaatsingen));
            plaatsingen = plaatsingen.Where(p => p.Active == true).ToList();
            int? total = plaatsingen.Where(p => p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) total += plaatsingen.Where(p => p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);
            string tag = total.ToString();
            return tag;
        }

        public void UpdateNote(int gastgezinId, string note)
        {
            var gastgezin = GastgezinRepository.GetById(gastgezinId);
            if (note == null)
            {
                note = "";
            }
            gastgezin.Note = note;
            GastgezinRepository.Update(gastgezin);
        }

        public void UpdateVOG(bool hasVOG, int gastgezinId)
        {
            var gastgezin = GastgezinRepository.GetById(gastgezinId);
            if (hasVOG == null)
            {
                hasVOG = false;
            }
            gastgezin.HasVOG = hasVOG;
            GastgezinRepository.Update(gastgezin);
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

        public void Restore(int gastgezinId)
        {
            var gastgezinInDb = GetGastgezin(gastgezinId);
            if (gastgezinInDb == null)
                return;

            gastgezinInDb.Deleted = false;
            gastgezinInDb.Comments?.RemoveAll(c => c.CommentType == CommentType.DELETION);
            UpdateGastgezin(gastgezinInDb, gastgezinId);
            var aanmeld = gastgezinInDb.AanmeldFormulier;
            if (aanmeld != null)
            {
                aanmeld.Deleted = false;
                aanmeld.Comments?.RemoveAll(c => c.CommentType == CommentType.DELETION);
                ReactieRepository.Update(aanmeld);
            }
            var intake = gastgezinInDb.IntakeFormulier;
            if (intake != null)
            {
                intake.Deleted = false;
                intake.Comments?.RemoveAll(c => c.CommentType == CommentType.DELETION);
                ReactieRepository.Update(intake);
            }
        }

        public void Delete(int gastgezinId, bool deleteForms, UserDetails user, string comment)
        {
            var gastgezinInDb = GetGastgezin(gastgezinId);
            if (gastgezinInDb == null)
                return;

            gastgezinInDb.Deleted = true;
            if (gastgezinInDb.Comments == null)
                gastgezinInDb.Comments = new List<Comment>();
            gastgezinInDb.Comments.Add(new Comment(comment, user, CommentType.DELETION));
            UpdateGastgezin(gastgezinInDb, gastgezinId);

            var aanmeld = gastgezinInDb.AanmeldFormulier;
            if (aanmeld != null && deleteForms)
            {
                aanmeld.Deleted = true;
                if (aanmeld.Comments == null)
                    aanmeld.Comments = new List<Comment>();
                aanmeld.Comments.Add(new Comment(comment, user, CommentType.DELETION));
                ReactieRepository.Update(aanmeld);
            }
            var intake = gastgezinInDb.IntakeFormulier;
            if (intake != null && deleteForms)
            {
                intake.Deleted = true;
                intake.Deleted = true;
                if (intake.Comments == null)
                    intake.Comments = new List<Comment>();
                intake.Comments.Add(new Comment(comment, user, CommentType.DELETION));
                ReactieRepository.Update(intake);
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
