using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Diagnostics;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class UserController : Controller
    {
        public IUserService _userService { get; }
        public IGastgezinService _gastgezinService { get; }
        public IMailService _mailService { get; }

        public UserController(IUserService userService, IGastgezinService gastgezinService, IMailService mailService)
        {
            _userService = userService;
            _gastgezinService = gastgezinService;
            _mailService = mailService;
        }

        [AllowAnonymous]
        [Route("Account/SignOut")]
        public IActionResult SignOutCatch()
        {
            return Redirect("/User/Overview");
        }

        [Authorize]
        [Route("MicrosoftIdentity/Account/AccessDenied")]
        public IActionResult AccessDeniedCatch()
        {
            _userService.checkIfUserExists(User);
            return View();
        }

        [Route("/Vrijwilligers")]
        [Authorize(Policy = "RequireSecretariaatRole")]
        public IActionResult Users()
        {
            _userService.checkIfUserExists(User);
            List<UserViewModel> viewModel = new List<UserViewModel>();
            var users = _userService.GetAllUsers();
            var allGastgezinnen = _gastgezinService.GetAllGastgezinnen();
            foreach (var u in users)
            {
                var gastgezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(u.Id, allGastgezinnen);
                var aantalBuddies = gastgezinnen.Count(g => g.Buddy?.Id == u.Id);
                var aantalIntakes = gastgezinnen.Count(g => g.Begeleider?.Id == u.Id);
                viewModel.Add(new UserViewModel(u) { AantalBuddies = aantalBuddies, AantalIntakes = aantalIntakes});

            }
            return View(viewModel);
        }


        [Authorize]
        public IActionResult Overview()
        {
            _userService.checkIfUserExists(User);
            UserDetails userDetails = _userService.GetUserByAADId(User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value);
            return View(new UserViewModel(userDetails));
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [HttpPut]
        public IActionResult UserUpdate(int id, bool inDropdown)
        {
            var updatedUser = _userService.GetUserById(id);
            if (updatedUser != null)
            {
                try
                {
                    updatedUser.InDropdown = inDropdown;
                    _userService.UpdateUser(updatedUser, id);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        //[Authorize]
        [AllowAnonymous]
       // [Route("user/MailGroup")]
        [HttpPost]
        //[ActionName("MailGroup")]
        public IActionResult MailGroup(string onderwerp, string bericht, string emailAdressen)
        {

            List<string> mailAdressen = emailAdressen.Split(',').ToList();
            _mailService.SetApiKey("***REMOVED***");
            _mailService.SetFromMail("niek.nieuwenhuisen@ordina.nl");
            _mailService.SendGroupMail(onderwerp, bericht, mailAdressen);

            return Ok();
        }

    }
}
