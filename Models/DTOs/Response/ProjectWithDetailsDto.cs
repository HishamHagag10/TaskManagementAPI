using TaskManagement.API.Models.DTOs.DashboradDTOs;

namespace TaskManagement.API.Models.DTOs.Response
{
    public class ProjectWithDetailsDto
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
        public IEnumerable<TaskSimplifiedDto> Tasks { get; set; }
        public IEnumerable<UserDto> WorkingUsers { get; set; }
    }
}
