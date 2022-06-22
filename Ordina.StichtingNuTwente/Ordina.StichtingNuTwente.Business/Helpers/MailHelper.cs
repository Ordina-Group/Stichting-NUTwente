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

        private Persoon getContactPersoon(Gastgezin gastgezin)
        {
            if(gastgezin == null)
            {
                return null;
            }

            Persoon contactPersoon = new Persoon();

            if (gastgezin.Buddy != null)
            {
                contactPersoon.Naam = gastgezin.Buddy.FirstName;
                contactPersoon.Achternaam = gastgezin.Buddy.LastName;
                contactPersoon.Mobiel = gastgezin.Buddy.PhoneNumber;
                contactPersoon.Email = gastgezin.Buddy.Email;
            }
            else if (gastgezin.Begeleider != null)
            {
                contactPersoon.Naam = gastgezin.Begeleider.FirstName;
                contactPersoon.Achternaam = gastgezin.Begeleider.LastName;
                contactPersoon.Mobiel = gastgezin.Begeleider.PhoneNumber;
                contactPersoon.Email = gastgezin.Begeleider.Email;
            }
            else
            {
                throw new Exception("Expexted buddy or begeleider not to be null"); 
            }

            return contactPersoon; 
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
            Persoon contactPersoon = getContactPersoon(gastgezin);
            string afsluitingBericht;

            var mail = new Mail()
            {
                MailToAdress = gastgezin.Contact.Email,
                MailToName = gastgezin.Contact.Naam + " " + gastgezin.Contact.Achternaam,
                Subject = "Bevestiging uitvoering intake",
                Message = $"Beste {gastgezin.Contact.Achternaam},\n\nOp {gastgezin.IntakeFormulier.DatumIngevuld.ToShortDateString()}heeft er bij u thuis een intakegesprek plaatsgevonden.Onderdeel van dit gesprek was het invullen van een vragenlijst: het intakeformulier. Dit formulier hebben wij in goede orde ontvangen. U bent vanaf heden inzetbaar als gastgezin voor de opvang van Oekraïense vluchtelingen via NuTwente. Heeft u vragen of opmerkingen of wilt u een wijziging doorgeven dan kunt u contact opnemen met uw buddy {gastgezin.Buddy.FirstName} {gastgezin.Buddy.LastName}, telefoonnummer: {gastgezin.Buddy.PhoneNumber}.\n\n\nVriendelijke groet,\n\nTeam Housing NuTwente\n\n\nDit is een automatisch gegenereerd bericht."
            };

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
                Message = $"Beste {persoon.Naam},\n\nBedankt dat u zich heeft aangemeld als vrijwilliger bij NuTwente. Wij trachten binnen twee weken contact met u op te nemen voor een telefonisch kennismakingsgesprek. In dit gesprek kunt u aangeven wat uw wensen en verwachtingen zijn en overleggen we over de mogelijkheden. Mocht u binnen twee weken geen nader bericht van ons hebben ontvangen dan verzoeken wij u een mail te sturen naar help@nutwente.nl t.a.v. R. Legtenberg.\n\nVriendelijke groet,\n\nTeam Vrijwilligers NuTwente\n\n\nDit is een automatisch gegenereerd bericht"
            };
            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }

        public async Task<bool> AanmeldingGastgezin(Persoon persoon)
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
                Message = $"Beste {persoon.Naam},\n\nOp {DateTime.Now.Date.ToShortDateString()} heeft u zich aangemeld als gastgezin voor het opvangen van Oekraïense vluchtelingen. Wij hebben uw aanmelding in goede orde ontvangen. Wij trachten binnen twee weken contact met u op te nemen voor het inplannen van een intakegesprek met één van onze intakers. Dit gesprek vindt bij u thuis plaats. Mocht u binnen twee weken geen nader bericht van ons hebben ontvangen dan verzoeken wij u een mail te sturen naar help@nutwente.nl, t.a.v. A. Esser.\n\nVriendelijke groet,\n\nTeam Housing NuTwente\n\n\nDit is een automatisch gegenereerd bericht"
            };
            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }


        //TODO: mail als alternatief bij missend telefoonnummer?
        public async Task<bool> PlaatsingVluchteling(Plaatsing plaatsing)
        {
            if(plaatsing == null)
            {
                return false;
            }

            var mail = new Mail()
            {
                MailToAdress = plaatsing.Gastgezin.Buddy.Email,
                MailToName = plaatsing.Gastgezin.Buddy.FirstName,
                Subject = "Plaatsing vluchteling",
                Message = $"Beste buddy,\n\nBij jouw gastgezin {plaatsing.Gastgezin.Contact.Achternaam}, {plaatsing.Gastgezin.PlaatsingsInfo.AdresVanLocatie} in {plaatsing.Gastgezin.PlaatsingsInfo.PlaatsnaamVanLocatie} is/zijn op {plaatsing.DateTime.Date.ToShortDateString()} {plaatsing.Amount} vluchteling(en) geplaatst. Je kunt deze plaatsing terugvinden in je overzicht ‘mijn gastgezinnen’ in de database. Wil je op korte termijn, als dit nog niet is gebeurd, contact opnemen met het gastgezin? Het telefoonnummer is {plaatsing.Gastgezin.PlaatsingsInfo.TelefoonnummerVanLocatie}.\n\n\nDit is een automatisch gegenereerd bericht."
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
