namespace TaskManagement.API.Models.DTOs.DashboradDTOs
{
    public class TaskSummaryDto
    {
        public int TotalTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int NotCompletedTasks => TotalTasksCount - CompletedTasksCount;
        public int TasksDueToday { get; set; }
        public IEnumerable<TaskSimplifiedDto> RecentTasks { get; set; }
    };
}
