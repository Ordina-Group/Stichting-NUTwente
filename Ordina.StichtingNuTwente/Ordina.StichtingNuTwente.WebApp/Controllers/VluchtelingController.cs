using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class VluchtelingController : Controller
    {

        private readonly IPersoonService _persoonService;
        private readonly IUserService _userService;

        public VluchtelingController(IPersoonService persoonservice, IUserService userService)
        {
            _persoonService = persoonservice;
            _userService = userService;
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("vluchtelingenOverzicht")]
        public IActionResult Vluchtelingen()
        {
            _userService.checkIfUserExists(User);
            List<VluchtelingListView> viewModel = new List<VluchtelingListView>();
            var vluchtelingen = _persoonService.GetAllVluchtelingen();
            viewModel = vluchtelingen.ToList().ConvertAll(v => new VluchtelingListView(v));
            return View(viewModel);
        }
    }
}
