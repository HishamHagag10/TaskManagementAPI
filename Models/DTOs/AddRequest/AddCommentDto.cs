namespace TaskManagement.API.Models.DTOs.AddRequest
{
    public class AddCommentDto
    {
        public string Content { get; set; }
        public int TaskId { get; set; }
    }
}
