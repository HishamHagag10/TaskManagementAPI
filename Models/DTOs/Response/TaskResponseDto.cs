using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Models.DTOs.Response
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public string? AssignedUserEmail { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
