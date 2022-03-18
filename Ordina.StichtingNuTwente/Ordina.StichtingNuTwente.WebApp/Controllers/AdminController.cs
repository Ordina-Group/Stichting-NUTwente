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
                var redirect = Request.Query["redirect"].ToString();

                if (loginDetails.Password == "NUTwente2022" && loginDetails.Username == "vrijwilliger")
                {
                    HttpContext.Session.SetString("loggedIn", "22D4B2BA-EA60-4CC7-AF9B-860B31A321CC");
                    return Redirect(string.IsNullOrEmpty(redirect) ? redirect : "getnutwenteoverheidreacties987456list");
                }
                else if (loginDetails.Password == "a3XM72RvW@" && loginDetails.Username == "intake")
                {
                    HttpContext.Session.SetString("loggedIn", "4F7F9757-4D80-42E8-8583-634503A6E387");
                    return Redirect(string.IsNullOrEmpty(redirect) ? redirect : "loginnutwentevrijwilligers");
                }
            }
            return View(loginDetails);
        }

        [Route("logout")]
        [HttpGet]
        public ActionResult logout()
        {
            HttpContext.Session.Remove("loggedIn");

            return Redirect("loginnutwentevrijwilligers");
        }
    }
}
