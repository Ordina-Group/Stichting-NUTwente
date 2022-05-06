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
        private readonly IWebHostEnvironment _environment;
        public MaintenanceController(IMaintenanceService maintenanceService, IWebHostEnvironment environment, IReactionService reactionService)
        {
            this.maintenanceService = maintenanceService;
            _environment = environment;
            _reactionService = reactionService;
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

    }
}
