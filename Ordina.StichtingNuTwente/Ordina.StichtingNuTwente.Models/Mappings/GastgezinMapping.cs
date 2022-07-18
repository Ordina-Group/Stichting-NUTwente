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
            DateTime? aanmeldDateTime= null;
            int intakeFormulierId = 0;
            DateTime? intakeDateTime = null;

            if (gastgezin.AanmeldFormulier != null)
            {
                aanmeldFormulierId = gastgezin.AanmeldFormulier.Id;
                aanmeldDateTime = gastgezin.AanmeldFormulier.DatumIngevuld;
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
            Comment? deletionComment = null;
            Comment? intakeCompletedComment = null;

            if (gastgezin.Comments != null && gastgezin.Comments.Count > 0)
            {
                deletionComment = gastgezin.Comments.LastOrDefault(g => g.CommentType == CommentType.DELETION);
                rejectionComment = gastgezin.Comments.LastOrDefault(g => g.CommentType == CommentType.BUDDY_REJECTION);
                intakeCompletedComment = gastgezin.Comments.LastOrDefault(g => g.CommentType == CommentType.INTAKE_COMPLETED);
            }


            var begeleiderId = 0;
            var begeleider = "";
            if (gastgezin.Begeleider != null)
            {
                begeleiderId = gastgezin.Begeleider.Id;
                begeleider = gastgezin.Begeleider.FirstName;
            }

            var buddyId = 0;
            var buddy = "";
            if (gastgezin.Buddy != null)
            {
                buddyId = gastgezin.Buddy.Id;
                buddy = gastgezin.Buddy.FirstName;
            }

            if (gastgezin.PlaatsingsInfo == null || !int.TryParse(gastgezin.PlaatsingsInfo.VolwassenenGrotereKinderen, out int maxOlderThanTwo))
            {
                maxOlderThanTwo = gastgezin.MaxOlderThanTwo.GetValueOrDefault();
            }

            if (gastgezin.PlaatsingsInfo == null || !int.TryParse(gastgezin.PlaatsingsInfo.KleineKinderen, out int maxYoungerThanThree))
            {
                maxYoungerThanThree = gastgezin.MaxYoungerThanThree.GetValueOrDefault();
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
                AanmeldDatum = aanmeldDateTime,
                IntakeFormulierId = intakeFormulierId,
                IntakeDatum = intakeDateTime,
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
                OnHold = gastgezin.OnHold,
                NoodOpvang = gastgezin.NoodOpvang,
                MaxOlderThanTwo = maxOlderThanTwo,
                MaxYoungerThanThree = maxYoungerThanThree,
                Note = gastgezin.Note,
                Deleted = gastgezin.Deleted,
                DeletionComment = deletionComment,
                VrijwilligerOpmerkingen = gastgezin.VrijwilligerOpmerkingen,
                CoordinatorOpmerkingen = gastgezin.CoordinatorOpmerkingen,
                ContactLogs = gastgezin.ContactLogs,
                IntakeCompletedComment = intakeCompletedComment
            };

            return gastgezinViewModel;
        }
    }
}
