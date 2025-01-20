using System.Text.Json.Serialization;

namespace TaskManagement.API.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public IEnumerable<Task> Tasks { get; set; }
    }
}
