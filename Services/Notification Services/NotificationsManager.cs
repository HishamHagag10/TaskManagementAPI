namespace TaskManagement.API.Services.NotificationService
{
    public class NotificationsManager
    {
        private readonly INotificationService _emailNotificationService;

        public NotificationsManager(EmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }
        public bool SendEmail(string recipient, string subject, string body)
        {
            return _emailNotificationService.Notify(recipient, subject, body);
        }
        public bool SendSMS(string recipient, string subject, string body)
        {
            // SMS Notification Service
            return true;
        }
        public bool SendPushNotification(string recipient, string subject, string body)
        {
            // Push Notification Service
            return true;
        }
        public bool SendNotification(string recipient, string subject, string body)
        {
            SendEmail(recipient, subject, body);
            return true;
        }

    }
}
