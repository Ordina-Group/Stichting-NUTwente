using Microsoft.AspNetCore.Mvc;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class VluchtelingController : Controller
    {

        private readonly IPersoonService _persoonService;

        public HomeController(IPersoonservice persoonservice)
        {
            _persoonService = persoonservice;
            
        }

        public IActionResult Vluchtelingen()
        {

            return View();
        }
    }
}
