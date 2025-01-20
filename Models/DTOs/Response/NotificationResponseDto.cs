namespace TaskManagement.API.Models.DTOs.Response
{
    public class NotificationResponseDto
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ReadAt { get; set; }
        public bool IsRead { get; set; }
    }
}
