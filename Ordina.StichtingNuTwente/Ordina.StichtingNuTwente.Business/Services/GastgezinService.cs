using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
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
        public BeschikbareGastgezinnenModel BeschikBeschikbareGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string[]? filters = null, string statusFilter = "", UserDetails? user = null)
        {
            var model = new BeschikbareGastgezinnenModel();

            var gastgezinQuery = GetAllGastgezinnen().Where(g => g.IntakeFormulier != null);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                gastgezinQuery = BeschikbareGastgezinnenStatusFilter(statusFilter, gastgezinQuery);
            }

            if (filters != null && filters.Length > 0)
            {
                var originalQuery = gastgezinQuery;
                foreach (var filterParameter in filters)
                {
                    var split = filterParameter.Split('=');
                    if (split.Length > 1)
                    {
                        var filterKey = split[0];
                        var filterValue = split[1].ToLower();
                        var results = 0;
                        switch (filterKey)
                        {
                            case "Notitie":
                                gastgezinQuery = gastgezinQuery.Where(g => g.Note != null && g.Note.ToLower().Contains(filterValue));
                                results = originalQuery.Count(g => g.Note != null && g.Note.ToLower().Contains(filterValue));
                                break;
                            case "Opmerkingen":
                                gastgezinQuery = gastgezinQuery.Where(g => g.VrijwilligerOpmerkingen != null && g.VrijwilligerOpmerkingen.ToLower().Contains(filterValue));
                                results = originalQuery.Count(g => g.VrijwilligerOpmerkingen != null && g.VrijwilligerOpmerkingen.ToLower().Contains(filterValue));
                                break;
                            case "Buddy":
                                gastgezinQuery = gastgezinQuery.Where(g => g.Buddy != null && (g.Buddy.FirstName.Contains(filterValue, StringComparison.CurrentCultureIgnoreCase)));
                                results = originalQuery.Count(g => g.Buddy != null && (g.Buddy.FirstName.Contains(filterValue, StringComparison.CurrentCultureIgnoreCase)));
                                break;
                            default:
                                gastgezinQuery = gastgezinQuery.Where(g => g.PlaatsingsInfo?.GetValueByFieldString(filterKey)?.ToLower().Contains(filterValue) == true);
                                results = originalQuery.Count(g => g.PlaatsingsInfo?.GetValueByFieldString(filterKey)?.ToLower().Contains(filterValue) == true);
                                break;
                        }
                        model.SearchQueries.Add(new SearchQueryViewModel() { OriginalQuery = filterParameter, Field = filterKey, SearchQuery = filterValue, Results = results });
                    }
                }
            }

            var gastGezinnen = gastgezinQuery;

            foreach (var gastGezin in gastGezinnen)
            {
                var plaatsingTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Plaatsing, gastGezin);
                var reserveTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                model.MijnGastgezinnen.Add(gastgezinViewModel);
            }
            if (sortOrder == "Ascending")
            {
                model = BeschikbareGastgezinnenSortByAscending(sortBy, model);
            }
            else if (sortOrder == "Descending")
            {
                model = BeschikbareGastgezinnenSortByDescending(sortBy, model);
            }
            model.TotalPlaatsingTag = _plaatsingenService.GetPlaatsingenTag(gastGezinnen.ToList(), PlacementType.Plaatsing);
            model.TotalResTag = _plaatsingenService.GetPlaatsingenTag(gastGezinnen.ToList(), PlacementType.Reservering);
            model.TotalMaxAdults = gastGezinnen.Sum(g => g.MaxOlderThanTwo);
            model.TotalMaxChildren = gastGezinnen.Sum(g => g.MaxYoungerThanThree);
            return model;
        }

        public AlleGastgezinnenModel AlleGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string statusFilter = "", UserDetails? user = null, List<UserDetails> vrijwilligers = null)
        {
            var alleGastgezinnen = new AlleGastgezinnenModel();

            foreach (var vrijwilliger in vrijwilligers.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                alleGastgezinnen.Vrijwilligers.Add(new Vrijwilliger(vrijwilliger));
            }
            IEnumerable<Gastgezin> gastGezinnen = GetAllGastgezinnen();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                gastGezinnen = BeschikbareGastgezinnenStatusFilter(statusFilter, gastGezinnen);
            }

            foreach (var gastGezin in gastGezinnen)
            {
                if (gastGezin.Contact == null)
                {
                    continue;
                }
                var plaatsingTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Plaatsing, gastGezin);
                var reserveTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                alleGastgezinnen.Gastgezinnen.Add(gastgezinViewModel);
            }

            if (sortOrder == "Ascending")
            {
                alleGastgezinnen = AlleGastgezinnenSortByAscending(sortBy, alleGastgezinnen);
            }
            else if (sortOrder == "Descending")
            {
                alleGastgezinnen = AlleGastgezinnenSortByDescending(sortBy, alleGastgezinnen);
            }

            return alleGastgezinnen;
        }

        private MijnGastgezinnenModel FillMijnGastgezinnenModel(string? filter, UserDetails user, bool? editAddress)
        {
            ICollection<Gastgezin> gastGezinnen = GetGastgezinnenForVrijwilliger(user.Id);
            var intakerCount = gastGezinnen.Where(g => g.Intaker == user).Count();
            var buddyCount = gastGezinnen.Where(g => g.Buddy == user).Count();
            var mijnGastgezinnen = new MijnGastgezinnenModel(user, intakerCount, buddyCount);

            if (filter != null)
            {
                if (filter == "Buddy")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Buddy == user).ToList();
                }
                if (filter == "Intaker")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Intaker == user).ToList();
                }
            }

            foreach (var gastGezin in gastGezinnen)
            {
                var plaatsingTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Plaatsing, gastGezin);
                var reserveTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                mijnGastgezinnen.MijnGastgezinnen.Add(gastgezinViewModel);
            }
            if (editAddress != null)
            {
                mijnGastgezinnen.EditAddress = (bool)editAddress;
            }

            FillBaseModel(mijnGastgezinnen, user);
            return mijnGastgezinnen;
        }
        public void FillBaseModel(BaseModel model, UserDetails user)
        {
            if (user == null || user.Roles == null) return;

            model.IsSecretariaat = user.Roles.Contains("group-secretariaat");
            model.IsVrijwilliger = user.Roles.Contains("group-vrijwilliger");
        }
        public bool IntakerOrBuddyChange(List<IntakerOrBuddyChangeModel> intakerOrBuddyChangeModels, List<UserDetails> vrijwilligers)
        {
            foreach (var intakerOrBuddyChange in intakerOrBuddyChangeModels)
            {
                if (int.TryParse(intakerOrBuddyChange.Id, out int gastgezinId))
                {
                    var gastgezin = GetGastgezin(gastgezinId);
                    if (gastgezin == null)
                        return false;

                    if (intakerOrBuddyChange.BuddyId != null)
                    {
                        if (intakerOrBuddyChange.BuddyId == "-")
                        {
                            gastgezin.Buddy = null;
                        }
                        else
                        {
                            if (int.TryParse(intakerOrBuddyChange.BuddyId, out int buddyId))
                            {
                                var buddy = vrijwilligers.FirstOrDefault(v => v.Id == buddyId);
                                if (buddy != null && buddy.Id != gastgezin.Buddy?.Id)
                                {
                                    gastgezin.BekekenDoorBuddy = false;
                                    gastgezin.Buddy = buddy;
                                }
                            }
                        }
                    }

                    if (intakerOrBuddyChange.IntakerId != null)
                    {
                        if (intakerOrBuddyChange.IntakerId == "-")
                        {
                            gastgezin.Intaker = null;
                        }
                        else
                        {
                            if (int.TryParse(intakerOrBuddyChange.IntakerId, out int intakerId))
                            {
                                var intaker = vrijwilligers.FirstOrDefault(v => v.Id == intakerId);
                                if (intaker != null && intaker.Id != gastgezin.Intaker?.Id)
                                {
                                    gastgezin.BekekenDoorIntaker = false;
                                    gastgezin.Intaker = intaker;
                                }
                            }
                        }
                    }
                    UpdateGastgezin(gastgezin, gastgezinId);
                }
            }
            return true;
        }

        private IEnumerable<Gastgezin> BeschikbareGastgezinnenStatusFilter(string statusFilter, IEnumerable<Gastgezin>? gastGezinnen)
        {
            switch (statusFilter)
            {
                case "Beschikbaar":
                    gastGezinnen = gastGezinnen.Where(g => !g.NoodOpvang && g.Status == GastgezinStatus.Bezocht);
                    break;
                case "Gereserveerd":
                    gastGezinnen = gastGezinnen.Where(g => g.Status == GastgezinStatus.Gereserveerd);
                    break;
                case "Geplaatst":
                    gastGezinnen = gastGezinnen.Where(g => g.Status == GastgezinStatus.Geplaatst);
                    break;
                case "Nood":
                    gastGezinnen = gastGezinnen.Where(g => g.NoodOpvang);
                    break;
                case "On Hold":
                    gastGezinnen = gastGezinnen.Where(g => g.OnHold);
                    break;
                case "Geen Intaker":
                    gastGezinnen = gastGezinnen.Where(g => g.Intaker == null);
                    break;
                case "Geen Buddy":
                    gastGezinnen = gastGezinnen.Where(g => g.Buddy == null);
                    break;
            }
            return gastGezinnen;
        }

        private BeschikbareGastgezinnenModel BeschikbareGastgezinnenSortByAscending(string sortBy, BeschikbareGastgezinnenModel model)
        {
            switch (sortBy)
            {
                case "Woonplaats":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "Woonplaats";
                    break;
                case "Naam":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenBy(g => g.Naam).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "Naam";
                    break;
                case "Geplaatst":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenBy(g => g.PlaatsingTag).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "Geplaatst (laag-hoog)";
                    break;
                case "Gereserveerd":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.ReserveTag.Contains("HOLD")).ThenBy(g => g.ReserveTag).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "Gereserveerd (laag-hoog)";
                    break;
                case "AanmeldingsId":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenBy(g => g.AanmeldFormulierId == null).ThenBy(g => g.AanmeldFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "AanmeldingsId (laag-hoog)";
                    break;
                case "IntakeId":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenBy(g => g.IntakeFormulierId == null).ThenBy(g => g.IntakeFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "IntakeId (laag-hoog)";
                    break;
            }
            return model;
        }

        private BeschikbareGastgezinnenModel BeschikbareGastgezinnenSortByDescending(string sortBy, BeschikbareGastgezinnenModel model)
        {
            switch (sortBy)
            {
                case "Geplaatst":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenByDescending(g => g.PlaatsingTag).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "Geplaatst (hoog-laag)";
                    break;
                case "Gereserveerd":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenByDescending(g => g.ReserveTag).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "Gereserveerd (hoog-laag)";
                    break;
                case "AanmeldingsId":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenBy(g => g.AanmeldFormulierId == null).ThenByDescending(g => g.AanmeldFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "AanmeldingsId (hoog-laag)";
                    break;
                case "IntakeId":
                    model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag.Contains("HOLD")).ThenBy(g => g.IntakeFormulierId == null).ThenByDescending(g => g.IntakeFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    model.SortDropdownText = "IntakeId (hoog-laag)";
                    break;
            }
            return model;
        }

        private AlleGastgezinnenModel AlleGastgezinnenSortByAscending(string sortBy, AlleGastgezinnenModel alleGastgezinnen)
        {
            switch (sortBy)
            {
                case "Woonplaats":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "Woonplaats";
                    break;
                case "Naam":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.Naam).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "Naam";
                    break;
                case "Telefoonnummer":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.Telefoonnummer).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "Telefoonnummer";
                    break;
                case "Intaker":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.Intaker).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "Intaker (laag-hoog)";
                    break;
                case "Buddy":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.Buddy).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "Buddy (laag-hoog)";
                    break;
                case "AanmeldingsId":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.AanmeldFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "AanmeldingsId (laag-hoog)";
                    break;
                case "IntakeId":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.IntakeFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "IntakeId (laag-hoog)";
                    break;
            }
            return alleGastgezinnen;
        }

        private AlleGastgezinnenModel AlleGastgezinnenSortByDescending(string sortBy, AlleGastgezinnenModel alleGastgezinnen)
        {
            switch (sortBy)
            {
                case "Intaker":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderByDescending(g => g.Intaker).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "Intaker (hoog-laag)";
                    break;
                case "Buddy":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderByDescending(g => g.Buddy).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "Buddy (hoog-laag)";
                    break;
                case "AanmeldingsId":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderByDescending(g => g.AanmeldFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "AanmeldingsId (hoog-laag)";
                    break;
                case "IntakeId":
                    alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderByDescending(g => g.IntakeFormulierId).ThenBy(g => g.Woonplaats).ToList();
                    alleGastgezinnen.SortDropdownText = "IntakeId (hoog-laag)";
                    break;
            }
            return alleGastgezinnen;
        }
    }
}
