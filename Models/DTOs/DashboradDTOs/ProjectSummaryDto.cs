namespace TaskManagement.API.Services.Dashboard_Services
{
    public partial class DashbaordService
    {
        public class ProjectSummaryDto
        {
            public int TotalProjects { get; set; }
            public IEnumerable<ProjectDashboardDto> RecentProjects {  get; set; }
        }

    }
}
