using Microsoft.AspNetCore.Mvc;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class GastgezinController : Controller
    {
        public IActionResult Overview()
        {
            return View();
        }
    }
}
