namespace TaskManagement.API.Services.NotificationService
{
    public interface INotificationService
    {
        bool Notify(string recipient, string subject, string body);
    }
}
