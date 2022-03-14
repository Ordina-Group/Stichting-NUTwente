using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business;
using Ordina.StichtingNuTwente.WebApp.Models;
using System.Diagnostics;
using Ordina.StichtingNuTwente.Entities;
using System.Text.Json;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Business.Interfaces;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFormBusiness _formBusiness;
        private readonly IReactionService _reactionService;

        public HomeController(ILogger<HomeController> logger, IFormBusiness formBusiness, IReactionService reactionService)
        {
            _logger = logger;
            _formBusiness = formBusiness;
            _reactionService = reactionService;
        }

        [Route("GastgezinAanmelding")]
        [HttpGet]
        [ActionName("Index")]
        public IActionResult IndexGastgezinAanmelding()
        {
            string file = "GastgezinAanmelding.json";
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        
        [Route("GastgezinIntake")]
        [HttpGet]
        [ActionName("Index")]
        public IActionResult IndexGastgezinIntake()
        {
            string file = "GastgezinIntake.json";
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        [Route("VluchtelingIntake")]
        [HttpGet]
        [ActionName("Index")]
        public IActionResult IndexVluchtelingIntake()
        {
            string file = "VluchtelingIntake.json";
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        [Route("VrijwilligerAanmelding")]
        [HttpGet]
        [ActionName("Index")]
        public IActionResult IndexVrijwilligerAanmelding()
        {
            string file = "VrijwilligerAanmelding.json";
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        [Route("getnutwenteoverheidreactiesdetail25685niveau")]
        [HttpGet]
        [ActionName("Index")]
        public IActionResult getnutwenteoverheidreactiesdetail25685niveau(int id)
        {
            Form questionForm = _reactionService.GetAnwersFromId(id);
            return View(questionForm);
        }

        [Route("Bedankt")]
        [HttpGet]
        public IActionResult Bedankt()
        {
           
            return View();
        }

        [Route("getnutwenteoverheidreacties987456list")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreacties987456list()
        {
            var responses = _reactionService.GetAllRespones();
            return View(responses);
        }

        [Route("getnutwenteoverheidreactiesspecifiek158436form")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreactiesspecifiek158436form(int formId)
        {
            var responses = _reactionService.GetAllRespones(formId);
            return View(responses);
        }

        [Route("downloadexport15filefromform")]
        [HttpGet]
        [ActionName("Bedankt")]
        public IActionResult downloadexport15filefromform(int formId)
        {

            var file = _reactionService.GenerateExportCSV(formId);
            MemoryStream stream = new MemoryStream(file);
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = string.Format("form Export {0:dd-MM-yyyy}.xlsx", DateTime.Now) };
        }



        [HttpPost]
        public IActionResult Save(string answers)
        {
            try
            {
                if (answers != null)
                {
                    var answerData =JsonSerializer.Deserialize<AnswersViewModel>(answers);
                    _reactionService.Save(answerData);
                }
            }
            catch (Exception ex)
            {

            }

            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}