using System.Text.Json.Serialization;
using TaskManagement.API.Helpers.Enums;
using TaskManagement.API.Models.DTOs;

namespace TaskManagement.API.Models
{
    public class Project 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
        public DateTime Start_date { set; get; }
        public DateTime End_date { set; get; } 
        public ProjectStatus Status { set; get; }
        public string? ProjectManagerEmail { get; set; }
        [JsonIgnore]
        public User? ProjectManager { get; set;}
        [JsonIgnore]
        public IEnumerable<Task> Tasks { get; set; }
        [JsonIgnore]
        public ICollection<User> WorkingUsers { get; set; }
    }
}
