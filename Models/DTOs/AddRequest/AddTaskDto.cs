using NetTopologySuite.Index.Strtree;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TaskManagement.API.Helpers.Enums;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using TaskManagement.API.Services.UserService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.API.Models.DTOs.AddRequest
{
    public class AddTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public string? AssignedUserEmail { get; set; }
        public ICollection<int> TagsIds { get; set; }
    }
}
