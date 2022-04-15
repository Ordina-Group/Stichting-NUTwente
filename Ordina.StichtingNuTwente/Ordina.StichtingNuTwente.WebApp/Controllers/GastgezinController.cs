using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Models.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireCoordinatorRole")]
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
            var gastGezin = _gastgezinService.GetGastgezin(id);

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
            viewModel.PlaatsingDTO = new PlaatsingDTO();

            viewModel.PlaatsingStats = new PlaatsingStats();
            viewModel.PlaatsingStats.PlaatsVolwassen = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.PlaatsKind = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResVolwassen = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResKind = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResOnbekend = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);

            return View(viewModel);
        }

        [ActionName("PostPlaatsing")]
        [Route("GastgezinController/PostPlaatsing")]
        public IActionResult PostPlaatsing(int GastGezinId, int PlaatsVolwassen, int PlaatsKind)
        {
            var PrevPlaatsVolwassen = _gastgezinService.GetPlaatsingen(GastGezinId, PlacementType.Plaatsing, AgeGroup.Volwassene).Sum(p => p.Amount);
            var PrevPlaatsKind = _gastgezinService.GetPlaatsingen(GastGezinId, PlacementType.Plaatsing, AgeGroup.Kind).Sum(p => p.Amount);

            if (PlaatsVolwassen != PrevPlaatsVolwassen)
            {
                var plaatsing = new Plaatsing()
                {
                    Gastgezin = _gastgezinService.GetGastgezin(GastGezinId),
                    Amount = PlaatsVolwassen - PrevPlaatsVolwassen,
                    AgeGroup = AgeGroup.Volwassene,
                    PlacementType = PlacementType.Plaatsing,
                    DateTime = DateTime.Now,
                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                };
                _gastgezinService.AddPlaatsing(plaatsing);
            }
            if (PlaatsKind != PrevPlaatsKind)
            {
                var plaatsing = new Plaatsing()
                {
                    Gastgezin = _gastgezinService.GetGastgezin(GastGezinId),
                    Amount = PlaatsKind - PrevPlaatsKind,
                    AgeGroup = AgeGroup.Kind,
                    PlacementType = PlacementType.Plaatsing,
                    DateTime = DateTime.Now,
                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User)
                };
                _gastgezinService.AddPlaatsing(plaatsing);
            }
            return Redirect("gastgezin");
        }

        public IActionResult DeletePlaatsing(int GastGezinId)
        {
            return PostPlaatsing(GastGezinId, 0, 0);
        }

        public IActionResult UpdatePlaatsing()
        {
            return View();
        }
    }
}
