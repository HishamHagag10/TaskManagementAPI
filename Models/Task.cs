using TaskManagement.API.Helpers.Enums;
using TaskManagement.API.Models.DTOs;

namespace TaskManagement.API.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public TaskManagement.API.Helpers.Enums.TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string? AssignedUserEmail { get; set; }
        public User? AssignedUser { get; set; }
        public ICollection<Comment>Comments  { get; set; }
        public ICollection<Tag> Tags { get; set; }
        
    }
}
