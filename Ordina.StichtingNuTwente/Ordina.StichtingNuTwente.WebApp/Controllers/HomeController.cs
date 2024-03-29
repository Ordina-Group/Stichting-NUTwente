﻿using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.WebApp.Models;
using System.Diagnostics;
using System.Text.Json;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Ordina.StichtingNuTwente.Business.Helpers;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Business.Services;
using Ordina.StichtingNuTwente.Models.Mappings;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IReactionService _reactionService;
        private readonly IUserService _userService;
        private readonly IPersoonService _persoonService;
        private readonly IMailService _mailService;
        private readonly IGastgezinService _gastgezinService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IReactionService reactionService, IUserService userService, IPersoonService persoonService, IMailService mailService, IGastgezinService gastgezinService, IConfiguration configuration)
        {
            _logger = logger;
            _reactionService = reactionService;
            _userService = userService;
            _persoonService = persoonService;
            _mailService = mailService;
            _gastgezinService = gastgezinService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [Route("GastgezinAanmelding")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexGastgezinAanmelding()
        {
            _userService.checkIfUserExists(User);
            Form questionForm = FormHelper.GetFormFromFileId(1);
            FillBaseModel(questionForm);
            return View(questionForm);
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("GastgezinIntake")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexGastgezinIntake(int? gastgezinId)
        {
            _userService.checkIfUserExists(User);
            Form questionForm = FormHelper.GetFormFromFileId(2);
            questionForm.GastgezinId = gastgezinId;
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllDropdown());
            if (gastgezinId != null)
            {
                Gastgezin gastgezin = _gastgezinService.GetGastgezin((int)gastgezinId);
                var personaliaQuestions = questionForm.Sections[0].Questions;
                personaliaQuestions.FirstOrDefault(q => q.ParameterName == "Naam").Answer = gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam;
                personaliaQuestions.FirstOrDefault(q => q.ParameterName == "Straat").Answer = gastgezin.Contact.Adres.Straat;
                personaliaQuestions.FirstOrDefault(q => q.ParameterName == "Postcode").Answer = gastgezin.Contact.Adres.Postcode;
                personaliaQuestions.FirstOrDefault(q => q.ParameterName == "Woonplaats").Answer = gastgezin.Contact.Adres.Woonplaats;
                personaliaQuestions.FirstOrDefault(q => q.ParameterName == "Telefoonnummer").Answer = gastgezin.Contact.Telefoonnummer;
                personaliaQuestions.FirstOrDefault(q => q.ParameterName == "Telefoonnummer2").Answer = gastgezin.Contact.Telefoonnummer2;
                personaliaQuestions.FirstOrDefault(q => q.ParameterName == "Email").Answer = gastgezin.Contact.Email;

                var plaatsingsAdresQuestions = questionForm.Sections[1].Questions;
                plaatsingsAdresQuestions.FirstOrDefault(q => q.ParameterName == "AdresVanLocatie").Answer = gastgezin.Contact.Adres.Straat;
                plaatsingsAdresQuestions.FirstOrDefault(q => q.ParameterName == "PostcodeVanLocatie").Answer = gastgezin.Contact.Adres.Postcode;
                plaatsingsAdresQuestions.FirstOrDefault(q => q.ParameterName == "PlaatsnaamVanLocatie").Answer = gastgezin.Contact.Adres.Woonplaats;

            }
            FillBaseModel(questionForm);
            return View(questionForm);
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("VluchtelingIntake")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexVluchtelingIntake()
        {
            _userService.checkIfUserExists(User);
            Form questionForm = FormHelper.GetFormFromFileId(3);
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllDropdown());
            FillBaseModel(questionForm);
            return View(questionForm);
        }

        [AllowAnonymous]
        [Route("VrijwilligerAanmelding")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexVrijwilligerAanmelding()
        {
            _userService.checkIfUserExists(User);
            Form questionForm = FormHelper.GetFormFromFileId(4);
            FillBaseModel(questionForm);
            return View(questionForm);
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("getnutwenteoverheidreactiesdetail25685niveau")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult getnutwenteoverheidreactiesdetail25685niveau(int id)
        {
            _userService.checkIfUserExists(User);
            Form questionForm = _reactionService.GetAnwersFromId(id);
            if (questionForm == null || questionForm.Sections == null) return Redirect("Error");
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllVrijwilligers());
            FillBaseModel(questionForm);
            return View(questionForm);
        }



        [AllowAnonymous]
        [Route("Bedankt")]
        [HttpGet]
        public IActionResult Bedankt()
        {
            _userService.checkIfUserExists(User);
            return View();
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("getnutwenteoverheidreacties987456list")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreacties987456list()
        {
            _userService.checkIfUserExists(User);

            var model = new AnswerModel
            {
                AnswerLists = _reactionService.GetAllRespones()
            };
            FillBaseModel(model);
            return View(model);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("getnutwenteoverheidreactiesspecifiek158436form")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getnutwenteoverheidreactiesspecifiek158436form(int formId)
        {
            _userService.checkIfUserExists(User);
            var model = new AnswerModel
            {
                AnswerLists = _reactionService.GetAllRespones(formId)
            };
            FillBaseModel(model);
            return View(model);
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("mijnReacties")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getMijnReacties()
        {
            _userService.checkIfUserExists(User);
            var user = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value;
            var model = new AnswerModel
            {
                AnswerLists = _userService.GetMyReacties(user)
            };
            FillBaseModel(model);
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(string answers, int? gastgezinId)
        {
            if (answers != null)
            {
                Persoon persoon = new();
                Reactie reaction = new();
                try
                {
                    var answerData = JsonSerializer.Deserialize<AnswersViewModel>(answers);
                    reaction = _reactionService.NewReactie(answerData, gastgezinId);
                    persoon = _persoonService.GetPersoonByReactieId(reaction.Id);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                switch (reaction.FormulierId)
                {
                    //FormulierId = "GastgezinAanmelding.json";
                    case 1:
                        _mailService.AanmeldingGastgezin(persoon);
                        break;

                    //FormulierId = "GastgezinIntake.json";
                    case 2:
                        if (gastgezinId != null)
                        {    
                            _mailService.IntakeUitgevoerd(_gastgezinService.GetGastgezin((int)gastgezinId));
                        }
                        break;

                    //fileName = "VrijwilligerAanmelding.json";
                    case 4:
                        _mailService.AanmeldingVrijwilliger(persoon);
                        break;
                }
                return Ok();
            }
            return BadRequest();
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [HttpPut]
        public IActionResult Update(string answers, int id)
        {
            try
            {
                if (answers != null)
                {
                    var answerData = JsonSerializer.Deserialize<AnswersViewModel>(answers);
                    _reactionService.Update(answerData, id);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [Authorize(Policy = "RequireSecretariaatRole")]
        [HttpDelete]
        public IActionResult Delete(string id, string comment)
        {
            try
            {
                var aadID = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
                if (aadID != null)
                {
                    var userDetails = this._userService.GetUserByAADId(aadID.Value);
                    if (userDetails != null)
                    {
                        var numId = int.Parse(id);
                        _reactionService.Delete(numId, comment, userDetails);
                        return Ok();
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [HttpPost]
        public IActionResult Restore(string id)
        {
            try
            {
                var aadID = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
                if (aadID != null)
                {
                    var userDetails = this._userService.GetUserByAADId(aadID.Value);
                    if (userDetails != null)
                    {
                        var numId = int.Parse(id);
                        _reactionService.Restore(numId);
                        return Ok();
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult FriendlyError()
        {
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
            return _userService.GetUsersByRole("group-vrijwilliger").OrderBy(u => u.FirstName).ToList();
        }

        public List<UserDetails> GetAllDropdown()
        {
            return _userService.GetAllDropdownUsers().OrderBy(u => u.FirstName).ToList();
        }

        public void FillBaseModel(BaseModel model)
        {
            var user = GetUser();

            if (user == null || user.Roles == null) return;

            model.IsSecretariaat = user.Roles.Contains("group-secretariaat");
            model.IsVrijwilliger = user.Roles.Contains("group-vrijwilliger");
        }
    }
}