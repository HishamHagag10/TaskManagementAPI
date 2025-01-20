using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Models.DTOs.Response
{
    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime Start_date { set; get; }
        public DateTime End_date { set; get; }
        public string Status { set; get; }
        public string? ProjectManagerEmail { get; set; }

    }
}
