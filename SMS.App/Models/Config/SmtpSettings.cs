namespace SMS.App.Models.Config
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name {  get; set; } = string.Empty;
        public string[] NotificationAddresses { get; set; } 
            = Array.Empty<string>();
    }
}
