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

            if (gastgezin.AanmeldFormulier != null)
            {
                aanmeldFormulierId = gastgezin.AanmeldFormulier.Id;
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
                HeeftBekeken = heeftBekeken,
                RejectionComment = rejectionComment,
                Buddy = buddy,
                BuddyId = buddyId,
                Begeleider = begeleider,
                BegeleiderId = begeleiderId,
                Status = gastgezin.Status,
                MaxAdults = gastgezin.MaxAdults,
                MaxChildren = gastgezin.MaxChildren,
                Note = gastgezin.Note,
            };

            return gastgezinViewModel;
        }
    }
}
