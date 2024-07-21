namespace SMS.App.Models.Exceptions
{
    public class MissingTargetListException : Exception
    {
        public MissingTargetListException(string message = "Missing target list. Should be placed in appsettings.json")
            : base(message) { }
    }
}



