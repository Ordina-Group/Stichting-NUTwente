using Ordina.StichtingNuTwente.Business.Interfaces;
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
            Persoon contactPersoon = new Persoon();
            string afsluitingBericht;

            if(gastgezin.Buddy != null)
            {
                contactPersoon.Naam = gastgezin.Buddy.FirstName;
                contactPersoon.Achternaam = gastgezin.Buddy.LastName;
                contactPersoon.Mobiel = gastgezin.Buddy.PhoneNumber;
                contactPersoon.Email = gastgezin.Buddy.Email;
                afsluitingBericht = "met uw buddy: ";
            }
            else if(gastgezin.Begeleider != null)
            {
                contactPersoon.Naam = gastgezin.Begeleider.FirstName;
                contactPersoon.Achternaam = gastgezin.Begeleider.LastName;
                contactPersoon.Mobiel = gastgezin.Begeleider.PhoneNumber;
                contactPersoon.Email = gastgezin.Begeleider.Email;
                afsluitingBericht = "met uw begeleider: ";
            }
            else
            {
                contactPersoon.Naam = "Stichting NuTwente";
                contactPersoon.Email = "help@nutwente.nl";
                afsluitingBericht = "met: ";
            }

            mail.MailToName = gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam;
            mail.MailToAdress = gastgezin.Contact.Email;
            mail.Subject = "Bevestiging uitvoering intake";
            mail.Message = "Beste " + gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam + ", \n" + "Op " + gastgezin.IntakeFormulier.DatumIngevuld.ToShortDateString() + " heeft " + contactPersoon.Naam + " bij u een intakegesprek uitgevoerd, als voorbereiding op een mogelijke plaatsing van Oekraïense vluchtelingen. De gegevens van deze intake zijn geregistreerd bij Nutwente. Indien u daarin wijzigingen wilt aanbrengen of andere vragen heeft kunt u daarover altijd contact opnemen " + afsluitingBericht + contactPersoon.Naam + (string.IsNullOrEmpty(contactPersoon.Achternaam) ? "" : " " + contactPersoon.Achternaam) + (string.IsNullOrEmpty(contactPersoon.Telefoonnummer) ? "" : (", tel:" + contactPersoon.Telefoonnummer)) + (string.IsNullOrEmpty(contactPersoon.Email)  ? "" : (", e-mail: " + contactPersoon.Email)) + ". \n\nMet vriendelijke groet,\nNuTwente \n(deze e-mail is door een computer geschreven en verstuurd. U hoeft niet te reageren. Mocht u wel antwoorden(reply), dan komt uw bericht aan bij ons secretariaat)";

            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }

       /* public async Task<bool> BevestigPlaatsing(Gastgezin gastgezin, string afspraken)
        {
            Mail mail = new Mail();

            mail.MailToName = gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam;
            mail.MailToAdress = gastgezin.Contact.Email;
            mail.Subject = "Plaatsing bij uw gastgezin";
            mail.Message = "Beste " + gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam + ", \n" + "Zojuist is er in overleg met u een plaatsing gedaan bij uw gastgezin. Hierbij zijn de volgende afspraken gemaakt:\n" + afspraken + "\n"
        }*/

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

        public async Task<bool> AanmeldingVrijwilliger(Persoon persoon)
        {
            if (persoon == null)
            {
                return false; 
            }

            var mail = new Mail()
            {
                MailToAdress = persoon.Email,
                MailToName = persoon.Naam,
                Subject = "Bevestiging van aanmelding",
                Message = $"Beste {persoon.Naam},\n\nBedankt dat u zich heeft aangemeld als vrijwilliger bij NuTwente. Wij trachten binnen twee weken contact met u op te nemen voor een telefonisch kennismakingsgesprek. In dit gesprek kunt u aangeven wat uw wensen en verwachtingen zijn en overleggen we over de mogelijkheden. Mocht u binnen twee weken geen nader bericht van ons hebben ontvangen dan verzoeken wij u een mail te sturen naar help@nutwente.nl t.a.v. R. Legtenberg."
            };
            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }

        public async Task<bool> AanmeldingGastgezin(Gastgezin gastgezin)
        {
            if (gastgezin == null)
            {
                return false;
            }

            var mail = new Mail()
            {
                MailToAdress = gastgezin.Contact.Email,
                MailToName = gastgezin.Contact.Naam,
                Subject = "Bevestiging van aanmelding",
                Message = $"Beste {gastgezin.Contact.Naam},\n\nOp {DateTime.Now.Date.ToString()} heeft u zich aangemeld als gastgezin voor het opvangen van Oekraïense vluchtelingen. Wij hebben uw aanmelding in goede orde ontvangen. Wij trachten binnen twee weken contact met u op te nemen voor het inplannen van een intakegesprek met één van onze intakers. Dit gesprek vindt bij u thuis plaats. Mocht u binnen twee weken geen nader bericht van ons hebben ontvangen dan verzoeken wij u een mail te sturen naar help@nutwente.nl, t.a.v. A. Esser.\n\nVriendelijke groet,\n\nTeam Housing NuTwente\n\n\nDit is een automatisch gegenereerd bericht"
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
