using TaskManagement.API.Models.DTOs.DashboradDTOs;
using TaskManagement.API.Models.DTOs.Response;

namespace TaskManagement.API.Services.Dashboard_Services
{
    public partial class DashbaordService
    {
        public class AdminDashboardResponseDto
        {
            public UserDto User { get; set; }
            public UsersSummaryDto UsersSummary { get; set; }
            public TaskSummaryDto TaskSummary { get; set; }
            public ProjectSummaryDto ProjectSummary { get; set; }
            public IEnumerable<CommentResponseDto> RecentComments { get; set; }
        }

    }
}
