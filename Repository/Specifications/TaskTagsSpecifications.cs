using TaskManagement.API.Models;

namespace TaskManagement.API.Repository.Specifications
{
    public class TaskTagsSpecifications
        :BaseSpecification<TaskTag>
    {
        public TaskTagsSpecifications(int taskId)
        {
            AddCriteria(x => x.TaskId == taskId);
        }
    }
}
