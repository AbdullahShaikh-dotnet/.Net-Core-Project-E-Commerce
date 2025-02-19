namespace ECom.Utility.Settings
{
    public class MailJetSettings
    {
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public string EmailFrom { get; set; }
        public string SenderName { get; set; }

        // Method to convert a document to Base64
        public string ConvertDocumentToBase64(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("The specified file does not exist.", filePath);
                }
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string base64String = Convert.ToBase64String(fileBytes);
                if (!string.IsNullOrEmpty(base64String)) File.Delete(filePath);
                return base64String;
            }
            catch (Exception ex)
            {
                File.Delete(filePath);
                throw new Exception("An error occurred while converting the document to Base64.", ex);
            }
        }
    }
}