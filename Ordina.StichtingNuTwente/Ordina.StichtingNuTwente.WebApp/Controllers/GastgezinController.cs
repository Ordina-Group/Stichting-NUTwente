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
                Redirect("Error");
            }
            if (gastGezin.Begeleider.AADId != _userService.getUserFromClaimsPrincipal(User).AADId || !User.HasClaims("groups", "group-secretariaat", "group-coordinator", "group-superadmin"))
            {
                Redirect("User/AccesDeniedCatch");
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

                if (gastGezin.Contact.Reactie != null)
                {
                    aanmeldFormulierId = gastGezin.Contact.Reactie.Id;
                }

                if (gastGezin.IntakeFormulier != null)
                {
                    intakeFormulierId = gastGezin.IntakeFormulier.Id;
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
                    IntakeFormulierId = intakeFormulierId
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
    }
}
