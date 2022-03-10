using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business;
using Ordina.StichtingNuTwente.WebApp.Models;
using System.Diagnostics;
using Ordina.StichtingNuTwente.Entities;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFormBusiness _formBusiness;

        public HomeController(ILogger<HomeController> logger, IFormBusiness formBusiness)
        {
            _logger = logger;
            _formBusiness = formBusiness;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string file = "GastgezinIntake.json";
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        [HttpPost]
        public IActionResult Save(AnswersViewModel form) 
        {
            string file = "GastgezinIntake.json";
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}