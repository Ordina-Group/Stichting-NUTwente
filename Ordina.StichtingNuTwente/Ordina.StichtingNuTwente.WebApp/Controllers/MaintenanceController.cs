using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireSuperAdminRole")]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly IReactionService _reactionService;
        private readonly IGastgezinService _gastgezinService;
        private readonly IWebHostEnvironment _environment;
        public MaintenanceController(IMaintenanceService maintenanceService, IWebHostEnvironment environment, IReactionService reactionService, IGastgezinService gastgezinService)
        {
            this.maintenanceService = maintenanceService;
            _environment = environment;
            _reactionService = reactionService;
            _gastgezinService = gastgezinService;
        }
        [Route("/gastgezin/maintenance")]
        public IActionResult GastgezinMaintenance(int id)
        {
            var gastGezin = _gastgezinService.GetGastgezin(id);
            if (gastGezin == null)
            {
                return Redirect("Error");
            }

            var viewModel = new GastgezinViewModel() { };
            if (gastGezin.Contact != null)
            {
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

                if (gastGezin.AanmeldFormulier != null)
                {
                    aanmeldFormulierId = gastGezin.AanmeldFormulier.Id;
                }

                if (gastGezin.IntakeFormulier != null)
                {
                    intakeFormulierId = gastGezin.IntakeFormulier.Id;
                }

                viewModel.GastGezin = new GastGezin()
                {
                    Id = id,
                    Adres = adresText,
                    Email = contact.Email,
                    Naam = contact.Naam,
                    Telefoonnummer = contact.Telefoonnummer,
                    Woonplaats = woonplaatsText,
                    AanmeldFormulierId = aanmeldFormulierId,
                    IntakeFormulierId = intakeFormulierId,
                    Note = gastGezin.Note,
                    Status = gastGezin.Status,
                    HasVOG = gastGezin.HasVOG,
                    MaxAdults = gastGezin.MaxAdults,
                    MaxChildren = gastGezin.MaxChildren
                };
            }
            return View(viewModel);
        }

        public IActionResult Index()
        {
            return View(new MaintenanceModel());
        }
        public IActionResult LinkBegeleiderToGastgezin()
        {
            try
            {
                var model = new MaintenanceModel();
                var messages = maintenanceService.LinkBegeleiderToGastgezin();
                model.Messages.AddRange(messages.Select(x => new MaintenanceMessage
                {
                    Message = x.Message,
                    MessageType = (MaintenanceMessageType)x.MessageType
                }));
                ViewBag.Message = "Database Update Successful!!";
                return View("Index", model);
            }
            catch
            {
                ViewBag.Message = "Database Update failed!!";
                return View("Index", new MaintenanceModel());
            }

        }

        public IActionResult UpdateAll()
        {
            try
            {
                _reactionService.UpdateAll();
                ViewBag.Message = "Database Update Successful!!";
                return View("Index", new MaintenanceModel());
            }
            catch
            {
                ViewBag.Message = "Database Update failed!!";
                return View("Index", new MaintenanceModel());
            }

        }

        public IActionResult DatabaseIntegrity()
        {
            var databaseIntegrityModel = maintenanceService.TestDatabaseIntegrity();
            return View(databaseIntegrityModel);
        }

        [HttpPost]
        public async Task<ActionResult> UploadUpdateIntake(IFormFile file)
        {

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            try
            {
                var maintenanceModel = new MaintenanceModel();
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var messages = maintenanceService.UpdateDataFromExcel(fileStream, 2);
                    maintenanceModel.Messages.AddRange(messages.Select(x => new MaintenanceMessage
                    {
                        Message = x.Message,
                        MessageType = (MaintenanceMessageType)x.MessageType
                    }));
                }
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", maintenanceModel);
            }
            catch
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File upload failed!!";
                return View("Index", new MaintenanceModel());
            }
        }


        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    maintenanceService.LoadDataFromExcel(fileStream, 1);
                }
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", new MaintenanceModel());
            }
            catch
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File upload failed!!";
                return View("Index", new MaintenanceModel());
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadPlaatsingFile(IFormFile file)
        {

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            var maintenanceModel = new MaintenanceModel();
            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var messages = maintenanceService.LoadPlaatsingDataFromExcel(fileStream, User);
                    maintenanceModel.Messages.AddRange(messages.Select(x => new MaintenanceMessage
                    {
                        Message = x.Message,
                        MessageType = (MaintenanceMessageType)x.MessageType
                    }));
                }
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", maintenanceModel);
        }
            catch
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File upload failed!!";
                return View("Index", new MaintenanceModel());
            }
}

        [HttpPost]
        public async Task<ActionResult> UploadUpdateGastgezin(IFormFile file)
        {

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    maintenanceService.UpdateDataFromExcel(fileStream, 1);
                }
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", new MaintenanceModel());
            }
            catch
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File upload failed!!";
                return View("Index", new MaintenanceModel());
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadImportGastgezinnen(IFormFile file)
        {
            var model = new MaintenanceModel();

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var messages = maintenanceService.ImportGastgezinnen(fileStream).ToList();
                    model.Messages.AddRange(messages.Select(x => new MaintenanceMessage
                    {
                        Message = x.Message,
                        MessageType = (MaintenanceMessageType) x.MessageType
                    }));
                }
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", model);
            }
            catch
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File upload failed!!";
                return View("Index", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadGastgezinAanmeldFromIntake(IFormFile file)
        {
            var model = new MaintenanceModel();

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var messages = maintenanceService.UpdateAanmeldingFromIntakeId(fileStream).ToList();
                    model.Messages.AddRange(messages.Select(x => new MaintenanceMessage
                    {
                        Message = x.Message,
                        MessageType = (MaintenanceMessageType)x.MessageType
                    }));
                }
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index", model);
            }
            catch
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File upload failed!!";
                return View("Index", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateFormsForGastgezin(int gastgezinId, int aanmeldId, int intakeId)
        {
            var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
            if(gastgezin == null)
                return Redirect($"/gastgezin/maintenance?id={gastgezinId}&message=Gastgezin%20niet%20gevonden");
            var aanmeld = _reactionService.GetReactieFromId(aanmeldId);
            var intake = _reactionService.GetReactieFromId(intakeId);
            if (aanmeld == null || aanmeld.FormulierId != 1)
                return Redirect($"/gastgezin/maintenance?id={gastgezinId}&message=Aanmeld%20geen%20aanmeld%20formulier");
            if (intake != null && intake.FormulierId != 2)
                return Redirect($"/gastgezin/maintenance?id={gastgezinId}&message=Intake%20geen%20intake%20formulier");

            gastgezin.AanmeldFormulier = aanmeld;
            gastgezin.IntakeFormulier = intake;
            _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
            return Redirect("/gastgezin/maintenance?id=" + gastgezinId);
        }
    }
}
