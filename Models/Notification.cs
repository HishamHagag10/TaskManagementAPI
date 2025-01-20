namespace TaskManagement.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string ToUserId { get; set; }
        public User ToUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ReadAt { get; set; }
        public bool IsRead { get; set; }
    }
}
