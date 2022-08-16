using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Business.Services;
using Ordina.StichtingNuTwente.Models.Models;

namespace Ordina.StichtingNuTwente.WebApp.Controllers
{
    [Authorize(Policy = "RequireSecretariaatRole")]
    [Route("Plaatsingen")]
    public class PlaatsingenController : Controller
    {
        private readonly IPlaatsingenService _plaatsingenService;
        private readonly IGastgezinService _gastgezinService;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;

        public PlaatsingenController(IPlaatsingenService plaatsingenService, IGastgezinService gastgezinService, IMailService mailService, IUserService userService)
        {
            _plaatsingenService = plaatsingenService;
            _gastgezinService = gastgezinService;
            _mailService = mailService;
            _userService = userService;
        }

        [Route("Verwijderd")]
        [HttpGet]
        public IActionResult Verwijderd()
        {
            var plaatsingen = _plaatsingenService.GetAllPlaatsingen(PlacementType.VerwijderdePlaatsing).Where(p => p.DepartureDestination != null && p.DepartureDestination != DepartureDestination.Correctie).ToList();
            return View(plaatsingen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireCoordinatorRole")]
        [ActionName("PostPlaatsing")]
        [Route("PlaatsingenController/PostPlaatsing")]
        public IActionResult PostPlaatsing(int GastGezinId, PlacementType PlacementType, Gender Gender, AgeGroup AgeGroup, string Date, int Age = -1, int Amount = 1)
        {
            var plaatsType = PlacementType;

            for (int i = 0; i < Amount; i++)
            {
                var plaatsing = new Plaatsing()
                {
                    Gastgezin = _gastgezinService.GetGastgezin(GastGezinId),
                    Amount = 1,
                    Age = Age,
                    AgeGroup = AgeGroup,
                    PlacementType = plaatsType,
                    DateTime = DateTime.Parse(Date),
                    Vrijwilliger = _userService.getUserFromClaimsPrincipal(User),
                    Active = true,
                    Gender = Gender
                };
                _plaatsingenService.AddPlaatsing(plaatsing);
            }


            //TODO: zorgen dat er niet per losse plaatsing een mail wordt verstuurd
            //MailService.PlaatsingVluchteling(plaatsing);

            return Redirect("/gastgezin?id=" + GastGezinId);
        }

        [HttpPost]
        [Authorize(Policy = "RequireCoordinatorRole")]
        [ActionName("SendPlaatsingenEmail")]
        [Route("Plaatsingen/SendPlaatsingenEmail")]
        public bool SendPlaatsingenEmail(int GastGezinId)
        {
            _mailService.PlaatsingVluchteling(_gastgezinService.GetGastgezin(GastGezinId));

            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireCoordinatorRole")]
        public IActionResult UpdatePlaatsingen(IFormCollection formCollection)
        {
            string gastgezinIdString = formCollection.FirstOrDefault(k => k.Key.StartsWith("GastgezinId")).Value;

            if (Int32.TryParse(gastgezinIdString, out int gastgezinId))
            {
                var plaatsingIds = formCollection.Where(k => k.Key.StartsWith("PlaatsingsId")).Select(k => k.Value);

                foreach (var id in plaatsingIds)
                {
                    if (int.TryParse(id, out int plaatsingId))
                    {
                        var plaatsing = _plaatsingenService.GetPlaatsing(plaatsingId);

                        if (plaatsing != null)
                        {
                            var plaatsingData = formCollection.Where(k => k.Key.EndsWith(id));

                            foreach (var keyValuePair in plaatsingData)
                            {
                                var key = keyValuePair.Key;
                                var value = keyValuePair.Value;

                                if (key.StartsWith("Date") && DateTime.TryParse(value, out DateTime dateTime))
                                {
                                    plaatsing.DateTime = dateTime;
                                }
                                else if (key.StartsWith("Gender") && Enum.TryParse(value, out Gender gender))
                                {
                                    plaatsing.Gender = gender;
                                }
                                else if (key.StartsWith("Age_") && int.TryParse(value, out int age))
                                {
                                    plaatsing.Age = age;
                                }
                                else if (key.StartsWith("AgeGroup") && Enum.TryParse(value, out AgeGroup ageGroup))
                                {
                                    plaatsing.AgeGroup = ageGroup;
                                }
                                else if (key.StartsWith("DepartureReason"))
                                {
                                    plaatsing.DepartureReason = value;
                                }
                                else if (key.StartsWith("DepartureComment"))
                                {
                                    plaatsing.DepartureComment = value;
                                }
                                else if (key.StartsWith("DepartureDestination") && Enum.TryParse(value, out DepartureDestination departureDestination))
                                {
                                    plaatsing.DepartureDestination = departureDestination;
                                }
                            }
                            _plaatsingenService.UpdatePlaatsing(plaatsing);
                        }
                    }
                }
                return Redirect("/gastgezin?id=" + gastgezinId);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("DeletePlaatsing")]
        public IActionResult DeletePlaatsing(DateTime departureDate, int plaatsingId, string departureReason, DepartureDestination departureDestination, string departureComment)
        {
            _plaatsingenService.RemoveReserveringPlaatsingen(departureDate, plaatsingId, departureReason, _userService.getUserFromClaimsPrincipal(User), departureDestination, departureComment);
            return Redirect("/gastgezin?id=" + _plaatsingenService.GetPlaatsing(plaatsingId).Gastgezin.Id);
        }

        [Authorize(Policy = "RequireCoordinatorRole")]
        [Route("PlaatsReservering")]
        public IActionResult PlaatsReservering(int plaatsingId)
        {
            var plaatsing = _plaatsingenService.GetPlaatsing(plaatsingId);
            plaatsing.Active = false;
            _plaatsingenService.UpdatePlaatsing(plaatsing);
            var NieuwePlaatsing = new Plaatsing()
            {
                Gastgezin = plaatsing.Gastgezin,
                Amount = plaatsing.Amount,
                Age = plaatsing.Age,
                AgeGroup = plaatsing.AgeGroup,
                PlacementType = PlacementType.GeplaatsteReservering,
                DateTime = DateTime.Now,
                Vrijwilliger = _userService.getUserFromClaimsPrincipal(User),
                Active = true,
                Gender = plaatsing.Gender
            };
            _plaatsingenService.AddPlaatsing(NieuwePlaatsing);
            return Redirect("/gastgezin?id=" + plaatsing.Gastgezin.Id);
        }
    }
}
