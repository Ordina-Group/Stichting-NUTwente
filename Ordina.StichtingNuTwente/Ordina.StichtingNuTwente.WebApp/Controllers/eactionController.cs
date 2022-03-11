using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.WebApp.Models;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class eactionController : Controller
    {
        public IActionResult Index()
        {
            var filledForms = new List<Reactie>(){
            new Reactie() {
                Antwoorden = new List<Antwoord>(),
                DatumIngevuld = DateTime.Now,
                FormulierId = 0
            },
            new Reactie() {
                Antwoorden = new List<Antwoord>(),
                DatumIngevuld = DateTime.Now.AddDays(-1),
                FormulierId = 1
            },
            new Reactie() {
                Antwoorden = new List<Antwoord>(),
                DatumIngevuld = DateTime.Now.AddDays(-2),
                FormulierId = 0
            }
            };
            return View(filledForms);
        }
    }
}
