namespace TaskManagement.API.Models.DTOs.AddRequest
{
    public class AddProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start_date { set; get; }
        public DateTime End_date { set; get; }
        public int Status { set; get; }
        public string? ProjectManagerEmail { get; set; }
    }
}
