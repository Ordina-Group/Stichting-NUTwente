using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.WebApp.Models;
using System.Diagnostics;
using System.Text.Json;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Ordina.StichtingNuTwente.Business.Helpers;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFormBusiness _formBusiness;
        private readonly IReactionService _reactionService;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IFormBusiness formBusiness, IReactionService reactionService, IUserService userService)
        {
            _logger = logger;
            _formBusiness = formBusiness;
            _reactionService = reactionService;
            _userService = userService;
        }
        [AllowAnonymous]
        [Route("GastgezinAanmelding")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexGastgezinAanmelding()
        {
            checkIfUserExists();
            string file = FormHelper.GetFilenameFromId(1);

            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("GastgezinIntake")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexGastgezinIntake()
        {
            checkIfUserExists();
            string file = FormHelper.GetFilenameFromId(2);
            Form questionForm = _formBusiness.createFormFromJson(2, file);
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllVrijwilligers());
            return View(questionForm);
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("VluchtelingIntake")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexVluchtelingIntake()
        {
            checkIfUserExists();
            string file = FormHelper.GetFilenameFromId(3);
            Form questionForm = _formBusiness.createFormFromJson(3, file);
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllVrijwilligers());
            return View(questionForm);

        }

        [AllowAnonymous]
        [Route("VrijwilligerAanmelding")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexVrijwilligerAanmelding()
        {
            checkIfUserExists();
            string file = FormHelper.GetFilenameFromId(4);
            Form questionForm = _formBusiness.createFormFromJson(1, file);
            return View(questionForm);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("getnutwenteoverheidreactiesdetail25685niveau")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult getnutwenteoverheidreactiesdetail25685niveau(int id)
        {
            checkIfUserExists();
            Form questionForm = _reactionService.GetAnwersFromId(id);
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllVrijwilligers());
            return View(questionForm);
        }
        [AllowAnonymous]
        [Route("Bedankt")]
        [HttpGet]
        public IActionResult Bedankt()
        {
            checkIfUserExists();
            return View();
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("getnutwenteoverheidreacties987456list")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreacties987456list()
        {
            checkIfUserExists();
            var responses = _reactionService.GetAllRespones();
            return View(responses);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("getnutwenteoverheidreactiesspecifiek158436form")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreactiesspecifiek158436form(int formId)
        {
            checkIfUserExists();
            var responses = _reactionService.GetAllRespones(formId);
            return View(responses);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("downloadexport15filefromform")]
        [HttpGet]
        [ActionName("Bedankt")]
        public IActionResult downloadexport15filefromform(int formId)
        {
            var file = _reactionService.GenerateExportCSV(formId);
            var fileName = FormHelper.GetFilenameFromId(formId).Replace(".json", "");

            MemoryStream stream = new MemoryStream(file);
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = string.Format("{1} {0:dd-MM-yyyy}.xlsx", DateTime.Now, fileName) };
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

        [Authorize(Policy = "RequireSecretariaatRole")]
        [HttpPut]
        public IActionResult Update(string answers, int id)
        {
            try
            {
                if (answers != null)
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

        [Authorize(Policy = "RequireSecretariaatRole")]
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            try
            {
                var numId = int.Parse(id);
                _reactionService.Delete(numId);
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


        public UserDetails? GetUser()
        {
            var aadID = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (aadID != null)
            {
                var userDetails = this._userService.GetUserByAADId(aadID.Value);
                return userDetails;
            }
            return null;
        }

        public List<UserDetails> GetAllVrijwilligers()
        {
            return _userService.GetUsersByRole("group-vrijwilliger").ToList();
        }


        public void checkIfUserExists()
        {
            if (User != null && User.Claims.Count() > 0)
            {
                var aadID = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
                if (aadID != null)
                {
                    var userDetails = this._userService.GetUserByAADId(aadID.Value);
                    if (userDetails == null)
                    {
                        var email = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress"))?.Value;
                        var givenname = User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value;
                        var surname = User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value;
                        var groups = User.Claims.Where(c => c.Type.Contains("group")).Select(x => x.Value);
                        if (givenname == null)
                            givenname = "";
                        if (surname == null)
                            surname = "";
                        var newUserDetails = new UserDetails()
                        {
                            FirstName = givenname,
                            LastName = surname,
                            Email = email,
                            Roles = groups.ToList(),
                            AADId = aadID.Value
                        };
                        _userService.Save(newUserDetails);
                    }
                }
            }
        }
    }
}