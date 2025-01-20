namespace TaskManagement.API.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int TaskId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Task Task { get; set; }
        public string UserEmail { get; set; }
        public User User { get; set; }

    }
}
