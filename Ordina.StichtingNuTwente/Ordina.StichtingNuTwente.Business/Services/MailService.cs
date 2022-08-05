using Microsoft.Extensions.Configuration;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly MailHelper _mailHelper;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _mailHelper = new MailHelper(_configuration.GetSection("SENDGRID_API_KEY").Value, _configuration.GetSection("SENDGRID_MAILFROM_ADDRESS").Value, _configuration.GetSection("SENDGRID_MAILFROM_NAME").Value);
        }

        public async Task<bool> ToekennenIntaker(Gastgezin gastgezin, Persoon persoon)
        {
            if (persoon == null || gastgezin == null)
            {
                return false;
            }
            usedLocation location = UsedLocation(gastgezin);
            List<string> recipients = new List<string>()
            {
                persoon.Email,
                _configuration.GetSection("SENDGRID_MAILTO_B").Value,
            };

            return await _mailHelper.SendGroupMail("Intaker koppeling aan " + gastgezin.Contact.Naam, $"Beste {persoon.Naam},\n\nOp {DateTime.Now.ToString("dd-MM-yyyy")} heeft {gastgezin.Contact.Naam}, {location.adress} in {location.plaatsnaam} zich aangemeld als potentieel gastgezin bij NuTwente. Jij bent als intaker aan dit gezin gekoppeld om het intakegesprek af te nemen. Lukt het niet om binnen een week contact op te nemen met dit gezin of kun je om andere redenen dit intakegesprek niet afnemen dan verzoeken we je dit per omgaande door te geven aan Audrey Esser (audrey@essercommunications.nl). Er wordt dan een andere intaker gekoppeld aan dit gezin. Je kunt de gegevens van het gastgezin terugvinden in je overzicht ‘mijn gastgezinnen’ in de database.\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients);
        }

        public async Task<bool> ToekennenBuddy(Gastgezin gastgezin, Persoon persoon)
        {
            if (persoon == null || gastgezin == null)
            {
                return false;
            }
            usedLocation location = UsedLocation(gastgezin);
            List<string> recipients = new List<string>()
            {
                persoon.Email,
                _configuration.GetSection("SENDGRID_MAILTO_B").Value,
            };

            return await _mailHelper.SendGroupMail("Buddy koppeling aan " + gastgezin.Contact.Naam, $"Beste {persoon.Naam},\n\nOp {DateTime.Now.ToString("yyyy-MM-dd")} heeft er een intakegesprek plaatsgevonden bij {gastgezin.Contact.Naam}, {location.adress} in {location.plaatsnaam}. Jij bent als buddy gekoppeld aan dit gastgezin. Wil of kun je geen buddy zijn van dit gezin dan verzoeken we je dit door te geven aan Audrey Esser (audrey@essercommunications.nl). Er wordt dan een andere buddy gekoppeld aan dit gezin. Je kunt de gegevens van het gastgezin terugvinden in je overzicht ‘mijn gastgezinnen’ in de database.\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients);
        }

        public async Task<bool> VertrekVluchteling(Gastgezin gastgezin, Persoon persoon)
        {
            if (persoon == null || gastgezin == null)
            {
                return false;
            }
            usedLocation location = UsedLocation(gastgezin);
            List<string> recipients = new List<string>()
            {
                persoon.Email,
                _configuration.GetSection("SENDGRID_MAILTO_T").Value,
                _configuration.GetSection("SENDGRID_MAILTO_O").Value,
            };

            return await _mailHelper.SendGroupMail("Vertrek vluchtelingen", $"Beste buddy,\n\nBij jouw gastgezin {gastgezin.Contact.Naam}, {location.adress} in {location.plaatsnaam} heeft er een intakegesprek plaatsgevonden bij {gastgezin.Contact.Naam},  {location.adress} in {location.plaatsnaam} is / zijn op {DateTime.Now.ToString("yyyy-MM-dd")} vluchtelingen vertrokken. Je kunt deze mutatie terugvinden in je overzicht 'mijn gastgezinnen' in de database\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients);
        }

        public async Task<bool> PlaatsingsReservering(Gastgezin gastgezin, Persoon persoon)
        {
            if (persoon == null || gastgezin == null)
            {
                return false;
            }
            usedLocation location = UsedLocation(gastgezin);
            List<string> recipients = new List<string>()
            {
                persoon.Email,
                _configuration.GetSection("SENDGRID_MAILTO_B").Value,
            };

            return await _mailHelper.SendGroupMail("Reservering bij " + gastgezin.Contact.Naam, $"Beste buddy,\n\nBij jouw gastgezin {gastgezin.Contact.Naam}, {location.adress} in {location.plaatsnaam} geld een reservering voor de plaatsing van vluchtelingen. Je kunt deze reserveringen terugvinden in je overzicht 'mijn gastgezinnen' in de database.\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients);
        }

        public async Task<bool> VerwijderenGastgezin(Gastgezin gastgezin, Persoon persoon)
        {
            if (persoon == null || gastgezin == null)
            {
                return false;
            }
            usedLocation location = UsedLocation(gastgezin);
            List<string> recipients = new List<string>()
            {
                persoon.Email,
                _configuration.GetSection("SENDGRID_MAILTO_B").Value,
            };

            return await _mailHelper.SendGroupMail("Verwijdering gastgezin van " + gastgezin.Contact.Naam, $"Beste {persoon.Naam},\n\nOp {DateTime.Now.ToString("dd-MM-yyyy")} is {gastgezin.Contact.Naam}, {location.adress} in {location.plaatsnaam} verwijderd als gastgezin uit de database van NuTwente. Dit gastgezin staat vanaf heden niet meer in jouw overzicht met gastgezinnen. Mocht je meer willen weten over bijvoorbeeld de reden van verwijdering, neem dan  contact op met de coördinator Housing.\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients);
        }

        public async Task<bool> IntakeUitgevoerd(Gastgezin gastgezin)
        {
            if (gastgezin == null)
            {
                return false;
            }
            List<string> emailAdresses = new() {
                    gastgezin.Contact.Email,
                    gastgezin.Intaker.Email,
                    _configuration.GetSection("SENDGRID_MAILTO_O").Value,
                    _configuration.GetSection("SENDGRID_MAILTO_B").Value};

            string recipient = gastgezin.Contact.Naam;
            return await _mailHelper.SendGroupMail("Intake Gastgezin", $"Beste {recipient},\n\n Op {DateTime.Now.ToString("dd-MM-yyyy")} heeft er bij u thuis een intakegesprek plaatsgevonden. Onderdeel van dit gesprek was het invullen van een vragenlijst: het intakeformulier. Dit formulier hebben wij in goede orde ontvangen. U bent vanaf heden inzetbaar als gastgezin voor de opvang van Oekraïense vluchtelingen via NuTwente. Heeft u vragen of opmerkingen of wilt u een wijziging doorgeven dan kunt u contact opnemen met uw buddy {gastgezin.Buddy?.FirstName} {gastgezin.Buddy?.LastName}, telefoonnummer {gastgezin.Buddy?.PhoneNumber}\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", emailAdresses);

        }

        public async Task<bool> AanmeldingVrijwilliger(Persoon vrijwilliger)
        {
            if (vrijwilliger == null)
            {
                return false;
            }
            List<string> recipients = new List<string>()
            {
                vrijwilliger.Email,
                _configuration.GetSection("SENDGRID_MAILTO_R").Value,
            };

            return await _mailHelper.SendGroupMail("Aanmelding als vrijwilliger bij NuTwente", $"Beste {vrijwilliger.Naam},\n\nBedankt dat u zich heeft aangemeld als vrijwilliger bij NuTwente. Wij trachten binnen twee weken contact met u op te nemen voor een telefonisch kennismakingsgesprek. In dit gesprek kunt u aangeven wat uw wensen en verwachtingen zijn en overleggen we over de mogelijkheden. Mocht u binnen twee weken geen nader bericht van ons hebben ontvangen dan verzoeken wij u een mail te sturen naar help@nutwente.nl t.a.v. R. Legtenberg.\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients);
        }

        public async Task<bool> AanmeldingGastgezin(Persoon contactpersoon)
        {
            if (contactpersoon == null)
            {
                return false;
            }
            List<string> recipients = new List<string>()
            {
                contactpersoon.Email,
                _configuration.GetSection("SENDGRID_MAILTO_B").Value,
            };

            return await _mailHelper.SendGroupMail("Bevestiging van aanmelding", $"Beste {contactpersoon.Naam},\n\nOp {DateTime.Now.ToString("dd-MM-yyyy")} heeft u zich aangemeld als gastgezin voor het opvangen van Oekraïense vluchtelingen. Wij hebben uw aanmelding in goede orde ontvangen. Wij trachten binnen twee weken contact met u op te nemen voor het inplannen van een intakegesprek met één van onze intakers. Dit gesprek vindt bij u thuis plaats. Mocht u binnen twee weken geen nader bericht van ons hebben ontvangen dan verzoeken wij u een mail te sturen naar help@nutwente.nl, t.a.v. A. Esser.\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht.", recipients);
        }

        public async Task<bool> PlaatsingVluchteling(Gastgezin gastgezin)
        {
            string recipient = "";
            if (gastgezin == null)
            {
                return false;
            }
            usedLocation location = UsedLocation(gastgezin);
            List<string> emailAdresses = new() {
                    _configuration.GetSection("SENDGRID_MAILTO_O").Value,
                    _configuration.GetSection("SENDGRID_MAILTO_T").Value
                };

            if (gastgezin.Buddy != null && gastgezin.Buddy.Deleted == false)
            {
                emailAdresses.Add(gastgezin.Buddy.Email);
                recipient = "buddy";
            }
            else
            {
                emailAdresses.Add(_configuration.GetSection("SENDGRID_MAILTO_B").Value);
                recipient = "'bij ontbreeking van buddy & intaker'";

            }
            return await _mailHelper.SendGroupMail("Plaatsing vluchteling", $"Beste {recipient},\n\nBij jouw gastgezin {gastgezin.Contact.Naam}, {location.adress} in {location.plaatsnaam} is/zijn nieuwe vluchteling(en) geplaatst. Je kunt deze plaatsing terugvinden in je overzicht ‘mijn gastgezinnen’ in de database. Wil je op korte termijn, als dit nog niet is gebeurd, contact opnemen met het gastgezin? Het telefoonnummer is {gastgezin.Contact.Telefoonnummer}.\n\nVriendelijke groet,\n\nTeam NuTwente\n\nDit is een automatisch gegenereerd bericht", emailAdresses);
        }

        private usedLocation UsedLocation(Gastgezin gastgezin) // temp fix voor ontbreken van data
        {
            usedLocation location = new usedLocation();
            if (gastgezin.PlaatsingsInfo?.PlaatsnaamVanLocatie != null)
            {
                location.plaatsnaam = gastgezin.PlaatsingsInfo?.PlaatsnaamVanLocatie;
            }
            else
            {
                location.plaatsnaam = gastgezin.Contact.Adres?.Woonplaats;
            }

            if (gastgezin.PlaatsingsInfo?.AdresVanLocatie != null)
            {
                location.plaatsnaam = gastgezin.PlaatsingsInfo?.AdresVanLocatie;
            }
            else
            {
                location.plaatsnaam = gastgezin.Contact.Adres?.Straat;
            }
            return location;
        }
    }

    public class usedLocation
    {
        public string? adress { get; set; }
        public string? plaatsnaam { get; set; }
    }
}
