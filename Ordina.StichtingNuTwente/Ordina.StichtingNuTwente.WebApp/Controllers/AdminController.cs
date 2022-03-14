using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IReactionService _reactionService;

        public AdminController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [Route("loginnutwentevrijwilligers")]
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }
        [Route("loginnutwentevrijwilligers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(Login loginDetails)
        {
            if (ModelState.IsValid)
            {
                if (loginDetails.Password == "NUTwente2022" && loginDetails.Username == "vrijwilliger")
                {
                    HttpContext.Session.SetString("loggedIn", "22D4B2BA-EA60-4CC7-AF9B-860B31A321CC");
                    return Redirect("getnutwenteoverheidreacties987456list");
                }
            }

            return View(loginDetails);
        }
    }
}
