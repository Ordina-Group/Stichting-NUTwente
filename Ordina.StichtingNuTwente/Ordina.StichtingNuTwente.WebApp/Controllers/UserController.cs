using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Extensions;
using System.Diagnostics;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Azure.Identity;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    public class UserController : Controller
    {
        public IUserService _userService { get; }
        public IGastgezinService _gastgezinService { get; }
        public IMailService _mailService { get; }
        private readonly GraphServiceClient _graphServiceClient;

        public UserController(IUserService userService, IGastgezinService gastgezinService, IMailService MailService, GraphServiceClient graphServiceClient)
        {
            _userService = userService;
            _gastgezinService = gastgezinService;
            _mailService = MailService;
            _graphServiceClient = graphServiceClient;
        }

        [AllowAnonymous]
        [Route("Account/SignOut")]
        public IActionResult SignOutCatch()
        {
            return Redirect("/");
        }

        [AllowAnonymous]
        [Route("/MicrosoftIdentity/Account/AccessDenied")]
        [Route("/AccessDenied")]
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
            List<UserViewModel> viewModel = new();
            var users = _userService.GetAllUsers().OrderBy(u => u.FirstName);
            var allGastgezinnen = _gastgezinService.GetAllGastgezinnen("Buddy,Intaker");
            foreach (var u in users)
            {
                var gastgezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(u.Id, allGastgezinnen);
                var aantalBuddies = gastgezinnen.Count(g => g.Buddy?.Id == u.Id);
                var aantalIntakes = gastgezinnen.Count(g => g.Intaker?.Id == u.Id);
                viewModel.Add(new UserViewModel(u) { AantalBuddies = aantalBuddies, AantalIntakes = aantalIntakes });
            }
            return View(viewModel);
        }

        [Authorize]
        [Route("/")]
        [Route("/MicrosoftIdentity/Account/Error")]
        public IActionResult Overview()
        {
            _userService.checkIfUserExists(User);
            if (User.HasClaim("groups", "group-coordinator"))
            {
                return Redirect("/BeschikbareGastgezinnen");
            }
            else if (User.HasClaim("groups", "group-secretariaat"))
            {
                return Redirect("/AlleGastgezinnen");
            }
            else if (User.HasClaim("groups", "group-vrijwilliger"))
            {
                return Redirect("/MijnGastgezinnen");
            }
            UserDetails userDetails = _userService.GetUserByAADId(User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier")).Value);
            return View(new UserViewModel(userDetails));
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
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
        
        ////[Authorize]
        //[AllowAnonymous]
        //// [Route("user/MailGroup")]
        //[HttpPost]
        ////[ActionName("MailGroup")]
        //public IActionResult MailGroup(string onderwerp, string bericht, string emailAdressen)
        //{

        //    List<string> mailAdressen = emailAdressen.Split(',').ToList();
        //    MailHelper(onderwerp, bericht, mailAdressen);
        //    return Ok();
        //}

        [Authorize(Policy = "RequireVrijwilligerRole")]
        public async Task<IActionResult> UpdateUserAddressAsync(int userId, string address = "", string city = "", string postalCode = "")
        {
            var user = _userService.GetUserById(userId);
            var currentUser = _userService.getUserFromClaimsPrincipal(User);

            if (user.AADId == currentUser.AADId || User.HasClaims("groups", "group-coordinator", "group-superadmin"))
            {
                if(user.Address == null)
                {
                    user.Address = new Adres()
                    {
                        Straat = address,
                        Woonplaats = city,
                        Postcode = postalCode
                    };
                }
                else
                {
                    user.Address.Straat = address;
                    user.Address.Woonplaats = city;
                    user.Address.Postcode = postalCode;
                }
                _userService.UpdateUser(user, userId);

                //Example of how to use Microsoft Graph
                var aadUser = new User
                {
                    PostalCode = postalCode,
                    City = city,
                    StreetAddress = address,
                };
                await _graphServiceClient.Users[user.AADId].Request().UpdateAsync(aadUser);

                if (user.AADId == currentUser.AADId)
                {
                    return Redirect("/MijnGastgezinnen");
                }
                else
                {
                    return Redirect($"/MijnGastgezinnen/{userId}");
                }
            }
            return Redirect("Error");
        }

        [Authorize(Policy ="RequireCoordinatorRole")]
        [Route("/User/{id}/Delete")]
        [HttpDelete]
        public IActionResult Delete(int id, string comment)
        {
            try
            {
                var userDetails = _userService.getUserFromClaimsPrincipal(User);
                if (userDetails != null)
                {
                    var success = _userService.Delete(id, userDetails, comment == null ? "" : comment);
                    if (success)
                        return Ok();
                    else
                        return NotFound();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("/User/{id}/Restore")]
        [HttpPost]
        public IActionResult Restore(int id)
        {
            try
            {
                var success = _userService.Restore(id);
                if (success)
                    return Ok();
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("/Archief/Vrijwilligers")]
        [HttpGet]
        public IActionResult DeletedUsers()
        {
            _userService.checkIfUserExists(User);
            List<UserViewModel> viewModel = new();
            var users = _userService.GetAllDeletedUsers().OrderBy(u => u.FirstName);
            var allGastgezinnen = _gastgezinService.GetAllGastgezinnen("Buddy,Intaker");
            foreach (var u in users)
            {
                var gastgezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(u.Id, allGastgezinnen);
                var aantalBuddies = gastgezinnen.Count(g => g.Buddy?.Id == u.Id);
                var aantalIntakes = gastgezinnen.Count(g => g.Intaker?.Id == u.Id);
                viewModel.Add(new UserViewModel(u) { AantalBuddies = aantalBuddies, AantalIntakes = aantalIntakes });
            }
            return View(viewModel);
        }

    }
}
