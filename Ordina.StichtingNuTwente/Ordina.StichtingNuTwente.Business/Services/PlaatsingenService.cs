﻿using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class PlaatsingenService : IPlaatsingenService
    {
        private readonly IRepository<Plaatsing> PlaatsingenRepository;

        public PlaatsingenService(IRepository<Plaatsing> plaatsingenRepository)
        {
            PlaatsingenRepository = plaatsingenRepository;
        }

        public ICollection<Plaatsing> GetAllPlaatsingen(PlacementType? placementType)
        {
            var plaatsingen = PlaatsingenRepository.GetAll("Gastgezin,Gastgezin.Contact,Vrijwilliger").Where(p => p.PlacementType == placementType);
            return plaatsingen.ToList();
        }

        public Plaatsing CheckAge(Plaatsing plaatsing)
        {
            if (plaatsing.Age != -1)
            {
                if (plaatsing.Age > 120) // Te oud
                {
                    plaatsing.Age = 120;
                }
                else if (plaatsing.Age > 17) // Volwassen
                {
                    plaatsing.AgeGroup = AgeGroup.Volwassene;
                }
                else if (plaatsing.Age < 18 && plaatsing.Age >= 0) // Kind
                {
                    plaatsing.AgeGroup = AgeGroup.Kind;
                }
                else if (plaatsing.Age < -1) // Onbekend
                {
                    plaatsing.Age = -1;
                }
            }
            return plaatsing;
        }
        public void AddPlaatsing(Plaatsing plaatsing)
        {
            PlaatsingenRepository.Create(CheckAge(plaatsing));
        }
        public void UpdatePlaatsing(Plaatsing plaatsing)
        {
            PlaatsingenRepository.Update(CheckAge(plaatsing));
        }

        public Plaatsing GetPlaatsing(int id)
        {
            var plaatsing = PlaatsingenRepository.GetById(id, "Gastgezin");
            return plaatsing;
        }
        public void RemoveReserveringPlaatsingen(DateTime departureDate, int plaatsingId, string departureReason, UserDetails user, DepartureDestination? departureDestination = null, string? departureComment = null)
        {
            var plaatsing = GetPlaatsing(plaatsingId);
            plaatsing.Active = false;
            UpdatePlaatsing(plaatsing);
            var placementType = PlacementType.VerwijderdePlaatsing;
            if (plaatsing.PlacementType == PlacementType.Reservering)
            {
                placementType = PlacementType.VerwijderdeReservering;
            }
            var deletedPlaatsing = new Plaatsing()
            {
                Gastgezin = plaatsing.Gastgezin,
                Amount = plaatsing.Amount,
                Age = plaatsing.Age,
                AgeGroup = plaatsing.AgeGroup,
                PlacementType = placementType,
                DateTime = departureDate,
                Vrijwilliger = user,
                Active = false,
                Gender = plaatsing.Gender,
                DepartureReason = departureReason,
                DepartureDestination = departureDestination,
                DepartureComment = departureComment
            };
            AddPlaatsing(deletedPlaatsing);
        }

        public void RemoveReserveringOnHold(Gastgezin gastgezin, UserDetails user)
        {
            var plaatsingen = gastgezin.Plaatsingen.Where(p => p.Active && p.PlacementType == PlacementType.Reservering).ToList();
            foreach (var plaatsing in plaatsingen)
            {
                if (plaatsing.Active && plaatsing.PlacementType == PlacementType.Reservering)
                {
                    RemoveReserveringPlaatsingen(DateTime.Now, plaatsing.Id, "Gastgezin on hold gezet", user);
                }
            }
        }

        public List<Plaatsing> GetPlaatsingen(int? gastGezinId = null, PlacementType? type = null, AgeGroup? ageGroup = null)
        {
            var plaatsingen = PlaatsingenRepository.GetAll("Gastgezin");
            if (gastGezinId != null)
            {
                plaatsingen = plaatsingen.Where(p => p.Gastgezin.Id == gastGezinId);
            }
            if (type != null)
            {
                plaatsingen = plaatsingen.Where(p => p.PlacementType == type);
            }
            if (ageGroup != null)
            {
                plaatsingen = plaatsingen.Where(p => p.AgeGroup == ageGroup);
            }
            //List<Plaatsing> plaatsingen = new();
            return plaatsingen.ToList();
        }

        public string GetPlaatsingTag(PlacementType placementType, Gastgezin gastgezin)// Spatie er achter voor het nummer in de front-end
        {
            string status = "";
            var gastgezinStatus = gastgezin.Status;

            if (gastgezin.OnHold)
            {
                status = "ON HOLD";
            }
            else if (gastgezin.NoodOpvang)
            {
                status = "NOOD";
            }
            var plaatsingen = gastgezin.Plaatsingen.Where(p => p.Active == true);
            int? total = plaatsingen.Where(p => p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) total += plaatsingen.Where(p => p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);
            string tag = "";
            if (total == 0 && status != "")
            {
                tag = status;
            }
            else
            {
                tag = status + " " + total;
            }
            return tag;
        }
        public string GetPlaatsingenTag(List<Gastgezin> gastgezinnen, PlacementType placementType)
        {
            var plaatsingen = new List<Plaatsing>();

            gastgezinnen.ForEach(g => plaatsingen.AddRange(g.Plaatsingen));
            plaatsingen = plaatsingen.Where(p => p.Active == true).ToList();
            int? total = plaatsingen.Where(p => p.PlacementType == placementType).Sum(p => p.Amount);
            if (placementType == PlacementType.Plaatsing) total += plaatsingen.Where(p => p.PlacementType == PlacementType.GeplaatsteReservering).Sum(p => p.Amount);
            string tag = total.ToString();
            return tag;
        }

        public bool PlaatsingExists(int gastgezinId, Plaatsing plaatsing)
        {
            var plaastingen = GetPlaatsingen(gastgezinId, plaatsing.PlacementType, plaatsing.AgeGroup);
            if (plaastingen.FirstOrDefault(p => p.DateTime == plaatsing.DateTime && p.Amount == plaatsing.Amount) != null)
            {
                return true;
            }
            return false;
        }
    }
}
