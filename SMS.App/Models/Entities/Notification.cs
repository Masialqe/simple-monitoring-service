namespace SMS.App.Models.Entities
{
    public class Notification
    {
        /// <summary>
        /// Subject of notification.
        /// </summary>
        public string Subject { get; set; } = string.Empty;
        /// <summary>
        /// Message (body) of notification.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    public static class NotificationExtensions
    {
        public static Notification SetMessage(this Notification notification,string message)
        {
            notification.Message = message;
            return notification;
        }

        public static Notification SetSubject(this Notification notification,string subject)
        {
            notification.Subject = subject;
            return notification;
        }
    }
}
