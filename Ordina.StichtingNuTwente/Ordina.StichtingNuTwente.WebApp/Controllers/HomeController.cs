using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.WebApp.Models;
using System.Diagnostics;
using System.Text.Json;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Ordina.StichtingNuTwente.Business.Helpers;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Business.Services;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFormBusiness _formBusiness;
        private readonly IReactionService _reactionService;
        private readonly IUserService _userService;
        private readonly IGastgezinService _gastgezinService;
        private readonly IPersoonService _persoonService;
        private readonly IMailService _mailService;

        public HomeController(ILogger<HomeController> logger, IFormBusiness formBusiness, IReactionService reactionService, IUserService userService, IGastgezinService gastgezinService, IPersoonService persoonService, IMailService mailService)
        {
            _logger = logger;
            _formBusiness = formBusiness;
            _reactionService = reactionService;
            _userService = userService;
            _gastgezinService = gastgezinService;
            _persoonService = persoonService;
            _mailService = mailService;
        }

        [AllowAnonymous]
        [Route("GastgezinAanmelding")]
        [HttpGet]
        [ActionName("QuestionForm")]
        public IActionResult IndexGastgezinAanmelding()
        {
            _userService.checkIfUserExists(User);
            string file = FormHelper.GetFilenameFromId(1);

            Form questionForm = _formBusiness.createFormFromJson(1, file);
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
            string file = FormHelper.GetFilenameFromId(2);
            Form questionForm = _formBusiness.createFormFromJson(2, file);
            questionForm.GastgezinId = gastgezinId;
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllDropdown());
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
            string file = FormHelper.GetFilenameFromId(3);
            Form questionForm = _formBusiness.createFormFromJson(3, file);
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
            string file = FormHelper.GetFilenameFromId(4);
            Form questionForm = _formBusiness.createFormFromJson(1, file);
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
            if (questionForm == null || questionForm.Sections == null)
                return Redirect("Error");
            questionForm.UserDetails = GetUser();
            questionForm.AllUsers.AddRange(GetAllVrijwilligers());
            FillBaseModel(questionForm);
            return View(questionForm);
        }

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("MijnGastgezinnen")]
        [HttpGet]
        [ActionName("MijnGastgezinnen")]
        public IActionResult MijnGastgezinnen(string? filter)
        {
            _userService.checkIfUserExists(User);

            var mijnGastgezinnen = new MijnGastgezinnenModel();

            var user = GetUser();
            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(user.Id);
            if (filter != null)
            {
                if (filter == "Buddy")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Buddy == user).ToList();
                }
                if (filter == "Intaker")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Begeleider == user).ToList();
                }
            }

            foreach (var gastGezin in gastGezinnen)
            {
                if (gastGezin.Contact == null)
                {
                    continue;
                }

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
                var heeftBekeken = false;
                if (user.Id == gastGezin.Buddy?.Id)
                {
                    heeftBekeken = gastGezin.BekekenDoorBuddy;
                }

                if (user.Id == gastGezin.Begeleider?.Id)
                {
                    heeftBekeken = gastGezin.BekekenDoorIntaker;
                }

                mijnGastgezinnen.MijnGastgezinnen.Add(new GastGezin
                {
                    Id = gastGezin.Id,
                    Adres = adresText,
                    Email = contact.Email,
                    Naam = contact.Naam + " " + contact.Achternaam,
                    Telefoonnummer = contact.Telefoonnummer,
                    Woonplaats = woonplaatsText,
                    AanmeldFormulierId = aanmeldFormulierId,
                    IntakeFormulierId = intakeFormulierId,
                    PlaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing),
                    ReserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering),
                    PlaatsingsInfo = gastGezin.PlaatsingsInfo,
                    HasVOG = gastGezin.HasVOG,
                    HeeftBekeken = heeftBekeken
                });
            }

            FillBaseModel(mijnGastgezinnen);
            return View(mijnGastgezinnen);
        }


        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("MijnGastgezinnen/{userId}")]
        [HttpGet]
        [ActionName("MijnGastgezinnen")]
        public IActionResult MijnGastgezinnen(string? filter, int userId)
        {
            _userService.checkIfUserExists(User);

            var mijnGastgezinnen = new MijnGastgezinnenModel();

            var user = _userService.GetUserById(userId);
            if(user == null)
                return Redirect("Error");
            mijnGastgezinnen.GastgezinnenVan = user.FirstName + " " + user.LastName;

            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(user.Id);
            if (filter != null)
            {
                if (filter == "Buddy")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Buddy == user).ToList();
                }
                if (filter == "Intaker")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Begeleider == user).ToList();
                }
            }

            foreach (var gastGezin in gastGezinnen)
            {
                if (gastGezin.Contact == null)
                {
                    continue;
                }

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
                var heeftBekeken = false;
                if (user.Id == gastGezin.Buddy?.Id)
                {
                    heeftBekeken = gastGezin.BekekenDoorBuddy;
                }

                if (user.Id == gastGezin.Begeleider?.Id)
                {
                    heeftBekeken = gastGezin.BekekenDoorIntaker;
                }

                mijnGastgezinnen.MijnGastgezinnen.Add(new GastGezin
                {
                    Id = gastGezin.Id,
                    Adres = adresText,
                    Email = contact.Email,
                    Naam = contact.Naam + " " + contact.Achternaam,
                    Telefoonnummer = contact.Telefoonnummer,
                    Woonplaats = woonplaatsText,
                    AanmeldFormulierId = aanmeldFormulierId,
                    IntakeFormulierId = intakeFormulierId,
                    PlaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing),
                    ReserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering),
                    PlaatsingsInfo = gastGezin.PlaatsingsInfo,
                    HasVOG = gastGezin.HasVOG,
                    HeeftBekeken = heeftBekeken
                });
            }

            FillBaseModel(mijnGastgezinnen);
            return View(mijnGastgezinnen);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("AlleGastgezinnen")]
        [HttpGet]
        [ActionName("AlleGastgezinnen")]
        public IActionResult AlleGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending")
        {
            _userService.checkIfUserExists(User);

            var mijnGastgezinnen = new AlleGastgezinnenModel();

            var vrijwilligers = GetAllDropdown();
            foreach (var vrijwilliger in vrijwilligers.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                mijnGastgezinnen.Vrijwilligers.Add(new Vrijwilliger
                {
                    Id = vrijwilliger.Id,
                    Naam = $"{vrijwilliger.FirstName} {vrijwilliger.LastName}",
                    Email = vrijwilliger.Email
                });
            }

            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetAllGastgezinnen();

            foreach (var gastGezin in gastGezinnen)
            {
                if (gastGezin.Contact == null)
                {
                    continue;
                }

                var contact = gastGezin.Contact;
                var adres = gastGezin.Contact.Adres;
                var adresText = "";
                var woonplaatsText = "";

                if (adres != null)
                {
                    adresText = adres.Straat;
                    woonplaatsText = adres.Woonplaats;
                }
                var aanmeldFormulierId = -1;
                var intakeFormulierId = -1;
                if (gastGezin.AanmeldFormulier != null)
                    aanmeldFormulierId = gastGezin.AanmeldFormulier.Id;
                if (gastGezin.IntakeFormulier != null)
                    intakeFormulierId = gastGezin.IntakeFormulier.Id;

                var begeleiderId = 0;
                var begeleider = "";
                if (gastGezin.Begeleider != null)
                {
                    begeleiderId = gastGezin.Begeleider.Id;
                    begeleider = $"{gastGezin.Begeleider.FirstName} {gastGezin.Begeleider.LastName} ({gastGezin.Begeleider.Email})";
                }

                var buddyId = 0;
                var buddy = "";
                if (gastGezin.Buddy != null)
                {
                    buddyId = gastGezin.Buddy.Id;
                    buddy = $"{gastGezin.Buddy.FirstName} {gastGezin.Buddy.LastName} ({gastGezin.Buddy.Email})";
                }

                mijnGastgezinnen.Gastgezinnen.Add(new GastGezin
                {
                    Id = gastGezin.Id,
                    Adres = adresText,
                    Email = contact.Email,
                    Naam = contact.Naam + " " + contact.Achternaam,
                    Telefoonnummer = contact.Telefoonnummer,
                    Woonplaats = woonplaatsText,
                    Begeleider = begeleider,
                    BegeleiderId = begeleiderId,
                    Buddy = buddy,
                    BuddyId = buddyId,
                    PlaatsingTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Plaatsing, gastGezin),
                    ReserveTag = _gastgezinService.GetPlaatsingTag(gastGezin.Id, PlacementType.Reservering, gastGezin),
                    PlaatsingsInfo = gastGezin.PlaatsingsInfo,
                    HasVOG = gastGezin.HasVOG,
                    AanmeldFormulierId = aanmeldFormulierId,
                    IntakeFormulierId = intakeFormulierId,
                    Note = gastGezin.Note,
                });

            }

            if (sortOrder == "Ascending")
            {
                switch (sortBy)
                {
                    case "Woonplaats":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "Woonplaats";
                        break;
                    case "Naam":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderBy(g => g.Naam).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "Naam";
                        break;
                    case "Telefoonnummer":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderBy(g => g.Telefoonnummer).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "Telefoonnummer";
                        break;
                    case "Intaker":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderBy(g => g.Begeleider).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "Intaker (laag-hoog)";
                        break;
                    case "Buddy":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderBy(g => g.Buddy).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "Buddy (laag-hoog)";
                        break;
                    case "AanmeldingsId":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderBy(g => g.AanmeldFormulierId).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "AanmeldingsId (laag-hoog)";
                        break;
                    case "IntakeId":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderBy(g => g.IntakeFormulierId).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "IntakeId (laag-hoog)";
                        break;
                }
            }
            else if (sortOrder == "Descending")
            {
                switch (sortBy)
                {
                    case "Intaker":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderByDescending(g => g.Begeleider).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "Intaker (hoog-laag)";
                        break;
                    case "Buddy":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderByDescending(g => g.Buddy).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "Buddy (hoog-laag)";
                        break;
                    case "AanmeldingsId":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderByDescending(g => g.AanmeldFormulierId).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "AanmeldingsId (hoog-laag)";
                        break;
                    case "IntakeId":
                        mijnGastgezinnen.Gastgezinnen = mijnGastgezinnen.Gastgezinnen.OrderByDescending(g => g.IntakeFormulierId).ThenBy(g => g.Woonplaats).ToList();
                        mijnGastgezinnen.SortDropdownText = "IntakeId (hoog-laag)";
                        break;
                }
            }

            FillBaseModel(mijnGastgezinnen);
            return View(mijnGastgezinnen);
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("AlleGastgezinnen")]
        [HttpPost]
        [ActionName("AlleGastgezinnen")]
        public IActionResult AlleGastgezinnenPost(IFormCollection formCollection)
        {
            var vrijwilligers = GetAllDropdown();

            foreach (var key in formCollection.Keys)
            {
                if (key.StartsWith("vrijwilliger_"))
                {
                    var value = formCollection[key];
                    int vrijwilligerId = 0;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        vrijwilligerId = Convert.ToInt32(value);
                    }

                    var gastgezinId = Convert.ToInt32(key.Substring(13));

                    var gastgezinItem = _gastgezinService.GetGastgezin(gastgezinId);
                    if (gastgezinItem == null)
                    {
                        continue;
                    }

                    UserDetails? assignVrijwilliger = null;
                    if (vrijwilligerId > 0)
                    {
                        assignVrijwilliger = vrijwilligers.FirstOrDefault(e => e.Id == vrijwilligerId);
                    }

                    if (assignVrijwilliger == null && gastgezinItem.Begeleider != null)
                    {
                        gastgezinItem.Begeleider = null;
                        gastgezinItem.BekekenDoorIntaker = false;
                        _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                    }
                    else
                    {
                        if (gastgezinItem.Begeleider != null && gastgezinItem.Begeleider.Id != assignVrijwilliger.Id)
                        {
                            gastgezinItem.Begeleider = assignVrijwilliger;
                            gastgezinItem.BekekenDoorIntaker = false;
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                        else if (gastgezinItem.Begeleider == null)
                        {
                            gastgezinItem.Begeleider = assignVrijwilliger;
                            gastgezinItem.BekekenDoorIntaker = false;
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                    }
                }
            }

            foreach (var key in formCollection.Keys)
            {
                if (key.StartsWith("buddy_"))
                {
                    var value = formCollection[key];
                    int vrijwilligerId = 0;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        vrijwilligerId = Convert.ToInt32(value);
                    }

                    var gastgezinId = Convert.ToInt32(key.Substring(6));

                    var gastgezinItem = _gastgezinService.GetGastgezin(gastgezinId);
                    if (gastgezinItem == null)
                    {
                        continue;
                    }

                    UserDetails? assignVrijwilliger = null;
                    if (vrijwilligerId > 0)
                    {
                        assignVrijwilliger = vrijwilligers.FirstOrDefault(e => e.Id == vrijwilligerId);
                    }

                    if (assignVrijwilliger == null && gastgezinItem.Buddy != null)
                    {
                        gastgezinItem.Buddy = null;
                        gastgezinItem.BekekenDoorBuddy = false;
                        _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                    }
                    else
                    {
                        if (gastgezinItem.Buddy != null && gastgezinItem.Buddy.Id != assignVrijwilliger.Id)
                        {
                            gastgezinItem.Buddy = assignVrijwilliger;
                            gastgezinItem.BekekenDoorBuddy = false;
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                        else if (gastgezinItem.Buddy is null)
                        {
                            gastgezinItem.Buddy = assignVrijwilliger;
                            gastgezinItem.BekekenDoorBuddy = false;
                            _gastgezinService.UpdateGastgezin(gastgezinItem, gastgezinId);
                        }
                    }
                }
            }

            return RedirectToAction("AlleGastgezinnen");
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

        [Authorize(Policy = "RequireVrijwilligerRole")]
        [Route("mijnReacties")]
        [HttpGet]
        [ActionName("GetAllReactions")]
        public IActionResult getMijnReacties()
        {
            _userService.checkIfUserExists(User);
            var responses = _userService.GetMyReacties(User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value);
            if (responses != null)
            {
                var viewModel = new AnswerModel
                {
                    //AnswerLists = responses.ToList().ConvertAll(r => ReactieMapping.FromDatabaseToWebListModel(r))
                };
                FillBaseModel(viewModel);
                return View(viewModel);
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Save(string answers, int? gastgezinId)
        {
            try
            {
                if (answers != null)
                {
                    var answerData = JsonSerializer.Deserialize<AnswersViewModel>(answers);
                    _reactionService.Save(answerData, gastgezinId);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAndSendEmailAsync(string answers)
        {
            try
            {
                if (answers != null)
                {
                    var answerData = JsonSerializer.Deserialize<AnswersViewModel>(answers);
                    //var reactieId = _reactionService.SaveAndGetReactieId(answerData);
                    //var persoon = _persoonService.GetPersoonByReactieId(reactieId);
                    //MailHelper mailHelper = new MailHelper(_mailService);
                    //bool success = await mailHelper.bevestiging(persoon);
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
        public IActionResult Delete(string id)
        {
            try
            {
                var numId = int.Parse(id);
                _reactionService.Delete(numId);
                return Ok();
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