namespace TaskManagement.API.Services.NotificationService
{
    public interface ITaskNotificationService
    {
        bool SendTaskAssignedEmail(string taskTitle, string taskDescription, string userEmail);
        bool SendTaskUnAssignedEmail(string taskTitle, string taskDescription, string userEmail);
        Task SendTaskDeadlineApproachingEmailAsync();
    }
}
