namespace TaskManagement.API.Services.Dashboard_Services
{
    public partial class DashbaordService
    {
        public class ProjectDashboardDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
        }

    }
}
