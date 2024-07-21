namespace SMS.App.Models.Exceptions
{
    public class MissingIntervalSettingsException: Exception
    {
        public MissingIntervalSettingsException(string message = "Missing interval settings. Should be placed in appsettings.json.")
            : base(message) { }
    }
}
