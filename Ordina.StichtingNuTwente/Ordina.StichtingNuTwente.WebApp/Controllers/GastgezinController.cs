using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.ViewModels;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Extensions;
using Microsoft.AspNetCore.Authorization;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Business.Services;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireVrijwilligerRole")]
    public class GastgezinController : Controller
    {
        private readonly IGastgezinService _gastgezinService;
        private readonly IPlaatsingenService _plaatsingenService;
        private readonly IUserService _userService;

        public GastgezinController(IGastgezinService gastgezinService, IPlaatsingenService plaatsingenService, IUserService userService, IMailService mailService)
        {
            _gastgezinService = gastgezinService;
            _plaatsingenService = plaatsingenService;
            _userService = userService;
        }

        [Route("gastgezin")]
        public IActionResult Gastgezin(int id, bool? EditPlaatsingen = false, bool? EditReserveringen = false, bool? EditVerwijderdePlaatsingen = false)
        {
            _userService.checkIfUserExists(User);
            var gastGezin = _gastgezinService.GetGastgezin(id);
            if (gastGezin == null)
            {
                return Redirect("Error");
            }

            //verify view permissions
            if (gastGezin.Intaker != null || gastGezin.Buddy != null)
            {
                if (!(gastGezin.Intaker?.AADId == _userService.getUserFromClaimsPrincipal(User).AADId || gastGezin.Buddy?.AADId == _userService.getUserFromClaimsPrincipal(User).AADId || User.HasClaims("groups", "group-secretariaat", "group-coordinator", "group-superadmin")))
                {
                    return Redirect("MicrosoftIdentity/Account/AccessDenied");
                }
            }
            else if (!User.HasClaims("groups", "group-secretariaat", "group-coordinator", "group-superadmin"))
            {
                return Redirect("MicrosoftIdentity/Account/AccessDenied");
            }

            var viewModel = new GastgezinDetailViewModel() { };
            var user = GetUser();
            if (gastGezin.Contact != null)
            {
                var plaatsingTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Plaatsing, gastGezin);
                var reserveTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                viewModel.GastGezin = gastgezinViewModel;
            }
            var PlacementViewModels = gastGezin.Plaatsingen.Select(p => new PlaatsingViewModel(p));

            viewModel.Plaatsingen = PlacementViewModels.Where(p => p.Active == true && (p.PlacementType == PlacementType.Plaatsing || p.PlacementType == PlacementType.GeplaatsteReservering)).ToList();
            viewModel.Reserveringen = PlacementViewModels.Where(p => p.Active == true && p.PlacementType == PlacementType.Reservering).ToList();
            viewModel.PlaatsingsGeschiedenis = PlacementViewModels.OrderByDescending(p => p.Id).ToList();
            viewModel.PlaatsingDTO = new PlaatsingDTO();

            //calculate plaatsing stats
            viewModel.PlaatsingStats = new PlaatsingStats();
            viewModel.PlaatsingStats.PlaatsVolwassen = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.PlaatsKind = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.PlaatsOnbekend = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == PlacementType.Plaatsing).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResVolwassen = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Volwassene && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResKind = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Kind && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);
            viewModel.PlaatsingStats.ResOnbekend = viewModel.PlaatsingsGeschiedenis.Where(p => p.AgeGroup == AgeGroup.Onbekend && p.PlacementType == PlacementType.Reservering).Sum(p => p.Amount);

            //check for edit mode
            if (EditPlaatsingen == true)
            {
                viewModel.EditPlaatsingen = true;
            }
            if (EditReserveringen == true)
            {
                viewModel.EditReserveringen = true;
            }
            if (EditVerwijderdePlaatsingen == true)
            {
                viewModel.EditVerwijderdePlaatsingen = true;
            }

            //check for delete permission
            var u = GetUser();
            viewModel.CanDelete = false;
            if (User.HasClaims("groups", "group-coordinator", "group-superadmin"))
                viewModel.CanDelete = true;

            return View(viewModel);
        }

        public IActionResult PostNote(int GastGezinId, string Note)
        {
            _gastgezinService.UpdateNote(GastGezinId, Note);
            return Redirect("/gastgezin?id=" + GastGezinId);
        }

        [HttpPost]
        public IActionResult PostVOG(bool HasVOG, int GastGezinId)
        {
            _gastgezinService.UpdateVOG(HasVOG, GastGezinId);
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateOpties(int GastGezinId, bool NoodOpvang, DateTime OnHoldTill, bool OnHold, bool HasVOG, int MaxYoungerThanThree, int MaxOlderThanTwo)
        {
            var gastgezin = _gastgezinService.GetGastgezin(GastGezinId);
            if (gastgezin != null)
            {
                gastgezin.NoodOpvang = NoodOpvang;
                gastgezin.OnHold = OnHold;
                if (OnHold)
                {
                    _plaatsingenService.RemoveReserveringOnHold(gastgezin, _userService.getUserFromClaimsPrincipal(User));
                }
                if (OnHold && OnHoldTill.Subtract(DateTime.Now).Hours > 0)
                {
                    gastgezin.OnHoldTill = OnHoldTill;
                }
                else if (!OnHold)
                {
                    gastgezin.OnHoldTill = null;
                }
                gastgezin.HasVOG = HasVOG;
                gastgezin.MaxOlderThanTwo = MaxOlderThanTwo;
                gastgezin.MaxYoungerThanThree = MaxYoungerThanThree;
                if (gastgezin.PlaatsingsInfo != null)
                {
                    gastgezin.PlaatsingsInfo.VolwassenenGrotereKinderen = MaxOlderThanTwo.ToString();
                    gastgezin.PlaatsingsInfo.KleineKinderen = MaxYoungerThanThree.ToString();
                }
                _gastgezinService.UpdateGastgezin(gastgezin, GastGezinId);
                return Redirect("/gastgezin?id=" + GastGezinId);
            }
            else return Redirect("Error");
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("/BeschikbareGastgezinnen")]
        [HttpGet]
        public IActionResult BeschikbareGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string[]? filters = null, string statusFilter = "")
        {
            _userService.checkIfUserExists(User);

            var model = _gastgezinService.BeschikBeschikbareGastgezinnen(sortBy, sortOrder, filters, statusFilter, GetUser());

            _gastgezinService.FillBaseModel(model, _userService.getUserFromClaimsPrincipal(User));
            return View(model);
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("{controller=Home}/{action=Index}/{id?}")]
        [HttpDelete]
        public IActionResult DeleteGastgezin(int id, string comment, bool deleteForms = true)
        {
            try
            {
                var userDetails = GetUser();
                if (userDetails != null)
                {
                    _gastgezinService.Delete(id, deleteForms, userDetails, comment == null ? "" : comment);
                    return Ok();
                }
                return BadRequest("No user found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("{controller=Home}/{action=Index}/{id?}")]
        [HttpPost]
        public IActionResult RestoreGastgezin(int id)
        {
            try
            {
                _gastgezinService.Restore(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// connects CurrentActiveUser with azureUser
        /// </summary>
        /// <returns></returns>
        public UserDetails? GetUser()
        {
            var aadID = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));
            if (aadID != null)
            {
                var userDetails = this._userService.GetUserByAADId(aadID.Value);
                return userDetails;
            }
            return null;
        }

        [HttpPut]
        [Route("MarkNewAsSeen/{gastgezinId}")]
        public IActionResult MarkNewAsSeen(int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null && gastgezin.Comments != null)
                {
                    gastgezin.Comments.RemoveAll(c => c.CommentType == CommentType.INTAKE_COMPLETED);
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPut]
        [Route("MarkAsRead/{gastgezinId}")]
        public IActionResult MarkAsRead(int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null)
                {
                    var user = GetUser();
                    if (!gastgezin.BekekenDoorIntaker && user?.Id == gastgezin.Intaker?.Id)
                    {
                        gastgezin.BekekenDoorIntaker = true;
                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    }

                    if (!gastgezin.BekekenDoorBuddy && user?.Id == gastgezin.Buddy?.Id)
                    {
                        gastgezin.BekekenDoorBuddy = true;
                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    }
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("RejectBuddy/{gastgezinId}")]
        public IActionResult RejectBeingBuddy(int gastgezinId, string comment) // NIET GEBRUIKT
        {
            try
            {
                var user = GetUser();
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null && user != null)
                {
                    _gastgezinService.RejectBeingBuddy(gastgezin, comment, user);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("MijnGastgezinnen/{userId}")]
        [HttpGet]
        [ActionName("MijnGastgezinnen")]
        public IActionResult MijnGastgezinnen(string? filter, int? userId, bool? editAddress)
        {
            _userService.checkIfUserExists(User);
            var user = _userService.GetUserById((int)userId);
            if (user == null)
                return Redirect("Error");
            var mijnGastgezinnen = FillMijnGastgezinnenModel(filter, user, editAddress);
            mijnGastgezinnen.GastgezinnenVan = mijnGastgezinnen.Vrijwilliger.Naam;
            return View(mijnGastgezinnen);
        }

        [Route("MijnGastgezinnen")]
        [HttpGet]
        [ActionName("MijnGastgezinnen")]
        public IActionResult MijnGastgezinnen(string? filter, bool? editAddress)
        {
            _userService.checkIfUserExists(User);
            var user = GetUser();
            if (user == null)
                return Redirect("Error");
            var mijnGastgezinnen = FillMijnGastgezinnenModel(filter, user, editAddress);
            _gastgezinService.FillBaseModel(mijnGastgezinnen, _userService.getUserFromClaimsPrincipal(User));
            return View(mijnGastgezinnen);
        }

        private MijnGastgezinnenModel FillMijnGastgezinnenModel(string? filter, UserDetails user, bool? editAddress)
        {
            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetGastgezinnenForVrijwilliger(user.Id);
            var intakerCount = gastGezinnen.Where(g => g.Intaker == user).Count();
            var buddyCount = gastGezinnen.Where(g => g.Buddy == user).Count();
            var mijnGastgezinnen = new MijnGastgezinnenModel(user, intakerCount, buddyCount);

            if (filter != null)
            {
                if (filter == "Buddy")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Buddy == user).ToList();
                }
                if (filter == "Intaker")
                {
                    gastGezinnen = gastGezinnen.Where(g => g.Intaker == user).ToList();
                }
            }

            foreach (var gastGezin in gastGezinnen)
            {
                var plaatsingTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Plaatsing, gastGezin);
                var reserveTag = _plaatsingenService.GetPlaatsingTag(PlacementType.Reservering, gastGezin);
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user, plaatsingTag, reserveTag);
                mijnGastgezinnen.MijnGastgezinnen.Add(gastgezinViewModel);
            }
            if(editAddress!= null)
            {
            mijnGastgezinnen.EditAddress = (bool)editAddress;
            }

            _gastgezinService.FillBaseModel(mijnGastgezinnen, _userService.getUserFromClaimsPrincipal(User));
            return mijnGastgezinnen;
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("AlleGastgezinnen")]
        [HttpGet]
        [ActionName("AlleGastgezinnen")]
        public IActionResult AlleGastgezinnen(string? sortBy = "Woonplaats", string? sortOrder = "Ascending", string statusFilter = "")
        {
            _userService.checkIfUserExists(User);
            var alleGastgezinnen = _gastgezinService.AlleGastgezinnen(sortBy, sortOrder, statusFilter, GetUser(), _userService.GetAllDropdownUsers().OrderBy(u => u.FirstName).ToList());

            _gastgezinService.FillBaseModel(alleGastgezinnen, _userService.getUserFromClaimsPrincipal(User));
            return View(alleGastgezinnen);
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [HttpPost]
        public IActionResult IntakerOrBuddyChange(List<IntakerOrBuddyChangeModel> intakerOrBuddyChangeModels)
        {
            bool succes = _gastgezinService.IntakerOrBuddyChange(intakerOrBuddyChangeModels, _userService.GetAllDropdownUsers().ToList());
            if (!succes)
                return BadRequest();
            return Ok();
        }

        [Authorize(Policy = "RequireSecretariaatRole")]
        [Route("VerwijderdeGastgezinnen")]
        [HttpGet]
        [ActionName("VerwijderdeGastgezinnen")]
        public IActionResult VerwijderdeGastgezinnen()
        {
            _userService.checkIfUserExists(User);

            var model = new List<GastgezinViewModel>();

            var user = GetUser();
            ICollection<Gastgezin> gastGezinnen = _gastgezinService.GetDeletedGastgezinnen();

            foreach (var gastGezin in gastGezinnen)
            {
                var gastgezinViewModel = GastgezinMapping.FromDatabaseToWebModel(gastGezin, user);
                model.Add(gastgezinViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateCommentVrijwilliger(string comments, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null)
                {
                    gastgezin.VrijwilligerOpmerkingen = comments == null ? "" : comments;
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [HttpPost]
        public IActionResult UpdateCommentCoordinator(string comments, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                if (gastgezin != null)
                {
                    gastgezin.CoordinatorOpmerkingen = comments == null ? "" : comments;
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult AddContactLog(DateTime date, string note, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                var user = GetUser();
                if (gastgezin != null && user != null)
                {
                    gastgezin.ContactLogs.Add(new ContactLog() { Notes = note, DateTime = date, Contacter = user });
                    _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                    return Ok();
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [HttpDelete]
        public IActionResult DeleteContactLog(int contactLogId, int gastgezinId)
        {
            try
            {
                var gastgezin = _gastgezinService.GetGastgezin(gastgezinId);
                var user = GetUser();
                if (gastgezin != null && user != null)
                {
                    var contactLog = gastgezin.ContactLogs.FirstOrDefault(c => c.Id == contactLogId);
                    if (contactLog != null && (contactLog.Contacter.AADId == GetUser().AADId) || User.HasClaims("groups", "group-coordinator", "group-superadmin"))
                    {
                        gastgezin.ContactLogs.Remove(contactLog);
                        _gastgezinService.UpdateGastgezin(gastgezin, gastgezinId);
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(403);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetGastgezinInformation() // NIET GEBRUIKT
        {
            var amountGastgezinnen = _gastgezinService.GetAllGastgezinnen().Where(g => g.Status == GastgezinStatus.Geplaatst);
            var amountGeplaatsteVluchtelingen = _plaatsingenService.GetPlaatsingen().Where(p => p.Active && (p.PlacementType == PlacementType.GeplaatsteReservering || p.PlacementType == PlacementType.Plaatsing));


            var returnModel = new GastgezinStatsViewModel(amountGeplaatsteVluchtelingen.Count(), amountGastgezinnen.Count());

            return Ok(returnModel);
        }
    }
}
