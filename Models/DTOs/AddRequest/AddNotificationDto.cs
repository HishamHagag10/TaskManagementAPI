namespace TaskManagement.API.Models.DTOs.AddRequest
{
    public class AddNotificationDto
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public string ToUserId { get; set; }
    }
}
