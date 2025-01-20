using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagement.API.Models.DTOs.UpdateRequest;

namespace TaskManagement.API.Helpers.Validators.DatabaseValidators
{
    public interface IDTOValidator
    {
        Task<IEnumerable<string>?> AddTaskValidateAsync(AddTaskDto dto);
        Task<IEnumerable<string>?> UpdateTaskValidateAsync(UpdateTaskDto dto, Models.Task task);
        Task<IEnumerable<string>?> AssignUserToTaskValidateAsync(string userEmail,Models.Task task);
    }
}
