using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Mappings
{
    public static class GastgezinMapping
    {
        public static GastgezinViewModel? FromDatabaseToWebModel(Gastgezin gastgezin, UserDetails user, string plaatsingTag = "", string ReserveTag = "")
        {
            if (gastgezin.Contact == null)
            {
                return null;
            }

            var contact = gastgezin.Contact;
            var adres = gastgezin.Contact.Adres;
            var adresText = "";
            var woonplaatsText = "";

            if (adres != null)
            {
                adresText = adres.Straat;
                woonplaatsText = adres.Woonplaats;
            }

            int aanmeldFormulierId = 0;
            int intakeFormulierId = 0;

            if (gastgezin.Contact.Reactie != null)
            {
                aanmeldFormulierId = gastgezin.Contact.Reactie.Id;
            }

            if (gastgezin.IntakeFormulier != null)
            {
                intakeFormulierId = gastgezin.IntakeFormulier.Id;
            }
            var heeftBekeken = false;
            if (user.Id == gastgezin.Buddy?.Id)
            {
                heeftBekeken = gastgezin.BekekenDoorBuddy;
            }

            if (user.Id == gastgezin.Begeleider?.Id)
            {
                heeftBekeken = gastgezin.BekekenDoorIntaker;
            }

            var gastgezinViewModel = new GastgezinViewModel
            {
                Id = gastgezin.Id,
                Adres = adresText,
                Email = contact.Email,
                Naam = contact.Naam + " " + contact.Achternaam,
                Telefoonnummer = contact.Telefoonnummer,
                Woonplaats = woonplaatsText,
                AanmeldFormulierId = aanmeldFormulierId,
                IntakeFormulierId = intakeFormulierId,
                PlaatsingTag = plaatsingTag,
                ReserveTag = ReserveTag,
                PlaatsingsInfo = gastgezin.PlaatsingsInfo,
                HasVOG = gastgezin.HasVOG,
                HeeftBekeken = heeftBekeken
            };

            return gastgezinViewModel;
        }
    }
}
