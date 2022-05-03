using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class UserController : Controller
    {
        public IUserService _userService { get; }
        public IMailService _mailService { get; }
        public UserController(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
        }

        [Authorize]
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

        [Authorize(Policy = "RequireSecretariaatRole")]
        public IActionResult Users()
        {
            _userService.checkIfUserExists(User);
            List<UserViewModel> viewModel = new List<UserViewModel>();
            var users = _userService.GetUsersByRole("group-vrijwilliger");
            users.Concat(_userService.GetUsersByRole("group-superadmin"));
            viewModel = users.ToList().ConvertAll(u => new UserViewModel(u)
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Roles = u.Roles
            });
            return View(viewModel);
        }


        [Authorize]
        public IActionResult Overview()
        {
            _userService.checkIfUserExists(User);
            UserDetails userDetails = _userService.GetUserByAADId(User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value);
            return View(new UserViewModel(userDetails));
        }

        //[Authorize]
        [AllowAnonymous]
       // [Route("user/MailGroup")]
        [HttpPost]
        //[ActionName("MailGroup")]
        public IActionResult MailGroup(string onderwerp, string bericht, string emailAdressen)
        {

            List<string> mailAdressen = emailAdressen.Split(',').ToList();
            _mailService.setApiKey("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");
            _mailService.setFromMail("niek.nieuwenhuisen@ordina.nl");
            _mailService.sendGroupMail(onderwerp, bericht, mailAdressen);

            return Ok();
        }

    }
}
