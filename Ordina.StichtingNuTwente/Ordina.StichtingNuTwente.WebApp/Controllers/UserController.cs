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
        public UserController(IUserService userService)
        {
            _userService = userService;
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
            viewModel = users.ToList().ConvertAll(u => new UserViewModel(u));
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
    }
}
