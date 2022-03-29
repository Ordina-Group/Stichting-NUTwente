using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireSuperAdminRole")]
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly IWebHostEnvironment _environment;
        public MaintenanceController(IMaintenanceService maintenanceService, IWebHostEnvironment environment)
        {
            this.maintenanceService = maintenanceService;
            _environment = environment;
        }



        public IActionResult Index()
        {
            return View();
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
                return View();
            }
            catch
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists) fileInfo.Delete();
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }
    }
}
