using TaskManagement.API.Models.DTOs.DashboradDTOs;

namespace TaskManagement.API.Services.Dashboard_Services
{
    public partial class DashbaordService
    {
        public class UsersSummaryDto
        {
           public int TotalUsers { get; set; }
           public IEnumerable<UserDto> RecentUsers { get; set; }
        }

    }
}
