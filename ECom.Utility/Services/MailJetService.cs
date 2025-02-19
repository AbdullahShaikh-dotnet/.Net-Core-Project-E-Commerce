using ECom.Utility.Interface;
using ECom.Utility.Settings;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

public class MailJetService : IMailJetService
{
    private readonly MailJetSettings _mailJetSettings;
    public MailJetService(IOptions<MailJetSettings> mailJetSettings)
    {
        _mailJetSettings = mailJetSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new MailjetClient(_mailJetSettings.APIKey, _mailJetSettings.SecretKey);

        var request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.FromEmail, _mailJetSettings.EmailFrom)
        .Property(Send.FromName, _mailJetSettings.SenderName)
        .Property(Send.Subject, subject)
        .Property(Send.HtmlPart, body)
        .Property(Send.Recipients, new JArray
        {
            new JObject
            {
                {"Email", toEmail}
            }
        });

        await client.PostAsync(request);
    }

    public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string fileName)
    {
        var client = new MailjetClient(_mailJetSettings.APIKey, _mailJetSettings.SecretKey);
        var attachmentBase64 = System.Convert.ToBase64String(attachment);

        var request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.FromEmail, _mailJetSettings.EmailFrom)
        .Property(Send.FromName, _mailJetSettings.SenderName)
        .Property(Send.Subject, subject)
        .Property(Send.HtmlPart, body)
        .Property(Send.Recipients, new JArray
        {
            new JObject
            {
                {"Email", toEmail}
            }
        })
        .Property(Send.Attachments, new JArray
        {
            new JObject
            {
                {"ContentType", "application/octet-stream"},
                {"Filename", fileName},
                {"Base64Content", attachmentBase64}
            }
        });
        await client.PostAsync(request);
    }

    public async Task SendBulkEmailAsync(List<string> toEmails, string subject, string body)
    {
        var client = new MailjetClient(_mailJetSettings.APIKey, _mailJetSettings.SecretKey);
        var recipients = new JArray();

        foreach (var email in toEmails)
        {
            recipients.Add(new JObject
            {
                {"Email", email}
            });
        }
        var request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.FromEmail, _mailJetSettings.EmailFrom)
        .Property(Send.FromName, _mailJetSettings.SenderName)
        .Property(Send.Subject, subject)
        .Property(Send.HtmlPart, body)
        .Property(Send.Recipients, recipients);

        await client.PostAsync(request);
    }
}