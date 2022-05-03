using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

public class MailService : IMailService
{
    private string MailAdressFrom { get; set; }
    private string ApiKey { get; set; }

    public async Task<bool> sendMail(Mail mail)
    {
        //TODO: checks invoeren om te kijken of het correcte mailadressen zijn.
        if (mail.MailFromName == null || mail.MailFromName == "")
        {
            mail.MailFromName = "Secretariaat NUTwente";
        }

        var client = new SendGridClient(ApiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(MailAdressFrom, mail.MailFromName),
            Subject = mail.Subject,
            PlainTextContent = mail.Message
        };
        msg.AddTo(new EmailAddress(mail.MailToAdress, mail.MailToName));
        var response = await client.SendEmailAsync(msg);

        Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");

        return response.IsSuccessStatusCode;

        //Console.WriteLine("Mail sent to: " + mail.MailToAdress + ", with message: " + msg);

        //return true;
    }

    public async Task<bool> sendGroupMail(string subject, string message, List<string> mailAdresses)
    {
        List<EmailAddress> emailAddresses = new List<EmailAddress>();
        foreach(var mail in mailAdresses)
        {
            emailAddresses.Add(new EmailAddress(mail));
        }

        var client = new SendGridClient(ApiKey);
        /*var msg = new SendGridMessage()
        {
            From = new EmailAddress(MailAdressFrom, mail.MailFromName),
            Subject = subject,
            PlainTextContent = message,
            Personalizations = new List<Personalization>
            {
                new Personalization
                {
               
                }
            }
        }*/

        EmailAddress fromMail = new EmailAddress(this.MailAdressFrom);

        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(fromMail, emailAddresses, subject, message, null) ;

        var response = await client.SendEmailAsync(msg);
        /*foreach(var mail in mailAdresses)
        {
            Console.WriteLine("Mail sent to: " + mail + ", with message: " + msg);

        }*/

        Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");
        return response.IsSuccessStatusCode;
    }

    public void setFromMail(string mailAdress)
    {
        MailAdressFrom = mailAdress;
    }

    public void setApiKey(string apiKey)
    {
        ApiKey = apiKey;
    }
}
