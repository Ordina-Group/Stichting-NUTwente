﻿using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Helpers
{
    public class MailHelper
    {
        private IMailService _mailService { get; set; }

        public MailHelper(IMailService mailService)
        {
            _mailService = mailService;
        }



        public async Task<bool> MaakIntakeMatch(Gastgezin gastgezin, Persoon persoon)
        {
            Mail mail = new Mail();

            //persoon.Gastgezin = gastgezin;

            mail.MailToName = persoon.Naam;
            mail.MailToAdress = ""; //voor nu even hardcoded om spam en ongelukken bij het testen te voorkomen.
            mail.Subject = "Uw nieuwe intake match met " + gastgezin.Contact.Naam;
            mail.Message = "Beste " + persoon.Naam + ", \n" + "Zojuist bent u toegewezen aan het gastgezin van: " + gastgezin.Contact.Naam + ". U kunt hem/haar bereiken via mail: " + gastgezin.Contact.Email + ", of via telefoon: " + gastgezin.Contact.Mobiel + ", om een afspraak te maken.";

            bool succes = await(_mailService.SendMail(mail));

            return succes;
        }

        public async Task<bool> IntakeUitgevoerd(Gastgezin gastgezin)
        {
            Mail mail = new Mail();

            //TODO gastgezin.Begeleider naar een nieuw object hiero. Zodat als begeleider Null is, dan komt er algemene contact info van NuTwente

            mail.MailToName = gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam;
            mail.MailToAdress = gastgezin.Contact.Email;
            mail.Subject = "Bevestiging uitvoering intake";
            mail.Message = "Beste " + gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam + ", \n" + "Op " + gastgezin.IntakeFormulier.DatumIngevuld.ToString() + " heeft " + gastgezin.Begeleider.FirstName + " bij u een intakegesprek uitgevoerd, als voorbereiding op een mogelijke plaatsing van Oekraïense vluchtelingen. De gegevens van deze intake zijn geregistreerd bij Nutwente. Indien u daarin wijzigingen wilt aanbrengen of andere vragen heeft kunt u daarover altijd contact opnemen met uw begeleider" + (gastgezin.Begeleider != null ? ": " + gastgezin.Begeleider.FirstName + " " + gastgezin.Begeleider.LastName + (gastgezin.Begeleider.PhoneNumber != null ? ", tel:" + gastgezin.Begeleider.PhoneNumber : "") + (gastgezin.Begeleider.Email != null ? ", e-mail:" + gastgezin.Begeleider.Email : "") + ". \n\nMet vriendelijke groet,\nNutwente \n(deze e-mail is door een computer geschreven en verstuurd. U hoeft niet te reageren. Mocht u wel antwoorden(reply), dan komt uw bericht aan bij ons secretariaat)" : ".");

            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }

        public async Task<bool> Bevestiging(Persoon persoon)
        {
            var mail = new Mail()
            {
                MailToAdress = persoon.Email,
                MailToName = persoon.Naam,
                Subject = "Bevestiging van aanmelding",
                Message = $"Beste {persoon.Naam},\n\nBedankt voor uw aanmelding.\nVoor meer informatie kunt u terecht op onze website: www.nutwente.nl\n\nMet vriendelijke groet,\nStichting NuTwente"
            };
            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }

        public async Task<List<Persoon>> SendMailToGroup(List<Persoon> personen, string onderwerp, string bericht)
        {
            Mail mail = new Mail();
            List<Persoon> unsendList = new List<Persoon>();
            mail.Subject = onderwerp;
            mail.Message = bericht;

            foreach (Persoon p in personen)
            {
                mail.MailToName = p.Naam;
                //mail.MailToAdress = p.Email;
                mail.MailToAdress = ""; //voor nu even hardcoded om spam en ongelukken bij het testen te voorkomen.

                if(! await (_mailService.SendMail(mail)))
                {
                    unsendList.Add(p);
                }
            }

            return unsendList;
        }

        public async Task<List<Persoon>> SendMailToGroup(List<Persoon> personen, string onderwerp, string bericht, UserDetails userDetails)
        {
            Mail mail = new Mail();
            List<Persoon> unsendList = new List<Persoon>();

            mail.MailFromName = userDetails.FirstName;
            _mailService.SetFromMail(userDetails.Email);
            mail.Subject = onderwerp;
            mail.Message = bericht;

            foreach (Persoon p in personen)
            {
                mail.MailToName = p.Naam;
                //mail.MailToAdress = p.Email;
                mail.MailToAdress = ""; //voor nu even hardcoded om spam en ongelukken bij het testen te voorkomen.

                if (!await (_mailService.SendMail(mail)))
                {
                    unsendList.Add(p);
                }
            }


            return unsendList;
        }

    }
}
