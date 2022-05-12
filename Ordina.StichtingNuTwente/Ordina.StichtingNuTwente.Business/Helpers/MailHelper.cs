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

        private string DefaultSendAdress { get; set; }
        private string SendName { get; set; }

        public MailHelper(IMailService mailService)
        {
            _mailService = mailService;
            DefaultSendAdress = "secretariaat@NUTwente.nl";
            SendName = "Secretariaat Stichting NUTwente";
            _mailService.SetApiKey("***REMOVED***");
        }



        public async Task<bool> MaakIntakeMatch(Gastgezin gastgezin, Persoon persoon)
        {
            Mail mail = new Mail();
            _mailService.SetFromMail(DefaultSendAdress);

            //persoon.Gastgezin = gastgezin;

            mail.MailToName = persoon.Naam;
            mail.MailToAdress = ""; //voor nu even hardcoded om spam en ongelukken bij het testen te voorkomen.
            mail.MailFromName = SendName;
            mail.Subject = "Uw nieuwe intake match met " + gastgezin.Contact.Naam;
            mail.Message = "Beste " + persoon.Naam + ", \n" + "Zojuist bent u toegewezen aan het gastgezin van: " + gastgezin.Contact.Naam + ". U kunt hem/haar bereiken via mail: " + gastgezin.Contact.Email + ", of via telefoon: " + gastgezin.Contact.Mobiel + ", om een afspraak te maken.";

            bool succes = await(_mailService.SendMail(mail));

            return succes;
        }

        public async Task<bool> Bevestiging(Persoon persoon)
        {
            _mailService.SetFromMail("niek.nieuwenhuisen@ordina.nl");
            //_mailService.setFromMail(DefaultSendAdress);
            var mail = new Mail()
            {
                MailToAdress = persoon.Email,
                MailToName = persoon.Naam,
                Subject = "Bevestiging van aanmelding",
                MailFromName = "Stichting NUTwente",
                Message = "Beste " + persoon.Naam + ", \n" + "Bedankt voor uw inschrijving. Voor meer informatie kunt u terecht op onze website: https://www.nutwente.nl/"
            };
            bool succes = await (_mailService.SendMail(mail));

            return succes;
        }

        public async Task<List<Persoon>> SendMailToGroup(List<Persoon> personen, string onderwerp, string bericht)
        {
            Mail mail = new Mail();
            List<Persoon> unsendList = new List<Persoon>();
            _mailService.SetFromMail(DefaultSendAdress);

            mail.MailFromName = SendName;
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
