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
        private readonly IRepository<Gastgezin> GastgezinRepository;
        private readonly IRepository<Plaatsing> PlaatsingsRepository;
        private readonly IRepository<Reactie> ReactieRepository;
        private readonly IPlaatsingenService _plaatsingenService;

        public GastgezinService(IRepository<Gastgezin> gastgezinRepository, IRepository<Plaatsing> plaatsingsRepository, IRepository<Reactie> reactieRepository, IPlaatsingenService plaatsingenService)
        {
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

        public Gastgezin UpdateGastgezin(Gastgezin gastgezin, int? id)
        {
            GastgezinRepository.Update(gastgezin);
            return gastgezin;
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
