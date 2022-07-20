using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Business.Services;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireSecretariaatRole")]
    [Route("Plaatsingen")]
    public class PlaatsingenController : Controller
    {
        private readonly IPlaatsingenService _plaatsingenService;

        public PlaatsingenController(IPlaatsingenService plaatsingenService)
        {
            _plaatsingenService = plaatsingenService;
        }

        [Route("Verwijderd")]
        [HttpGet]
        public IActionResult Verwijderd()
        {
            var plaatsingen = _plaatsingenService.GetAllPlaatsingen(PlacementType.VerwijderdePlaatsing).Where(p => p.DepartureDestination != null && p.DepartureDestination != DepartureDestination.Correctie);
            return View(plaatsingen);
        }
    }
}
