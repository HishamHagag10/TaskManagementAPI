using Microsoft.EntityFrameworkCore;

namespace TaskManagement.API.Models
{
    [PrimaryKey("ProjectId","UserEmail")]
    public class UserProject
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string UserEmail { get; set; }
        public User User { get; set; }
    }
}
