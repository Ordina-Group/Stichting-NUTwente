using Microsoft.Extensions.Configuration;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Helpers
{
    class EmailContact
    {
        public string? EmailAdress { get; set; }
        public string? ContactPerson { get; set; }
    }

    public class MailHelper
    {
        private readonly IConfiguration _configuration;
        private IMailService _mailService { get; set; }

        public MailHelper(IMailService mailService, IConfiguration configuration)
        {
            _configuration = configuration;
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
                throw new Exception("Expected buddy or begeleider not to be null"); 
            }

            return contactPersoon; 
        }

        public async Task<bool> ToekennenIntaker(Gastgezin gastgezin, Persoon persoon) // checked
        {
            Mail mail = new Mail();

            //persoon.Gastgezin = gastgezin;

            mail.MailToName = persoon.Naam;
            mail.MailToAdress = persoon.Email;
            mail.Subject = "Uw nieuwe intake match met " + gastgezin.Contact.Naam;
            mail.Message = $"Beste {persoon.Naam},\n\nOp {DateTime.Now.ToString("yyyy-MM-dd")} heeft {gastgezin.Contact.Naam}, {gastgezin.Contact.Adres.Straat} in {gastgezin.Contact.Adres.Woonplaats} zich aangemeld als potentieel gastgezin bij NuTwente. Jij bent als intaker aan dit gezin gekoppeld om het intakegesprek af te nemen. Lukt het niet om binnen een week contact op te nemen met dit gezin of kun je om andere redenen dit intakegesprek niet afnemen dan verzoeken we je dit per omgaande door te geven aan Audrey Esser (audrey@essercommunications.nl). Er wordt dan een andere intaker gekoppeld aan dit gezin. Je kunt de gegevens van het gastgezin terugvinden in je overzicht ‘mijn gastgezinnen’ in de database.\n\n\nDit is een automatisch gegenereerd bericht.";

            bool succes = await(_mailService.SendMail(mail));

            return succes;
        }

        public async Task<bool> ToekennenBuddy(Gastgezin gastgezin, Persoon persoon) 
        {
            Mail mail = new Mail();

            //persoon.Gastgezin = gastgezin;

            mail.MailToName = persoon.Naam;
            mail.MailToAdress = persoon.Email;
            mail.Subject = "Uw nieuwe intake match met " + gastgezin.Contact.Naam;
            mail.Message = $"Beste {persoon.Naam},\n\nOp {DateTime.Now.ToString("yyyy-MM-dd")} heeft er een intakegesprek plaatsgevonden bij {gastgezin.Contact.Naam}, {gastgezin.Contact.Adres.Straat} in {gastgezin.Contact.Adres.Woonplaats}. Jij bent als buddy gekoppeld aan dit gastgezin. Wil of kun je geen buddy zijn van dit gezin dan verzoeken we je dit door te geven aan Audrey Esser (audrey@essercommunications.nl). Er wordt dan een andere buddy gekoppeld aan dit gezin. Je kunt de gegevens van het gastgezin terugvinden in je overzicht ‘mijn gastgezinnen’ in de database.\n\n\nDit is een automatisch gegenereerd bericht.";

            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }

        public async Task<bool> IntakeUitgevoerd(Gastgezin gastgezin) // checked
        {
            if (gastgezin == null)
            {
                return false;
            }
            List<string> emailAdresses = new() {
                    gastgezin.Contact.Email,
                    gastgezin.Begeleider.Email,
                    _configuration.GetSection("EmailAdressO)").Value,
                    _configuration.GetSection("EmailAdressB").Value};

            string recipient = gastgezin.Contact.Naam;
            return await _mailService.SendGroupMail("Plaatsing vluchteling", $"Beste {recipient},\n\n Op {DateTime.Now.ToString("dd-MM-yyyy")} heeft er bij u thuis een intakegesprek plaatsgevonden. Onderdeel van dit gesprek was het invullen van een vragenlijst: het intakeformulier. Dit formulier hebben wij in goede orde ontvangen. U bent vanaf heden inzetbaar als gastgezin voor de opvang van Oekraïense vluchtelingen via NuTwente. Heeft u vragen of opmerkingen of wilt u een wijziging doorgeven dan kunt u contact opnemen met uw buddy {gastgezin.Buddy?.FirstName} {gastgezin.Buddy?.LastName}, telefoonnummer {gastgezin.Buddy?.PhoneNumber}\n\nVriendelijke groet,\n\nTeam Housing, NuTwente\n\nDit is een automatisch gegenereerd bericht.", emailAdresses);

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

        public async Task<bool> AanmeldingGastgezin(Persoon persoon) // Checked
        {
            if (persoon == null)
            {
                return false;
            }
            List<string> recipients = new List<string>()
            {
                persoon.Email,
                _configuration.GetSection("EmailAdressB)").Value,
            };

            bool succes = await (_mailService.SendGroupMail("Bevestiging van aanmelding", $"Beste {persoon.Naam},\n\nOp {DateTime.Now.ToString("dd-MM-yyyy")} heeft u zich aangemeld als gastgezin voor het opvangen van Oekraïense vluchtelingen. Wij hebben uw aanmelding in goede orde ontvangen. Wij trachten binnen twee weken contact met u op te nemen voor het inplannen van een intakegesprek met één van onze intakers. Dit gesprek vindt bij u thuis plaats. Mocht u binnen twee weken geen nader bericht van ons hebben ontvangen dan verzoeken wij u een mail te sturen naar help@nutwente.nl, t.a.v. A. Esser.\n\nVriendelijke groet,\n\nTeam Housing NuTwente\n\nVriendelijke groet,\n\nTeam Housing, NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients));

            return succes;
        }


        public async Task<bool> PlaatsingVluchteling(Gastgezin gastgezin)
        {
            string recipient = "";
            if(gastgezin == null)
            {
                return false;
            }

            EmailContact emailContact = new EmailContact();
            List<string> emailAdresses = new() {
                    _configuration.GetSection("EmailAdressO").Value,
                    _configuration.GetSection("EmailAdressT").Value
                };

            if (gastgezin.Buddy != null && gastgezin.Buddy.Deleted == false)
            {
                emailAdresses.Add(gastgezin.Buddy.Email);
                recipient = "buddy";
            }
            else
            {
                emailAdresses.Add(_configuration.GetSection("EmailAdressB").Value);
                recipient = "'bij ontbreeking van buddy & intaker'";

            }
            return await _mailService.SendGroupMail("Plaatsing vluchteling", $"Beste {recipient},\n\nBij jouw gastgezin {gastgezin.Contact.Naam}, {gastgezin.PlaatsingsInfo?.AdresVanLocatie} in {gastgezin.PlaatsingsInfo?.PlaatsnaamVanLocatie} is/zijn nieuwe vluchteling(en) geplaatst. Je kunt deze plaatsing terugvinden in je overzicht ‘mijn gastgezinnen’ in de database. Wil je op korte termijn, als dit nog niet is gebeurd, contact opnemen met het gastgezin? Het telefoonnummer is {gastgezin.Contact.Telefoonnummer}.\n\n\nDit is een automatisch gegenereerd bericht.", emailAdresses);
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
