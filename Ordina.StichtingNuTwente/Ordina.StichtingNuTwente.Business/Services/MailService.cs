using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

public class MailService : IMailService
{
    private string MailFrom { get; set; }
    private string ApiKey { get; set; }

    public async Task<bool> sendMail(Mail mail)
    {
        //TODO: checks invoeren om te kijken of het correcte mailadressen zijn.

        var client = new SendGridClient(ApiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(MailFrom, mail.MailFromName),
            Subject = mail.Subject,
            PlainTextContent = mail.Message
        };
        msg.AddTo(new EmailAddress(mail.MailToAdress, mail.MailToName));
        var response = await client.SendEmailAsync(msg);

        //Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");

        return response.IsSuccessStatusCode;

        //Console.WriteLine("Mail sent: " + msg);

        //return true;
    }

    public void setFromMail(string mailAdress)
    {
        MailFrom = mailAdress;
    }

    public void setApiKey(string apiKey)
    {
        ApiKey = apiKey;
    }
}
