using TaskManagement.API.Helpers.Enums;

namespace TaskManagement.API.Models.DTOs.Response
{
    public class TaskWithDetailsDto
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
        public string Project { get; set; }
        public string? AssignedUserEmail { get; set; }
        public string? AssignedUserName { get; set; }
        public ICollection<CommentResponseDto> Comments { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}
