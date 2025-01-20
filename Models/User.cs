using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public IEnumerable<Task> Tasks { get; set; }
        [JsonIgnore]
        public IEnumerable<Comment> Comments { get; set; }
        [JsonIgnore]
        public IEnumerable<Project> ProjectsWorkingAt { get; set; }
        [JsonIgnore]
        public IEnumerable<Project> ManageProjects { get; set; }
        [JsonIgnore]
        public IEnumerable<Notification> Notifications { get; set; }

    }
}
