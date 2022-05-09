﻿using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Models.Models;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult Gastgezin(int id)
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

            var viewModel = new GastgezinViewModel() { };
            if (gastGezin.Contact != null)
            {
                var contact = gastGezin.Contact;
                var adres = gastGezin.Contact.Adres;
                var adresText = "";
                var woonplaatsText = "";

                if (adres != null)
                {
                    adresText = adres.Straat;
                    woonplaatsText = adres.Woonplaats;
                }

                int aanmeldFormulierId = 0;
                int intakeFormulierId = 0;

                if (gastGezin.AanmeldFormulier != null)
                {
                    aanmeldFormulierId = gastGezin.AanmeldFormulier.Id;
                }

                if (gastGezin.IntakeFormulier != null)
                {
                    intakeFormulierId = gastGezin.IntakeFormulier.Id;
                }

                var plaatsingsInfo = new PlaatsingsInfo();

                if (gastGezin.PlaatsingsInfo != null)
                {
                    plaatsingsInfo = gastGezin.PlaatsingsInfo;
                }

                var user = GetUser();
                if (!gastGezin.BekekenDoorBuddy && user?.Id == gastGezin.Buddy?.Id)
                {
                    gastGezin.BekekenDoorBuddy = true;
                    _gastgezinService.UpdateGastgezin(gastGezin, gastGezin.Id);
                }

                if (!gastGezin.BekekenDoorIntaker &&  user?.Id == gastGezin.Begeleider?.Id)
                {
                    gastGezin.BekekenDoorIntaker = true;
                    _gastgezinService.UpdateGastgezin(gastGezin, gastGezin.Id);
                }

                int? maxAdults = 0;
                if (gastGezin.MaxAdults != null)
                {
                    maxAdults = gastGezin.MaxAdults;
                }

                int? maxChildren = 0;
                if (gastGezin.MaxChildren != null)
                {
                    maxChildren = gastGezin.MaxChildren;
                }

                viewModel.GastGezin = new GastGezin()
                {
                    Id = id,
                    Adres = adresText,
                    Email = contact.Email,
                    Naam = contact.Naam,
                    Telefoonnummer = contact.Telefoonnummer,
                    Woonplaats = woonplaatsText,
                    AanmeldFormulierId = aanmeldFormulierId,
                    IntakeFormulierId = intakeFormulierId,
                    Note = gastGezin.Note,
                    Status = gastGezin.Status,
                    HasVOG = gastGezin.HasVOG,
                    PlaatsingsInfo = plaatsingsInfo,
                    MaxAdults = maxAdults,
                    MaxChildren = maxChildren
                };
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

        public IActionResult UpdatePlaatsing()
        {
            return View();
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

            ICollection<Gastgezin> gastGezinnen = gastgezinQuery.ToList();

            foreach (var gastGezin in gastGezinnen)
            {
                if (gastGezin.Contact == null)
                {
                    continue;
                }

                var contact = gastGezin.Contact;
                var adres = gastGezin.Contact.Adres;
                var adresText = "";
                var woonplaatsText = "";

                if (adres != null)
                {
                    adresText = adres.Straat;
                    woonplaatsText = adres.Woonplaats;
                }
                var aanmeldFormulierId = -1;
                var intakeFormulierId = -1;
                if (gastGezin.AanmeldFormulier != null)
                    aanmeldFormulierId = gastGezin.AanmeldFormulier.Id;
                if (gastGezin.IntakeFormulier != null)
                    intakeFormulierId = gastGezin.IntakeFormulier.Id;

                var begeleider = "";
                if (gastGezin.Begeleider != null)
                {
                    begeleider = $"{gastGezin.Begeleider.FirstName} {gastGezin.Begeleider.LastName} ({gastGezin.Begeleider.Email})";
                }
                var buddy = "";
                if (gastGezin.Buddy != null)
                {
                    buddy = $"{gastGezin.Buddy.FirstName} {gastGezin.Buddy.LastName} ({gastGezin.Buddy.Email})";
                }

                model.MijnGastgezinnen.Add(new GastGezin
                {
                    Id = gastGezin.Id,
                    Adres = adresText,
                    Email = contact.Email,
                    Naam = contact.Naam + " " + contact.Achternaam,
                    Telefoonnummer = contact.Telefoonnummer,
                    Woonplaats = woonplaatsText,
                    Begeleider = begeleider,
                    Buddy = buddy,
                    PlaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing),
                    ReserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering),
                    PlaatsingsInfo = gastGezin.PlaatsingsInfo,
                    HasVOG = gastGezin.HasVOG,
                    AanmeldFormulierId = aanmeldFormulierId,
                    IntakeFormulierId = intakeFormulierId,
                    Note = gastGezin.Note,
                    MaxAdults = gastGezin.MaxAdults,
                    MaxChildren = gastGezin.MaxChildren
                });
            }
            if (sortOrder == "Ascending")
            {
                switch (sortBy)
                {
                    case "Woonplaats":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.Woonplaats).ToList();
                        model.SortDropdownText = "Woonplaats";
                        break;
                    case "Naam":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.Naam).ToList();
                        model.SortDropdownText = "Naam";
                        break;
                    case "Geplaatst":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag).ToList();
                        model.SortDropdownText = "Geplaatst (laag-hoog)";
                        break;
                    case "Gereserveerd":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.PlaatsingTag).ToList();
                        model.SortDropdownText = "Gereserveerd (laag-hoog)";
                        break;
                    case "AanmeldingsId":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.AanmeldFormulierId).ToList();
                        model.SortDropdownText = "AanmeldingsId (laag-hoog)";
                        break;
                    case "IntakeId":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderBy(g => g.IntakeFormulierId).ToList();
                        model.SortDropdownText = "IntakeId (laag-hoog)";
                        break;
                }
            }
            else if (sortOrder == "Descending")
            {
                switch (sortBy)
                {
                    case "Geplaatst":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderByDescending(g => g.PlaatsingTag).ToList();
                        model.SortDropdownText = "Geplaatst (hoog-laag)";
                        break;
                    case "Gereserveerd":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderByDescending(g => g.PlaatsingTag).ToList();
                        model.SortDropdownText = "Gereserveerd (hoog-laag)";
                        break;
                    case "AanmeldingsId":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderByDescending(g => g.AanmeldFormulierId).ToList();
                        model.SortDropdownText = "AanmeldingsId (hoog-laag)";
                        break;
                    case "IntakeId":
                        model.MijnGastgezinnen = model.MijnGastgezinnen.OrderByDescending(g => g.IntakeFormulierId).ToList();
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

        [Authorize(Policy = "RequireSuperAdminRole")]
        [Route("{controller=Home}/{action=Index}/{id?}")]
        [HttpDelete]
        public IActionResult DeleteGastgezin(int id)
        {
            try
            {
                _gastgezinService.Delete(id);
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
    }
}
