using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Models.DTOs.DashboradDTOs
{
    public class UserDashboardResponseDto
    {
        public UserDto User { get; set; }
        public TaskSummaryDto TaskSummaryDto { get; set; }
        public IEnumerable<TaskSimplifiedDto> UpcomingTasks { get; set; }
        public IEnumerable<CommentResponseDto> RecentComments { get; set; }

    }
}
