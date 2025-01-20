using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Models.DTOs.UpdateRequest;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using TaskManagement.API.Services.UserService;
using TaskManagementAPI.Erorrs;

namespace TaskManagement.API.Helpers.Validators.DatabaseValidators
{
    public class DTOValidator : IDTOValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public DTOValidator(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<IEnumerable<string>?> AddTaskValidateAsync(AddTaskDto dto)
        {
            var errors = new List<string>();
            if (!await _unitOfWork.Projects.AnyAsync(x => x.Id == dto.ProjectId))
                errors.Add("Project does not Exist, Please set valid project Id");
            if (!string.IsNullOrEmpty(dto.AssignedUserEmail))
            {
                var user = await _userService.FindByEmailAsync(dto.AssignedUserEmail);
                if (user == null)
                    errors.Add("User does not Exist, Please set valid user Email");

                if (!await _userService.IsUserWorkInProjectAsync(dto.AssignedUserEmail, dto.ProjectId))
                    errors.Add("The User don't work at this project");
            }
            if (dto.TagsIds != null)
            {
                var tagSpecification = new TagsWithIdsSpecifications(dto.TagsIds);
                var tagsCount = await _unitOfWork.Tags.CountAsync(tagSpecification);
                if (tagsCount != dto.TagsIds.Count)
                    errors.Add("Some of tags not Exist, Please assign Exist Tags");
            }
            return errors.Count != 0 ? errors : null;
        }

        public async Task<IEnumerable<string>?> AssignUserToTaskValidateAsync(string userEmail, Models.Task task)
        {
            if (task.AssignedUserEmail != null)
            {
                return new List<string> { "The User is already Assigned to this task." };
            }
            var isexist = await _userService.IsUserWithEmailExistAsync(userEmail);
            if (!isexist)
            {
                return new List<string> { "No user with this Email" };
            }
            if (!await _userService.IsUserWorkInProjectAsync(userEmail, task.ProjectId))
            {
                return new List<string> { "The User don't work at this project" };
            }
            return null;
        }

        public async Task<IEnumerable<string>?> UpdateTaskValidateAsync(UpdateTaskDto dto,Models.Task task)
        {
            if (dto.ProjectId.HasValue && 
                !await _unitOfWork.Projects.AnyAsync(x => x.Id == dto.ProjectId))
                return new List<string> { "Project does not Exist, Please set valid project Id" };

            if (!string.IsNullOrEmpty(dto.AssignedUserEmail))
            {
                if (task.AssignedUserEmail != null)
                {
                    return new List<string> { "A User is already Assigned to this task." };
                }
                var user = await _userService.FindByEmailAsync(dto.AssignedUserEmail);
                if (user == null)
                    return new List<string> { "User does not Exist, Please set valid user Email" };
                
                var projectId = dto.ProjectId ?? task.ProjectId;
                if (!await _userService.IsUserWorkInProjectAsync(dto.AssignedUserEmail, projectId))
                    return new List<string> { "The User don't work at this project" };
            }

            if (dto.TagsIds != null)
            {
                var tagSpecification = new TagsWithIdsSpecifications(dto.TagsIds);
                var tagsCount = await _unitOfWork.Tags.CountAsync(tagSpecification);
                if (tagsCount != dto.TagsIds.Count)
                    return new List<string> { "Some of tags not Exist, Please assign Exist Tags" };
            }
            return null;
        }
    }
}
