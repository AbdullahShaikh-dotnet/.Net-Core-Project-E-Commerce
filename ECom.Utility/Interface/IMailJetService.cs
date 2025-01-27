using Mailjet.Client.Resources;
using Mailjet.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Utility.Interface
{
    public interface IMailJetService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);

        Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string fileName);

        Task SendBulkEmailAsync(List<string> toEmails, string subject, string body);
    }
}
