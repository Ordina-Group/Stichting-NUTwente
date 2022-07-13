using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Business.Helpers;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireVrijwilligerRole")]
    public class GastgezinController : Controller
    {
        private readonly IGastgezinService _gastgezinService;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private MailHelper mailHelper;

        public GastgezinController(IGastgezinService gastgezinService, IUserService userService, IMailService mailService)
        {
            _gastgezinService = gastgezinService;
            _userService = userService;
            _mailService = mailService;

            mailHelper = new MailHelper(_mailService);
        }

        public IActionResult Overview()
        {
            return View();
        }

        [Route("gastgezin")]
        public IActionResult Gastgezin(int id, bool? EditPlaatsingen = false, bool? EditReserveringen = false, bool? EditVerwijderdePlaatsingen = false)
        {
            _userService.checkIfUserExists(User);
            var gastGezin = _gastgezinService.GetGastgezin(id);
            if (gastGezin == null)
            {
                return Redirect("Error");
            }
            if (gastGezin.Begeleider != null || gastGezin.Buddy != null)
            {
                if (!(gastGezin.Begeleider?.AADId == _userService.getUserFromClaimsPrincipal(User).AADId || gastGezin.Buddy?.AADId == _userService.getUserFromClaimsPrincipal(User).AADId || User.HasClaims("groups", "group-secretariaat", "group-coordinator", "group-superadmin")))
                {
                    return Redirect("MicrosoftIdentity/Account/AccessDenied");
                }
            }
            else if (!User.HasClaims("groups", "group-secretariaat", "group-coordinator", "group-superadmin"))
            {
                return Redirect("MicrosoftIdentity/Account/AccessDenied");
            }

            var viewModel = new GastgezinDetailViewModel() { };
            var user = GetUser();
            if (gastGezin.Contact != null)
            {
                var plaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing);
                var reserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                viewModel.GastGezin = gastgezinViewModel;
            }
            var PlacementViewModels = new List<PlaatsingViewModel>();
            gastGezin.Plaatsingen.ToList().ForEach(p => PlacementViewModels.Add(new PlaatsingViewModel(p)));

            viewModel.Plaatsingen = PlacementViewModels.Where(p => p.PlacementType == PlacementType.Plaatsing && p.Active == true).ToList();
            viewModel.Plaatsingen.AddRange(PlacementViewModels.Where(p => p.PlacementType == PlacementType.GeplaatsteReservering && p.Active == true).ToList());
            viewModel.Reserveringen = PlacementViewModels.Where(p => p.PlacementType == PlacementType.Reservering && p.Active == true).ToList();

            viewModel.PlaatsingsGeschiedenis = new List<PlaatsingViewModel>();
            gastGezin.Plaatsingen.ToList().ForEach(p => viewModel.PlaatsingsGeschiedenis.Add(new PlaatsingViewModel(p)));
            viewModel.PlaatsingsGeschiedenis = viewModel.PlaatsingsGeschiedenis.OrderByDescending(p => p.Id).ToList();

            viewModel.PlaatsingDTO = new PlaatsingDTO();

            viewModel.PlaatsingStats = new PlaatsingStats();
            viewModel.PlaatsingStats.PlaatsVolwassen = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.PlaatsKind = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.PlaatsOnbekend = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResVolwassen = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResKind = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResOnbekend = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);

            if (EditPlaatsingen == true)
            {
                viewModel.EditPlaatsingen = true;
            }
            if (EditReserveringen == true)
            {
                viewModel.EditReserveringen = true;
            }
            if (EditVerwijderdePlaatsingen == true)
            {
                viewModel.EditVerwijderdePlaatsingen = true;
            }
            var u = GetUser();
            viewModel.CanDelete = false;
            if (User.HasClaims("groups", "group-coordinator"))
                viewModel.CanDelete = true;
            return View(viewModel);
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [ActionName("PostPlaatsing")]
        [Route("GastgezinController/PostPlaatsing")]
        public IActionResult PostPlaatsing(int GastGezinId, PlacementType PlacementType, Gender Gender, AgeGroup AgeGroup, string Date, int Age = -1, int Amount = 1)
        {
            var plaatsType = PlacementType;

            for (int i = 0; i < Amount; i++)
            {
                var plaatsing = new Plaatsing()
                {
                    Gastgezin = _gastgezinService.GetGastgezin(GastGezinId),
                    Amount = 1,
                    Age = Age,
                    AgeGroup = AgeGroup,
                    PlacementType = plaatsType,
                    DateTime = DateTime.Parse(Date),
                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User),
                    Active = true,
                    Gender = Gender
                };
                _gastgezinService.AddPlaatsing(plaatsing);
            }


            //TODO: zorgen dat er niet per losse plaatsing een mail wordt verstuurd
            //mailHelper.PlaatsingVluchteling(plaatsing);

            return Redirect("/gastgezin?id=" + GastGezinId);
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        public IActionResult UpdatePlaatsingen(IFormCollection formCollection)
        {
            string gastgezinIdString = formCollection.FirstOrDefault(k => k.Key.StartsWith("GastgezinId")).Value;

            if (Int32.TryParse(gastgezinIdString, out int gastgezinId))
            {
                var plaatsingIds = formCollection.Where(k => k.Key.StartsWith("PlaatsingsId")).Select(k => k.Value);

                foreach (var id in plaatsingIds)
                {
                    if (int.TryParse(id, out int plaatsingId))
                    {
                        var plaatsing = _gastgezinService.GetPlaatsing(plaatsingId);

                        if (plaatsing != null)
                        {
                            var plaatsingData = formCollection.Where(k => k.Key.EndsWith(id));

                            foreach (var keyValuePair in plaatsingData)
                            {
                                var key = keyValuePair.Key;
                                var value = keyValuePair.Value;

                                if (key.StartsWith("Date") && DateTime.TryParse(value, out DateTime dateTime))
                                {
                                    plaatsing.DateTime = dateTime;
                                }
                                else if (key.StartsWith("Gender") && Enum.TryParse(value, out Gender gender))
                                {
                                    plaatsing.Gender = gender;
                                }
                                else if (key.StartsWith("Age_") && int.TryParse(value, out int age))
                                {
                                    plaatsing.Age = age;
                                }
                                else if (key.StartsWith("AgeGroup") && Enum.TryParse(value, out AgeGroup ageGroup))
                                {
                                    plaatsing.AgeGroup = ageGroup;
                                }
                                else if (key.StartsWith("DepartureReason"))
                                {
                                    plaatsing.DepartureReason = value;
                                }
                                else if (key.StartsWith("DepartureComment"))
                                {
                                    plaatsing.DepartureComment = value;
                                }
                                else if (key.StartsWith("DepartureDestination") && Enum.TryParse(value, out DepartureDestination departureDestination))
                                {
                                    plaatsing.DepartureDestination = departureDestination;
                                }
                            }
                            _gastgezinService.UpdatePlaatsing(plaatsing);
                        }
                    }
                }
                return Redirect("/gastgezin?id=" + gastgezinId);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("DeletePlaatsing")]
        public IActionResult DeletePlaatsing(DateTime departureDate, int plaatsingId, string departureReason, DepartureDestination departureDestination, string departureComment)
        {
            var plaatsing = _gastgezinService.GetPlaatsing(plaatsingId);
            plaatsing.Active = false;
            _gastgezinService.UpdatePlaatsing(plaatsing);
            var placementType = PlacementType.VerwijderdePlaatsing;
            if (plaatsing.PlacementType == PlacementType.Reservering)
            {
                placementType = PlacementType.VerwijderdeReservering;
            }
            var deletedPlaatsing = new Plaatsing()
            {
                Gastgezin = plaatsing.Gastgezin,
                Amount = plaatsing.Amount,
                Age = plaatsing.Age,
                AgeGroup = plaatsing.AgeGroup,
                PlacementType = placementType,
                DateTime = departureDate,
                Vrijwilliger = _userService.getUserFromClaimsPrincipal(User),
                Active = false,
                Gender = plaatsing.Gender,
                DepartureReason = departureReason,
                DepartureDestination = departureDestination,
                DepartureComment = departureComment
            };
            _gastgezinService.AddPlaatsing(deletedPlaatsing);
            return Redirect("/gastgezin?id=" + plaatsing.Gastgezin.Id);
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("PlaatsReservering")]
        public IActionResult PlaatsReservering(int plaatsingId)
        {
            var plaatsing = _gastgezinService.GetPlaatsing(plaatsingId);
            plaatsing.Active = false;
            _gastgezinService.UpdatePlaatsing(plaatsing);
            var NieuwePlaatsing = new Plaatsing()
            {
                Gastgezin = plaatsing.Gastgezin,
                Amount = plaatsing.Amount,
                Age = plaatsing.Age,
                AgeGroup = plaatsing.AgeGroup,
                PlacementType = PlacementType.GeplaatsteReservering,
                DateTime = DateTime.Now,
                Vrijwilliger = _userService.getUserFromClaimsPrincipal(User),
                Active = true,
                Gender = plaatsing.Gender
            };
            _gastgezinService.AddPlaatsing(NieuwePlaatsing);
            return Redirect("/gastgezin?id=" + plaatsing.Gastgezin.Id);
        }

        public IActionResult PostNote(int GastGezinId, string Note)
        {
            _gastgezinService.UpdateNote(GastGezinId, Note);
            return Redirect("/gastgezin?id=" + GastGezinId);
        }

        [HttpPost]
        public IActionResult PostVOG(bool HasVOG, int GastGezinId)
        {
            _gastgezinService.UpdateVOG(HasVOG, GastGezinId);
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateOpties(int GastGezinId, bool NoodOpvang, bool OnHold, bool HasVOG, int MaxAdults, int MaxChildren)
        {
            var gastgezin = _gastgezinService.GetGastgezin(GastGezinId);
            if (gastgezin != null)
            {
                gastgezin.NoodOpvang = NoodOpvang;
                gastgezin.OnHold = OnHold;
                gastgezin.HasVOG = HasVOG;
                gastgezin.MaxAdults = MaxAdults;
                gastgezin.MaxChildren = MaxChildren;
                _gastgezinService.UpdateGastgezin(gastgezin, GastGezinId);
                return Redirect("/gastgezin?id=" + GastGezinId);
            }
            else return Redirect("Error");
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("/BeschikbareGastgezinnen")]
        [HttpGet]
        public IActionResult BeschikbareGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string[]? filters = null, string statusFilter = "")
        {
            _userService.checkIfUserExists(User);

            var model = new BeschikbareGastgezinnenModel();

            var gastgezinQuery = _gastgezinService.GetAllGastgezinnen().Where(g => g.IntakeFormulier != null);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter)
                {
                    case "Beschikbaar":
                        gastgezinQuery = gastgezinQuery.Where(g => !g.NoodOpvang && g.Status == GastgezinStatus.Bezocht);
                        break;
                    case "Gereserveerd":
                        gastgezinQuery = gastgezinQuery.Where(g => g.Status == GastgezinStatus.Gereserveerd);
                        break;
                    case "Geplaatst":
                        gastgezinQuery = gastgezinQuery.Where(g => g.Status == GastgezinStatus.Geplaatst);
                        break;
                    case "Nood":
                        gastgezinQuery = gastgezinQuery.Where(g => g.NoodOpvang);
                        break;
                    case "On Hold":
                        gastgezinQuery = gastgezinQuery.Where(g => g.OnHold);
                        break;
                }
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
                        if (filterKey == "Notitie")
                        {
                            gastgezinQuery = gastgezinQuery.Where(g => g.Note != null && g.Note.ToLower().Contains(filterValue));
                            results = originalQuery.Count(g => g.Note != null && g.Note.ToLower().Contains(filterValue));
                        }
                        else if (filterKey == "Opmerkingen")
                        {
                            gastgezinQuery = gastgezinQuery.Where(g => g.VrijwilligerOpmerkingen != null && g.VrijwilligerOpmerkingen.ToLower().Contains(filterValue));
                            results = originalQuery.Count(g => g.VrijwilligerOpmerkingen != null && g.VrijwilligerOpmerkingen.ToLower().Contains(filterValue));
                        }
                        else
                        {
                            gastgezinQuery = gastgezinQuery.Where(g => g.PlaatsingsInfo?.GetValueByFieldString(filterKey)?.ToLower().Contains(filterValue) == true);
                            results = originalQuery.Count(g => g.PlaatsingsInfo?.GetValueByFieldString(filterKey)?.ToLower().Contains(filterValue) == true);
                        }
                        model.SearchQueries.Add(new SearchQueryViewModel() { OriginalQuery = filterParameter, Field = filterKey, SearchQuery = filterValue, Results = results });
                    }
                }
            }

            var gastGezinnen = gastgezinQuery;
            var user = GetUser();

            foreach (var gastGezin in gastGezinnen)
            {
                var plaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing, gastGezin);
                var reserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                model.MijnGastgezinnen.Add(gastgezinViewModel);
            }
            if (sortOrder == "Ascending")
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
            }
            else if (sortOrder == "Descending")
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
            }
            model.TotalPlaatsingTag = _gastgezinService.GetPlaatsingenTag(gastGezinnen.ToList(), PlacementType.Plaatsing);
            model.TotalResTag = _gastgezinService.GetPlaatsingenTag(gastGezinnen.ToList(), PlacementType.Reservering);
            model.TotalMaxAdults = gastGezinnen.Sum(g => g.MaxAdults);
            model.TotalMaxChildren = gastGezinnen.Sum(g => g.MaxChildren);
            FillBaseModel(model);
            return View(model);
        }

        public void FillBaseModel(BaseModel model)
        {
            var user = _userService.getUserFromClaimsPrincipal(User);

            if (user == null || user.Roles == null) return;

            model.IsSecretariaat = user.Roles.Contains("group-secretariaat");
            model.IsVrijwilliger = user.Roles.Contains("group-vrijwilliger");
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("{controller=Home}/{action=Index}/{id?}")]
        [HttpDelete]
        public IActionResult DeleteGastgezin(int id, string comment, bool deleteForms = true)
        {
            try
            {
                var userDetails = GetUser();
                if (userDetails != null)
                {
                    _gastgezinService.Delete(id, deleteForms, userDetails, comment == null ? "" : comment);
                    return Ok();
                }
                return BadRequest("No user found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("{controller=Home}/{action=Index}/{id?}")]
        [HttpPost]
        public IActionResult RestoreGastgezin(int id)
        {
            try
            {
                _gastgezinService.Restore(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public UserDetails? GetUser()
        {
            var aadID = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (aadID != null)
            {
                var userDetails = this._userService.GetUserByAADId(aadID.Value);
                return userDetails;
            }
            return null;
        }

        [HttpPut]
        [Route("MarkNewAsSeen/{gastgezinId}")]
        public IActionResult MarkNewAsSeen(int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null && gastgezin.Comments != null)
                {
                    gastgezin.Comments.RemoveAll(c => c.CommentType == CommentType.INTAKE_COMPLETED);
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPut]
        [Route("MarkAsRead/{gastgezinId}")]
        public IActionResult MarkAsRead(int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null)
                {
                    var user = GetUser();
                    if (!gastgezin.BekekenDoorIntaker && user?.Id == gastgezin.Begeleider?.Id)
                    {
                        gastgezin.BekekenDoorIntaker = true;
                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    }

                    if (!gastgezin.BekekenDoorBuddy && user?.Id == gastgezin.Buddy?.Id)
                    {
                        gastgezin.BekekenDoorBuddy = true;
                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    }
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("RejectBuddy/{gastgezinId}")]
        public IActionResult RejectBeingBuddy(int gastgezinId, string comment)
        {
            try
            {
                var user = GetUser();
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null && user != null)
                {
                    _gastgezinService.RejectBeingBuddy(gastgezin, comment, user);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("MijnGastgezinnen")]
        [HttpGet]
        [ActionName("MijnGastgezinnen")]
        public IActionResult MijnGastgezinnen(string? filter)
        {
            _userService.checkIfUserExists(User);

            var mijnGastgezinnen = new MijnGastgezinnenModel();

            var user = GetUser();
            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(user.Id);
            if (filter != null)
            {
                if (filter == "Buddy")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Buddy == user).ToList();
                }
                if (filter == "Intaker")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Begeleider == user).ToList();
                }
            }

            foreach (var gastGezin in gastGezinnen)
            {
                var plaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing, gastGezin);
                var reserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                mijnGastgezinnen.MijnGastgezinnen.Add(gastgezinViewModel);
            }
            FillBaseModel(mijnGastgezinnen);
            return View(mijnGastgezinnen);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("MijnGastgezinnen/{userId}")]
        [HttpGet]
        [ActionName("MijnGastgezinnen")]
        public IActionResult MijnGastgezinnen(string? filter, int userId)
        {
            _userService.checkIfUserExists(User);

            var mijnGastgezinnen = new MijnGastgezinnenModel();

            var user = _userService.GetUserById(userId);
            if (user == null)
                return Redirect("Error");
            mijnGastgezinnen.GastgezinnenVan = user.FirstName + " " + user.LastName;

            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(user.Id);
            if (filter != null)
            {
                if (filter == "Buddy")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Buddy == user).ToList();
                }
                if (filter == "Intaker")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Begeleider == user).ToList();
                }
            }

            foreach (var gastGezin in gastGezinnen)
            {
                var plaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing, gastGezin);
                var reserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                mijnGastgezinnen.MijnGastgezinnen.Add(gastgezinViewModel);
            }

            FillBaseModel(mijnGastgezinnen);
            return View(mijnGastgezinnen);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("AlleGastgezinnen")]
        [HttpGet]
        [ActionName("AlleGastgezinnen")]
        public IActionResult AlleGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string statusFilter = "")
        {
            _userService.checkIfUserExists(User);

            var alleGastgezinnen = new AlleGastgezinnenModel();

            var vrijwilligers = _userService.GetAllDropdownUsers().OrderBy(u => u.FirstName).ToList();
            foreach (var vrijwilliger in vrijwilligers.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                alleGastgezinnen.Vrijwilligers.Add(new Vrijwilliger
                {
                    Id = vrijwilliger.Id,
                    Naam = $"{vrijwilliger.FirstName}",
                    Email = vrijwilliger.Email
                });
            }
            var user = GetUser();
            IEnumerable<Gastgezin> gastGezinnen = _gastgezinService.GetAllGastgezinnen();

            if (!string.IsNullOrEmpty(statusFilter))
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
                        gastGezinnen = gastGezinnen.Where(g => g.Begeleider == null);
                        break;
                    case "Geen Buddy":
                        gastGezinnen = gastGezinnen.Where(g => g.Buddy == null);
                        break;
                }
            }

            foreach (var gastGezin in gastGezinnen)
            {
                if (gastGezin.Contact == null)
                {
                    continue;
                }
                var plaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing, gastGezin);
                var reserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                alleGastgezinnen.Gastgezinnen.Add(gastgezinViewModel);
            }

            if (sortOrder == "Ascending")
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
                        alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderBy(g => g.Begeleider).ThenBy(g => g.Woonplaats).ToList();
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
            }
            else if (sortOrder == "Descending")
            {
                switch (sortBy)
                {
                    case "Intaker":
                        alleGastgezinnen.Gastgezinnen = alleGastgezinnen.Gastgezinnen.OrderByDescending(g => g.Begeleider).ThenBy(g => g.Woonplaats).ToList();
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
            }

            FillBaseModel(alleGastgezinnen);
            return View(alleGastgezinnen);
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [HttpPost]
        public IActionResult IntakerOrBuddyChange(List<IntakerOrBuddyChangeModel> intakerOrBuddyChangeModels)
        {
            var vrijwilligers = _userService.GetAllDropdownUsers().ToList();
            foreach (var intakerOrBuddyChange in intakerOrBuddyChangeModels)
            {
                if (int.TryParse(intakerOrBuddyChange.Id, out int gastgezinId))
                {
                    var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                    if (gastgezin == null)
                        return BadRequest();

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
                            gastgezin.Begeleider = null;
                        }
                        else
                        {
                            if (int.TryParse(intakerOrBuddyChange.IntakerId, out int intakerId))
                            {
                                var intaker = vrijwilligers.FirstOrDefault(v => v.Id == intakerId);
                                if (intaker != null && intaker.Id != gastgezin.Begeleider?.Id)
                                {
                                    gastgezin.BekekenDoorIntaker = false;
                                    gastgezin.Begeleider = intaker;
                                }
                            }
                        }
                    }
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                }
            }
            return Ok();
        }

        /*Old method is very slow
        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("AlleGastgezinnen")]
        [HttpPost]
        [ActionName("AlleGastgezinnen")]
        public IActionResult AlleGastgezinnenPost(IFormCollection formCollection)
        {
            var vrijwilligers = _userService.GetAllDropdownUsers().OrderBy(u => u.FirstName).ToList();

            foreach (var key in formCollection.Keys)
            {
                if (key.StartsWith("vrijwilliger_"))
                {
                    var value = formCollection[key];
                    int vrijwilligerId = 0;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        vrijwilligerId = Convert.ToInt32(value);
                    }

                    var gastgezinId = Convert.ToInt32(key.Substring(13));

                    var gastgezinItem = _gastgezinService.GetGastgezin(gastgezinId);
                    if (gastgezinItem == null)
                    {
                        continue;
                    }

                    UserDetails? assignVrijwilliger = null;
                    if (vrijwilligerId > 0)
                    {
                        assignVrijwilliger = vrijwilligers.FirstOrDefault(e => e.Id == vrijwilligerId);
                    }

                    if (assignVrijwilliger == null && gastgezinItem.Begeleider != null)
                    {
                        gastgezinItem.Begeleider = null;
                        gastgezinItem.BekekenDoorIntaker = false;
                        _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                    }
                    else
                    {
                        if (gastgezinItem.Begeleider != null && gastgezinItem.Begeleider.Id != assignVrijwilliger.Id)
                        {
                            gastgezinItem.Begeleider = assignVrijwilliger;
                            gastgezinItem.BekekenDoorIntaker = false;
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                        else if (gastgezinItem.Begeleider == null)
                        {
                            gastgezinItem.Begeleider = assignVrijwilliger;
                            gastgezinItem.BekekenDoorIntaker = false;
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                    }
                }
            }

            foreach (var key in formCollection.Keys)
            {
                if (key.StartsWith("buddy_"))
                {
                    var value = formCollection[key];
                    int vrijwilligerId = 0;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        vrijwilligerId = Convert.ToInt32(value);
                    }

                    var gastgezinId = Convert.ToInt32(key.Substring(6));

                    var gastgezinItem = _gastgezinService.GetGastgezin(gastgezinId);
                    if (gastgezinItem == null)
                    {
                        continue;
                    }

                    UserDetails? assignVrijwilliger = null;
                    if (vrijwilligerId > 0)
                    {
                        assignVrijwilliger = vrijwilligers.FirstOrDefault(e => e.Id == vrijwilligerId);
                    }

                    if (assignVrijwilliger == null && gastgezinItem.Buddy != null)
                    {
                        gastgezinItem.Buddy = null;
                        gastgezinItem.BekekenDoorBuddy = false;
                        gastgezinItem.Comments?.RemoveAll(c => c.CommentType == CommentType.BUDDY_REJECTION);
                        _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                    }
                    else
                    {
                        if (gastgezinItem.Buddy != null && gastgezinItem.Buddy.Id != assignVrijwilliger.Id)
                        {
                            gastgezinItem.Buddy = assignVrijwilliger;
                            gastgezinItem.BekekenDoorBuddy = false;
                            gastgezinItem.Comments?.RemoveAll(c => c.CommentType == CommentType.BUDDY_REJECTION);
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                        else if (gastgezinItem.Buddy is null)
                        {
                            gastgezinItem.Buddy = assignVrijwilliger;
                            gastgezinItem.BekekenDoorBuddy = false;
                            gastgezinItem.Comments?.RemoveAll(c => c.CommentType == CommentType.BUDDY_REJECTION);
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                    }
                }
            }

            return RedirectToAction("AlleGastgezinnen");
        }*/
        [Authorize(Policy = "RequireCoordinatorRole")]
        public IActionResult EditPlaatsing(int GastgezinId, int PlaatsingsId)
        {
            return Redirect("/gastgezin?id=" + GastgezinId);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("VerwijderdeGastgezinnen")]
        [HttpGet]
        [ActionName("VerwijderdeGastgezinnen")]
        public IActionResult VerwijderdeGastgezinnen()
        {
            _userService.checkIfUserExists(User);

            var model = new List<GastgezinViewModel>();

            var user = GetUser();
            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetDeletedGastgezinnen();

            foreach (var gastGezin in gastGezinnen)
            {
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user);
                model.Add(gastgezinViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateCommentVrijwilliger(string comments, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null)
                {
                    gastgezin.VrijwilligerOpmerkingen = comments == null ? "" : comments;
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [HttpPost]
        public IActionResult UpdateCommentCoordinator(string comments, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null)
                {
                    gastgezin.CoordinatorOpmerkingen = comments == null ? "" : comments;
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult AddContactLog(DateTime date, string note, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                var user = GetUser();
                if (gastgezin != null && user != null)
                {
                    gastgezin.ContactLogs.Add(new ContactLog() { Notes = note, DateTime = date, Contacter = user });
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [HttpDelete]
        public IActionResult DeleteContactLog(int contactLogId, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                var user = GetUser();
                if (gastgezin != null && user != null)
                {
                    var contactLog = gastgezin.ContactLogs.FirstOrDefault(c => c.Id == contactLogId);
                    if (contactLog != null && (contactLog.Contacter.AADId == GetUser().AADId) || User.HasClaims("groups", "group-coordinator", "group-superadmin"))
                    {
                        gastgezin.ContactLogs.Remove(contactLog);
                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(403);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetGastgezinInformation()
        {
            var amountGastgezinnen = _gastgezinService.GetAllGastgezinnen().Where(g => g.Status == GastgezinStatus.Geplaatst);
            var amountGeplaatsteVluchtelingen = _gastgezinService.GetPlaatsingen().Where(p => p.Active && (p.PlacementType == PlacementType.GeplaatsteReservering || p.PlacementType == PlacementType.Plaatsing));


            var returnModel = new GastgezinStatsViewModel(amountGeplaatsteVluchtelingen.Count(), amountGastgezinnen.Count());

            return Ok(returnModel);
        }
    }
}
