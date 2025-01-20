namespace TaskManagement.API.Models.DTOs.DashboradDTOs
{
    public class TaskSimplifiedDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}
