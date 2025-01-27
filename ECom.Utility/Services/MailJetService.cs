using ECom.Utility.Settings;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class MailJetService
{
    private readonly IConfiguration _configuration;
    public string APIKey { get; set; }
    public string SecretKey { get; set; }
    public string EmailFrom { get; set; }
    public string SenderName { get; set; }
    public MailJetService(IConfiguration configuration)
    {
        _configuration = configuration;
        var MailJetCredential = _configuration.GetSection("MailJet").Get<MailJetSettings>();
        APIKey =  MailJetCredential.APIKey;
        SecretKey = MailJetCredential.SecretKey;
        EmailFrom = MailJetCredential.EmailFrom;
        SenderName = MailJetCredential.SenderName;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new MailjetClient(APIKey, SecretKey);

        var request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.FromEmail, EmailFrom)
        .Property(Send.FromName, SenderName)
        .Property(Send.Subject, subject)
        .Property(Send.TextPart, body)
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
        var client = new MailjetClient(APIKey, SecretKey);
        var attachmentBase64 = System.Convert.ToBase64String(attachment);

        var request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.FromEmail, EmailFrom)
        .Property(Send.FromName, SenderName)
        .Property(Send.Subject, subject)
        .Property(Send.TextPart, body)
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
        var client = new MailjetClient(APIKey, SecretKey);
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
        .Property(Send.FromEmail, EmailFrom)
        .Property(Send.FromName, SenderName)
        .Property(Send.Subject, subject)
        .Property(Send.TextPart, body)
        .Property(Send.Recipients, recipients);

        await client.PostAsync(request);
    }
}