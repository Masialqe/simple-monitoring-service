namespace SMS.App.Models.Exceptions
{
    public class MissingSmtpConfigurationException : Exception
    {
        public MissingSmtpConfigurationException(string message = "Missing SMTP confuguration. Should be placed in appsettings.json.")
            :base(message) { }
    }
}
