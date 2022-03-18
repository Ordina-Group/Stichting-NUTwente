using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business;
using Ordina.StichtingNuTwente.WebApp.Models;
using System.Diagnostics;
using Ordina.StichtingNuTwente.Entities;
using System.Text.Json;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Ordina.StichtingNuTwente.Business.Helpers;

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
     //   [AllowAnonymous]
        [Route("GastgezinAanmelding")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexGastgezinAanmelding()
        {
            string file = FormHelper.GetFilenameFromId(1);
            
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

      //  [Authorize]
        [Route("GastgezinIntake")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexGastgezinIntake()
        {
            
            string file = FormHelper.GetFilenameFromId(2);
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
            
        }

       // [Authorize]
        [Route("VluchtelingIntake")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexVluchtelingIntake()
        {
           
                string file = FormHelper.GetFilenameFromId(3);
                Form questionForm = _formBusiness.createFormFromJson(1, file);
                return View(questionForm);
           
        }

   //     [AllowAnonymous]
        [Route("VrijwilligerAanmelding")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexVrijwilligerAanmelding()
        {
            string file = FormHelper.GetFilenameFromId(4);
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

//[Authorize]
        [Route("getnutwenteoverheidreactiesdetail25685niveau")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult getnutwenteoverheidreactiesdetail25685niveau(int id)
        {
            if (LoggedIn(2))
            {
                Form questionForm = _reactionService.GetAnwersFromId(id);
                return View(questionForm);
            }
            else
            {
                return Redirect("/loginnutwentevrijwilligers?redirect=getnutwenteoverheidreacties987456list");
            }
        }
     //   [AllowAnonymous]
        [Route("Bedankt")]
        [HttpGet]
        public IActionResult Bedankt()
        {

            return View();
        }

        //[Authorize]
        [Route("getnutwenteoverheidreacties987456list")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreacties987456list()
        {
            if (LoggedIn(2))
            {
                var responses = _reactionService.GetAllRespones();
                return View(responses);
            }
            else
            {
                return Redirect("/loginnutwentevrijwilligers?redirect=getnutwenteoverheidreacties987456list");
            }
        }

     //   [Authorize]
        [Route("getnutwenteoverheidreactiesspecifiek158436form")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreactiesspecifiek158436form(int formId)
        {
            if (LoggedIn(2))
            {
                var responses = _reactionService.GetAllRespones(formId);
                return View(responses);
            }
            else
            {
                return Redirect("/loginnutwentevrijwilligers?redirect=getnutwenteoverheidreacties987456list");
            }
        }

     //   [Authorize]
        [Route("downloadexport15filefromform")]
        [HttpGet]
        [ActionName("Bedankt")]
        public IActionResult downloadexport15filefromform(int formId)
        {
            if (LoggedIn(2))
            {
                var file = _reactionService.GenerateExportCSV(formId);
                MemoryStream stream = new MemoryStream(file);
                return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = string.Format("form Export {0:dd-MM-yyyy}.xlsx", DateTime.Now) };
            }
            else
            {
                return Redirect("/loginnutwentevrijwilligers?redirect=getnutwenteoverheidreacties987456list");
            }
        }



        [HttpPost]
        public IActionResult Save(string answers)
        {
            try
            {
                if (answers != null)
                {
                    var answerData = JsonSerializer.Deserialize<AnswersViewModel>(answers);
                    _reactionService.Save(answerData);
                }
            }
            catch (Exception ex)
            {

            }

            return View();

        }

    //    [Authorize]
        [HttpPut]
        public IActionResult Update(string answers, int id)
        {
            try
            {
                if (answers != null && HttpContext.Session.GetString("loggedIn") == "22D4B2BA-EA60-4CC7-AF9B-860B31A321CC")
                {
                    var answerData = JsonSerializer.Deserialize<AnswersViewModel>(answers);
                    _reactionService.Update(answerData, id);
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


        private bool LoggedIn(int minAccessLevel)
        {
            var accessToken = HttpContext.Session.GetString("loggedIn");

            switch (accessToken)
            {
                case "22D4B2BA-EA60-4CC7-AF9B-860B31A321CC": //Vrijwilliger
                    return minAccessLevel < 3;
                case "4F7F9757-4D80-42E8-8583-634503A6E387": // Gast
                    return minAccessLevel < 2;
            }
            return false;
        }
    }
}