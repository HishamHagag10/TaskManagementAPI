using Hangfire;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
namespace TaskManagement.API.Services.NotificationService
{
    public class TaskNotificationService : ITaskNotificationService
    {
        private readonly NotificationsManager _notificationManager;
        private readonly IUnitOfWork _unitOfWork;
        public TaskNotificationService(NotificationsManager notificationManager, IUnitOfWork unitOfWork)
        {
            _notificationManager = notificationManager;
            _unitOfWork = unitOfWork;
        }
        public bool SendTaskAssignedEmail(string taskTitle, string taskDescription, string userEmail)
        {
            var subject = "You have been assigned a new task";
            var emailBody = $@"<html>
                                <body>
                <h2>Hello Tasker </h2>
                <h3>You have been assigned the task: {taskTitle}</h3> <br>
                <p>Description: {taskDescription}</p> <br>
                     <h6>Best regards<br>
                       Your Task Management System</h6>
                    </body>
                        </html>
                     ";
            return _notificationManager.SendEmail(userEmail!, subject, emailBody);
        }
        public bool SendTaskUnAssignedEmail(string taskTitle, string taskDescription, string userEmail)
        {
            var subject = "You have been removed from the task";
            var emailBody = $@"<html>
                                <body>
                <h2>Hello Tasker </h2>
                <h3> unfortunately you have been removed from the task: {taskTitle}</h3> <br>
                    <h6>Best regards<br>
                       Your Task Management System</h6>
                    </body>
                        </html>
                     ";
            return _notificationManager.SendEmail(userEmail!, subject, emailBody);
        }
        public async System.Threading.Tasks.Task SendTaskDeadlineApproachingEmailAsync()
        {
            var tomorrow = DateTime.UtcNow.AddDays(1);
            var builder = new TasksSpecificationBuilder();
            builder.AddDeadlineFilter(tomorrow);
            builder.IncludeAssignedUser();
            var tasksDeadlineTomorrow = await _unitOfWork.Tasks
                .ListAsync(builder.Build());
            foreach (var task in tasksDeadlineTomorrow)
            {
                if (task.AssignedUser != null)
                    SendTaskDeadlineEmail(task);
            }
        }


        private bool SendTaskDeadlineEmail(Models.Task task)
        {
            var subject = "Task deadline approaching!";
            var emailBody = $@"<html>
                                <body>
                <h2>Hello: {task.AssignedUser?.FullName}</h2>
                <h3>The deadline for task '{task.Title}' is approaching soon.<br> 
                        Please ensure to complete it on time</h3>
                     <h6>Best regards<br>
                       Your Task Management System</h6>
                    </body>
                        </html>
                     ";
            return _notificationManager.SendEmail(task.AssignedUserEmail!, subject, emailBody);
        }
    }
}
