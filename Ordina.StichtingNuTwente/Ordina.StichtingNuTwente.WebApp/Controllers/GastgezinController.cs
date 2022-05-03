using System.Runtime.Serialization;
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
            if (gastGezin.Begeleider != null)
            {
                if (!(gastGezin.Begeleider.AADId == _userService.getUserFromClaimsPrincipal(User).AADId || User.HasClaims("groups", "group-secretariaat", "group-coordinator", "group-superadmin")))
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
                };
            }
            viewModel.PlaatsingsGeschiedenis = new List<PlaatsingViewModel>();
            gastGezin.Plaatsingen.ToList().ForEach(p => viewModel.PlaatsingsGeschiedenis.Add(new PlaatsingViewModel(p)));
            viewModel.PlaatsingsGeschiedenis = viewModel.PlaatsingsGeschiedenis.OrderByDescending(p => p.DateTime.Ticks).ToList();

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
        public IActionResult PostPlaatsing(int GastGezinId, int PlacementType, int Volwassen, int Kind, int Onbekend)
        {
            var plaatsType = (PlacementType)PlacementType;
            var PrevVolwassen = _gastgezinService.GetPlaatsingen(GastGezinId, plaatsType, AgeGroup.Volwassene).Sum(p => p.Amount);
            var PrevKind = _gastgezinService.GetPlaatsingen(GastGezinId, plaatsType, AgeGroup.Kind).Sum(p => p.Amount);
            var PrevOnbekend = _gastgezinService.GetPlaatsingen(GastGezinId, plaatsType, AgeGroup.Onbekend).Sum(p => p.Amount);


            if (Volwassen != PrevVolwassen)
            {
                var plaatsing = new Plaatsing()
                {
                    Gastgezin = _gastgezinService.GetGastgezin(GastGezinId),
                    Amount = Volwassen - PrevVolwassen,
                    AgeGroup = AgeGroup.Volwassene,
                    PlacementType = plaatsType,
                    DateTime = DateTime.Now,
                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                };
                _gastgezinService.AddPlaatsing(plaatsing);
            }
            if (Kind != PrevKind)
            {
                var plaatsing = new Plaatsing()
                {
                    Gastgezin = _gastgezinService.GetGastgezin(GastGezinId),
                    Amount = Kind - PrevKind,
                    AgeGroup = AgeGroup.Kind,
                    PlacementType = plaatsType,
                    DateTime = DateTime.Now,
                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                };
                _gastgezinService.AddPlaatsing(plaatsing);
            }
            if (Onbekend != PrevOnbekend)
            {
                var plaatsing = new Plaatsing()
                {
                    Gastgezin = _gastgezinService.GetGastgezin(GastGezinId),
                    Amount = Onbekend - PrevOnbekend,
                    AgeGroup = AgeGroup.Onbekend,
                    PlacementType = plaatsType,
                    DateTime = DateTime.Now,
                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                };
                _gastgezinService.AddPlaatsing(plaatsing);
            }
            return Redirect("/gastgezin?id=" + GastGezinId);
        }

        public IActionResult PostReservering(int GastGezinId, int PlacementType, int rVolwassen, int rKind, int rOnbekend)
        {
            return PostPlaatsing(GastGezinId, PlacementType, rVolwassen, rKind, rOnbekend);
        }

        public IActionResult PlaatsReservering(int GastGezinId, int rVolwassen, int rKind, int rOnbekend)
        {
            var PrevVolwassen = _gastgezinService.GetPlaatsingen(GastGezinId, PlacementType.Plaatsing, AgeGroup.Volwassene).Sum(p => p.Amount);
            var PrevKind = _gastgezinService.GetPlaatsingen(GastGezinId, PlacementType.Plaatsing, AgeGroup.Kind).Sum(p => p.Amount);
            var PrevOnbekend = _gastgezinService.GetPlaatsingen(GastGezinId, PlacementType.Plaatsing, AgeGroup.Onbekend).Sum(p => p.Amount);
            PostPlaatsing(GastGezinId, 1, PrevVolwassen + rVolwassen, PrevKind + rKind, PrevOnbekend + rOnbekend);
            return Delete(GastGezinId, 0);
        }

        public IActionResult Delete(int GastGezinId, int PlacementType)
        {
            return PostPlaatsing(GastGezinId, PlacementType, 0, 0, 0);
        }

        public IActionResult UpdatePlaatsing()
        {
            return View();
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
        public IActionResult UpdateOpties(GastgezinStatus Status, bool HasVOG, int GastGezinId)
        {
            var gastgezin = _gastgezinService.GetGastgezin(GastGezinId);
            if (gastgezin != null)
            {
                gastgezin.Status = Status;
                gastgezin.HasVOG = HasVOG;
                _gastgezinService.UpdateGastgezin(gastgezin, GastGezinId);
                return Redirect("/gastgezin?id=" + GastGezinId);
            }
            else return Redirect("Error");
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("/BeschikbareGastgezinnen")]
        [HttpGet]
        public IActionResult BeschikbareGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending")
        {
            _userService.checkIfUserExists(User);

            var model = new BeschikbareGastgezinnenModel();

            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetAllGastgezinnen().Where(g => g.IntakeFormulier != null).ToList();

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

                if (gastGezin.Begeleider != null)
                {
                    model.MijnGastgezinnen.Add(new GastGezin
                    {
                        Id = gastGezin.Id,
                        Adres = adresText,
                        Email = contact.Email,
                        Naam = contact.Naam + " " + contact.Achternaam,
                        Telefoonnummer = contact.Telefoonnummer,
                        Woonplaats = woonplaatsText,
                        Begeleider = $"{gastGezin.Begeleider.FirstName} {gastGezin.Begeleider.LastName} ({gastGezin.Begeleider.Email})",
                        PlaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing),
                        ReserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering),
                        PlaatsingsInfo = gastGezin.PlaatsingsInfo,
                        HasVOG = gastGezin.HasVOG,
                        AanmeldFormulierId = aanmeldFormulierId,
                        IntakeFormulierId = intakeFormulierId,
                        Note = gastGezin.Note,
                    });
                }
            }
            if(sortOrder == "Ascending")
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
                }
            }
            else if( sortOrder == "Descending")
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
                }
            }
            model.TotalPlaatsingTag = _gastgezinService.GetPlaatsingenTag(gastGezinnen.ToList(), PlacementType.Plaatsing);
            model.TotalResTag = _gastgezinService.GetPlaatsingenTag(gastGezinnen.ToList(), PlacementType.Reservering);
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
    }
}
