using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Ordina.StichtingNuTwente.Models.Mappings;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireVrijwilligerRole")]
    public class GastgezinController : Controller
    {
        private readonly IGastgezinService _gastgezinService;
        private readonly IUserService _userService;

        public GastgezinController(IGastgezinService gastgezinService, IUserService userService)
        {
            _gastgezinService = gastgezinService;
            _userService = userService;
        }

        public IActionResult Overview()
        {
            return View();
        }

        [Route("gastgezin")]
        public IActionResult Gastgezin(int id, bool? EditPlaatsingen = false, bool? EditReserveringen = false)
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
            if (gastGezin.Contact != null)
            {
                var user = GetUser();
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

            if(EditPlaatsingen == true)
            {
                viewModel.EditPlaatsingen = true;
            }
            if (EditReserveringen == true)
            {
                viewModel.EditReserveringen = true;
            }

            return View(viewModel);
        }

        [ActionName("PostPlaatsing")]
        [Route("GastgezinController/PostPlaatsing")]
        public IActionResult PostPlaatsing(int GastGezinId, PlacementType PlacementType, Gender Gender, AgeGroup AgeGroup, string Date, int Age = -1, int Amount = 1)
        {
            var plaatsType = PlacementType;

            for (int i = 0; i<Amount; i++)
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
            return Redirect("/gastgezin?id=" + GastGezinId);
        }


        public IActionResult UpdatePlaatsingen(IFormCollection formCollection)
        {
            var gastgezinId = 0;
            List<Plaatsing> plaatsingen = new();
            var plaatsingsId = 0;
            Plaatsing plaatsing = new();
            var age = 0;
            Gender gender = new();
            AgeGroup ageGroup = new();
            DateTime date = new();

            foreach (var key in formCollection.Keys)
            {
                if (key.StartsWith("GastgezinId"))
                {
                    gastgezinId = int.Parse(formCollection[key]);
                }
                if (key.StartsWith("PlaatsingsId"))
                {
                    plaatsingsId = int.Parse(formCollection[key]);
                    plaatsing = _gastgezinService.GetPlaatsing(plaatsingsId);
                }
                if (key.StartsWith("Date"))
                {
                    date = DateTime.Parse(formCollection[key]);
                }
                if (key.StartsWith("Gender"))
                {
                    gender = Enum.Parse<Gender>(formCollection[key]);
                }
                if (key.StartsWith("Age_"))
                {
                    age = int.Parse(formCollection[key]);
                }
                if (key.StartsWith("AgeGroup"))
                {
                    ageGroup = Enum.Parse<AgeGroup>(formCollection[key]);
                    
                    plaatsing.DateTime = date;
                    plaatsing.Gender = gender;
                    plaatsing.Age = age;
                    plaatsing.AgeGroup = ageGroup;
                    _gastgezinService.UpdatePlaatsing(plaatsing);
                }
            }
            return Redirect("/gastgezin?id=" + gastgezinId);
        }

        [Route("DeletePlaatsing")]
        public IActionResult DeletePlaatsing(int plaatsingId)
        {
            var plaatsing = _gastgezinService.GetPlaatsing(plaatsingId);
            plaatsing.Active = false;
            _gastgezinService.UpdatePlaatsing(plaatsing);
            var placementType = PlacementType.VerwijderdePlaatsing;
            if(plaatsing.PlacementType == PlacementType.Reservering)
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
                DateTime = DateTime.Now,
                Vrijwilliger = _userService.getUserFromClaimsPrincipal(User),
                Active = false,
                Gender = plaatsing.Gender
            };
            _gastgezinService.AddPlaatsing(deletedPlaatsing);
            return Redirect("/gastgezin?id=" + plaatsing.Gastgezin.Id);
        }

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
        public IActionResult UpdateOpties(int GastGezinId, GastgezinStatus Status, bool HasVOG, int MaxAdults, int MaxChildren)
        {
            var gastgezin = _gastgezinService.GetGastgezin(GastGezinId);
            if (gastgezin != null)
            {
                gastgezin.Status = Status;
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
        public IActionResult BeschikbareGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string[]? filters = null)
        {
            _userService.checkIfUserExists(User);

            var model = new BeschikbareGastgezinnenModel();

            var gastgezinQuery = _gastgezinService.GetAllGastgezinnen().Where(g => g.IntakeFormulier != null);

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

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("{controller=Home}/{action=Index}/{id?}")]
        [HttpDelete]
        public IActionResult DeleteGastgezin(int id, bool deleteForms = false)
        {
            try
            {
                _gastgezinService.Delete(id, deleteForms);
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

        [Authorize(Policy = "RequireVrijwilligerRole")]
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

        [Authorize(Policy = "RequireVrijwilligerRole")]
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

        [Authorize(Policy = "RequireVrijwilligerRole")]
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
        public IActionResult AlleGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending")
        {
            _userService.checkIfUserExists(User);
            
            var alleGastgezinnen = new AlleGastgezinnenModel();

            var vrijwilligers = _userService.GetAllDropdownUsers().OrderBy(u => u.FirstName).ToList();
            foreach (var vrijwilliger in vrijwilligers.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                alleGastgezinnen.Vrijwilligers.Add(new Vrijwilliger
                {
                    Id = vrijwilliger.Id,
                    Naam = $"{vrijwilliger.FirstName} {vrijwilliger.LastName}",
                    Email = vrijwilliger.Email
                });
            }
            var user = GetUser();
            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetAllGastgezinnen();

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

        [Authorize(Policy = "RequireSecretariaatRole")]
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
        }

        public IActionResult EditPlaatsing(int GastgezinId, int PlaatsingsId)
        {
            return Redirect("/gastgezin?id=" + GastgezinId);
        }
    }
}
