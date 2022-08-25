using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireSecretariaatRole")]
    public class DocumentController : Controller
    {
        private readonly IDocumentService documentService;

        public DocumentController(IDocumentService documentService)
        {
            this.documentService = documentService;
        }

        public IActionResult Index()
        {       
            return View();
        }
        public IActionResult GetGastgezinnnenPerGemeente()
        {
            var file = documentService.GenerateGastgezinnenPerGemeente();
            MemoryStream stream = new MemoryStream(file);
            return new FileStreamResult(stream, "application/txt") { FileDownloadName = string.Format("Aantal plaatsingen GG {0:dd-MM-yyyy}.txt", DateTime.Now) };
        }

        public IActionResult DataDump()
        {
            var file = documentService.GenerateDataDumpToExcel();

            MemoryStream stream = new MemoryStream(file);
            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") { FileDownloadName = string.Format("Data dump {0:dd-MM-yyyy}.xlsx", DateTime.Now) };
        }
    }
}
