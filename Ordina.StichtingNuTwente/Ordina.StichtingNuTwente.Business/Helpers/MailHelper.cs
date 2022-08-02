using Microsoft.Extensions.Configuration;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Models.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

public class MailHelper
{
    private string mailAdressFrom { get; set; }
    private string mailFromName { get; set; }
    private string apiKey { get; set; }

    public MailHelper(string ApiKey, string MailAdressFrom, string MailFromName)
    {
        apiKey = ApiKey;
        mailAdressFrom = MailAdressFrom;
        mailFromName = MailFromName;
    }

    public async Task<bool> SendMail(Mail mail)
    {
        //TODO: checks invoeren om te kijken of het correcte mailadressen zijn.
        if (mail.MailFromName == null || mail.MailFromName == "")
        {
            mail.MailFromName = mailFromName;
        }
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(mailAdressFrom, mail.MailFromName),
            Subject = mail.Subject,
            PlainTextContent = mail.Message
        };
        msg.AddTo(new EmailAddress(mail.MailToAdress, mail.MailToName));
        var response = await client.SendEmailAsync(msg);

        Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SendGroupMail(string subject, string message, List<string> mailAdresses)
    {
        mailAdresses = mailAdresses.Distinct().ToList();

        List<EmailAddress> emailAddresses = new List<EmailAddress>();
        foreach(var mail in mailAdresses)
        {
            if (mail!="")
            {
                emailAddresses.Add(new EmailAddress(mail));
            }
        }
        var client = new SendGridClient(apiKey);

        EmailAddress fromMail = new EmailAddress(mailAdressFrom, mailFromName);

        var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmailToMultipleRecipients(fromMail, emailAddresses, subject, message, null) ;

        var response = await client.SendEmailAsync(msg);

        Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");
        return response.IsSuccessStatusCode;
    }
}
