namespace SMS.App.Models.Exceptions
{
    public class MissingSecretKeyException : Exception
    {
        public MissingSecretKeyException(string message = "Missing Secret Key. Should be provided as env SECRET_KEY")
            : base(message) { }
    }
}
