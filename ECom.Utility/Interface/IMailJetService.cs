namespace ECom.Utility.Interface
{
    public interface IMailJetService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);

        Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string fileName);

        Task SendBulkEmailAsync(List<string> toEmails, string subject, string body);
    }
}
