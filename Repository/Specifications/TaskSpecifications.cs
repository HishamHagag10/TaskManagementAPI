namespace TaskManagement.API.Repository.Specifications
{
    public static class TaskSpecifications
    {
        public static TasksSpecificationBuilder GetTaskOfUserSpecification(string? userEmail)
        {
            var builder = new TasksSpecificationBuilder();
            if(userEmail != null)
            builder.AddUserFilter(userEmail);
            return builder;
        }
        public static TasksSpecificationBuilder GetTasksDueDateTodayOfUserSpecification(string? userEmail)
        {
            var builder = GetTaskOfUserSpecification(userEmail);
            builder.AddDeadlineFilter(DateTime.UtcNow);
            return builder;
        }

        public static TasksSpecificationBuilder GetCompletedTasksOfUserSpecification(string? userEmail)
        {
            var builder = GetTaskOfUserSpecification(userEmail);
            builder.AddStatusFilter((int)TaskManagement.API.Helpers.Enums.TaskStatus.Completed);
            return builder;
        }

        public static TasksSpecificationBuilder GetUpComingTasksOfUserSpecification(string? userEmail)
        {
            var builder = GetTaskOfUserSpecification(userEmail);
            builder.AddDeadlineDateRangeFilter(DateTime.UtcNow,DateTime.UtcNow.AddDays(1));
            return builder;
        }

        public static TasksSpecificationBuilder GetTaskWithIdSpecification(int id)
        {
            var builder = new TasksSpecificationBuilder();
            builder.AddIdFilter(id);
            builder.IncludeAssignedUser();
            builder.IncludeProject();
            builder.IncludeComments();
            return builder;
        }
    }
}
