using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using TaskManagement.API.Services.UserService;

namespace TaskManagement.API.Models.DTOs.UpdateRequest
{
    public class UpdateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public int? ProjectId { get; set; }
        public string? AssignedUserEmail { get; set; }
        public ICollection<int>? TagsIds { get; set; }
    }
}
