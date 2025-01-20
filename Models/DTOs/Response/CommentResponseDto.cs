namespace TaskManagement.API.Models.DTOs.Response
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int TaskId { get; set; }
        public string UserEmail { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
