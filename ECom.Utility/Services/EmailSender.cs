using ECom.Utility.Interface;
using ECom.Utility.Settings;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace ECom.Utility.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IMailJetService _mailJetService;
        public EmailSender(IMailJetService mailJetService)
        {
            _mailJetService = mailJetService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //return Task.CompletedTask;
            await _mailJetService.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
