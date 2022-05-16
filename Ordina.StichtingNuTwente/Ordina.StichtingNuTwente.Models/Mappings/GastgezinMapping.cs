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
            var postcodeText = "";

            if (adres != null)
            {
                adresText = adres.Straat;
                woonplaatsText = adres.Woonplaats;
                postcodeText = adres.Postcode;
            }

            int aanmeldFormulierId = 0;
            int intakeFormulierId = 0;
            DateTime? intakeDateTime = null;

            if (gastgezin.AanmeldFormulier != null)
            {
                aanmeldFormulierId = gastgezin.AanmeldFormulier.Id;
            }

            if (gastgezin.IntakeFormulier != null)
            {
                intakeFormulierId = gastgezin.IntakeFormulier.Id;
                intakeDateTime = gastgezin.IntakeFormulier.DatumIngevuld;
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

            Comment? rejectionComment = null;

            if (gastgezin.Comments != null && gastgezin.Comments.Count > 0)
            {
                rejectionComment = gastgezin.Comments.LastOrDefault(g => g.CommentType == CommentType.BUDDY_REJECTION);
            }

            var begeleiderId = 0;
            var begeleider = "";
            if (gastgezin.Begeleider != null)
            {
                begeleiderId = gastgezin.Begeleider.Id;
                begeleider = $"{gastgezin.Begeleider.FirstName} {gastgezin.Begeleider.LastName} ({gastgezin.Begeleider.Email})";
            }

            var buddyId = 0;
            var buddy = "";
            if (gastgezin.Buddy != null)
            {
                buddyId = gastgezin.Buddy.Id;
                buddy = $"{gastgezin.Buddy.FirstName} {gastgezin.Buddy.LastName} ({gastgezin.Buddy.Email})";
            }

            int? maxAdults = 0;
            int? maxChildren = 0;
            if (gastgezin.MaxAdults != null)
            {
                maxChildren = gastgezin.MaxAdults;
            }
            if (gastgezin.MaxChildren != null)
            {
                maxChildren = gastgezin.MaxChildren;
            }

            var gastgezinViewModel = new GastgezinViewModel
            {
                Id = gastgezin.Id,
                Adres = adresText,
                Email = contact.Email,
                Naam = contact.Naam + " " + contact.Achternaam,
                Telefoonnummer = contact.Telefoonnummer,
                Woonplaats = woonplaatsText,
                Postcode = postcodeText,
                AanmeldFormulierId = aanmeldFormulierId,
                IntakeFormulierId = intakeFormulierId,
                Intake = intakeDateTime,
                PlaatsingTag = plaatsingTag,
                ReserveTag = ReserveTag,
                PlaatsingsInfo = gastgezin.PlaatsingsInfo,
                HasVOG = gastgezin.HasVOG,
                HeeftBekeken = heeftBekeken,
                RejectionComment = rejectionComment,
                Buddy = buddy,
                BuddyId = buddyId,
                Begeleider = begeleider,
                BegeleiderId = begeleiderId,
                Status = gastgezin.Status,
                MaxAdults = maxAdults,
                MaxChildren = maxChildren,
                Note = gastgezin.Note,
            };

            return gastgezinViewModel;
        }
    }
}
