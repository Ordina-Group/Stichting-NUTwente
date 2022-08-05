using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly IReactionService _reactionService;
        private readonly IGastgezinService _gastgezinService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        public MaintenanceController(IMaintenanceService maintenanceService, IWebHostEnvironment environment, IReactionService reactionService, IGastgezinService gastgezinService, IUserService userService)
        {
            this.maintenanceService = maintenanceService;
            _environment = environment;
            _reactionService = reactionService;
            _gastgezinService = gastgezinService;
            _userService = userService;
        }

        [Authorize(Policy = "RequireSuperAdminRole")]
        [Route("/gastgezin/maintenance")]
        public IActionResult GastgezinMaintenance(int id)
        {
            var gastGezin = _gastgezinService.GetGastgezin(id);
            if (gastGezin == null)
            {
                return Redirect("Error");
            }

            var viewModel = new GastgezinMaintenanceViewModel() { };
            if (gastGezin.Contact != null)
            {
                viewModel.Gastgezin = GastgezinMapping.FromDatabaseToWebModel(gastGezin, new UserDetails());
            }
            var vrijwilligers = _userService.GetAllDropdownUsers().OrderBy(u => u.FirstName).ThenBy(e => e.LastName).ToList();
            foreach (var vrijwilliger in vrijwilligers)
            {
                viewModel.Vrijwilligers.Add(new Vrijwilliger(vrijwilliger));
            }

            return View(viewModel);
        }

        [Authorize(Policy = "RequireSuperAdminRole")]
        public IActionResult Index()
        {
            return View(new MaintenanceModel());
        }
        [Authorize(Policy = "RequireSuperAdminRole")]
        public IActionResult LinkIntakerToGastgezin()
        {
            try
            {
                var model = new MaintenanceModel();
                var messages = maintenanceService.LinkIntakerToGastgezin();
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

        [Authorize(Policy = "RequireSuperAdminRole")]
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

        [Authorize(Policy = "RequireSuperAdminRole")]
        public IActionResult DatabaseIntegrity()
        {
            var databaseIntegrityModel = maintenanceService.TestDatabaseIntegrity();
            return View(databaseIntegrityModel);
        }

        [Authorize(Policy = "RequireSuperAdminRole")]
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

        [Authorize(Policy = "RequireSuperAdminRole")]
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

        [Authorize(Policy = "RequireSuperAdminRole")]
        [HttpPost]
        public async Task<ActionResult> UploadNewIntake(IFormFile file)
        {

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            try
            {
                var maintenanceModel = new MaintenanceModel();
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var messages = maintenanceService.LoadDataFromExcel(fileStream, 2);
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

        [Authorize(Policy = "RequireSuperAdminRole")]
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

        [Authorize(Policy = "RequireSuperAdminRole")]
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

        [Authorize(Policy = "RequireSuperAdminRole")]
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

        [Authorize(Policy = "RequireSuperAdminRole")]
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

        [Authorize(Policy = "RequireSuperAdminRole")]
        [HttpPost]
        public async Task<ActionResult> UpdateFormsForGastgezin(int gastgezinId, int aanmeldId, int intakeId)
        {
            var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
            if (gastgezin == null)
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

        [Authorize(Policy = "RequireSuperAdminRole")]
        [HttpPost]
        public async Task<ActionResult> UpdateIntakerGastgezin(int gastgezinId, int Intaker)
        {
            var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);

            if (Intaker > 0 && gastgezin != null)
            {
                var intaker = _userService.GetUserById(Intaker);
                gastgezin.Intaker = intaker;
                _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                return Redirect($"/gastgezin/maintenance?id={gastgezinId}");
            }
            //if (gastgezin == null)
            //    return Redirect($"/gastgezin/maintenance?id={gastgezinId}&message=Gastgezin%20niet%20gevonden");
            //var aanmeld = _reactionService.GetReactieFromId(aanmeldId);
            //var intake = _reactionService.GetReactieFromId(intakeId);
            //if (aanmeld == null || aanmeld.FormulierId != 1)
            //    return Redirect($"/gastgezin/maintenance?id={gastgezinId}&message=Aanmeld%20geen%20aanmeld%20formulier");
            //if (intake != null && intake.FormulierId != 2)
            //    return Redirect($"/gastgezin/maintenance?id={gastgezinId}&message=Intake%20geen%20intake%20formulier");
            //
            //gastgezin.AanmeldFormulier = aanmeld;
            //_gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
            return Redirect($"/gastgezin/maintenance?id={gastgezinId}&message=Gastgezin%20niet%20gevonden%20of%20intaker%20incorrect");
        }

        [Authorize(Policy = "RequireSuperAdminRole")]
        [HttpPost]
        public async Task<ActionResult> UploadGastgezinCapacity(IFormFile file)
        {
            var model = new MaintenanceModel();

            string uploads = Path.Combine(_environment.WebRootPath, "");
            string filePath = Path.Combine(uploads, Guid.NewGuid().ToString() + ".xlxs");
            try
            {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    var messages = maintenanceService.LoadCapacityFromExcel(fileStream).ToList();
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

        [Authorize(Policy = "RequireSuperAdminRole")]
        public IActionResult FixStatus()
        {
            var model = new MaintenanceModel();
            var messages = maintenanceService.UpdateStatus();
            model.Messages = new List<MaintenanceMessage>();
            model.Messages.AddRange(messages.Select(x => new MaintenanceMessage
            {
                Message = x.Message,
                MessageType = (MaintenanceMessageType)x.MessageType
            }));
            return View("Index", model);
        }

        [Authorize(Policy = "RequireSuperAdminRole")]
        public IActionResult DuplicateComments()
        {
            var model = new MaintenanceModel();
            var messages = maintenanceService.DuplicateComments();
            model.Messages = new List<MaintenanceMessage>();
            model.Messages.AddRange(messages.Select(x => new MaintenanceMessage
            {
                Message = x.Message,
                MessageType = (MaintenanceMessageType)x.MessageType
            }));
            return View("Index", model);
        }
    }
}
