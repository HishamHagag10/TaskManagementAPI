using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagement.API.Helpers.Mapping;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagement.API.Models.DTOs.UpdateRequest;
using TaskManagement.API.Services.NotificationService;
using TaskManagement.API.Services.UserService;
using TaskManagementAPI.Erorrs;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using FluentValidation;
using TaskManagement.API.Helpers.Validators.DatabaseValidators;
using System.Security.Claims;
using TaskManagement.API.Helpers.Enums;
using TaskManagementAPI.Helpers;
using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        ,Roles ="Admin,User")]
    public class TaskController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDTOValidator _dTOValidator;
        private readonly string NotFoundMessage = "No Task With this Id";
        public TaskController(IUnitOfWork unitOfWork, IDTOValidator dTOValidator)
        {
            _unitOfWork = unitOfWork;
            _dTOValidator = dTOValidator;
        }
        [HttpGet]
        [Route("GetTasks")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin")]
        public async Task<IActionResult> GetTasksAsync([FromQuery] int? projectId,
            [FromQuery] int? priority, [FromQuery] int? status,
            [FromQuery] string? userEmail,
            [FromQuery] int? pageIndex, [FromQuery] int? pageSize,
            [FromQuery] string? sortBy, [FromQuery] string? sortType)
        {
            var builder = new TasksSpecificationBuilder();

            builder.AddProjectFilter(projectId);
            builder.AddPriorityFilter(priority);
            builder.AddStatusFilter(status);
            
            if (!string.IsNullOrEmpty(userEmail))
            {
                if (userEmail.ToLower() == "unassigend")
                    userEmail = null;
                builder.AddUserFilter(userEmail);
            }
            
            var count = await _unitOfWork.Tasks.CountAsync(builder.Build());

            builder.ApplyOrdering(sortBy, sortType);
            builder.AddPagination(pageIndex,pageSize);
            
            var tasks = await _unitOfWork.Tasks.ListAsync(builder.Build());
            
            var response = tasks.ToResponse(pageIndex,pageSize,count);

            return Ok(response);
        }


        [HttpGet]
        [Route("GetTaskById/{id:int}")]
        public async Task<IActionResult> GetTaskByIdAsync(int id)
        {
            var builder = TaskSpecifications.GetTaskWithIdSpecification(id);

            var task = await _unitOfWork.Tasks.GetEntityAsync(builder);
            if (task == null)
                return NotFound(new ApiErrorResponse(404));

            return Ok(task.ToDetailsDto());
        }
        
        [HttpGet]
        [Route("MyTasks")]
        public async Task<IActionResult> GetMyTasksAsync([FromQuery] int? projectId,
            [FromQuery] int? priority, [FromQuery] int? status,
            [FromQuery] int? pageIndex, [FromQuery] int? pageSize,
            [FromQuery] string? sortBy, [FromQuery] string? sortType)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            
            if (userEmail == null)
                return Unauthorized(new ApiErrorResponse(401));

            var builder = new TasksSpecificationBuilder();
            builder.AddUserFilter(userEmail);
            builder.AddPriorityFilter(priority);
            builder.AddProjectFilter(projectId);
            builder.AddStatusFilter(status);

            var count = await _unitOfWork.Tasks.CountAsync(builder.Build());
            
            builder.AddPagination(pageIndex, pageSize);
            builder.ApplyOrdering(sortBy, sortType);
            var tasks = await _unitOfWork.Tasks.ListAsync(builder.Build());

            return Ok(tasks.ToResponse(pageIndex,pageSize,count));
        }

        [HttpGet]
        [Route("TasksDueSoon")]
        public async Task<IActionResult> GetTasksDueSoonAsync([FromQuery]int days=7,[FromQuery]int pageIndex=1,[FromQuery] int pageSize=10)
        {
            var builder = new TasksSpecificationBuilder();
            
            builder.AddDeadlineDateRangeFilter(DateTime.UtcNow,
                DateTime.UtcNow.AddDays(days));

            var count = await _unitOfWork.Tasks.CountAsync(builder.Build());
            
            builder.ApplyOrdering(TaskOrderBy.DueDate.ToString()
                ,OrderType.desc.ToString());
            
            builder.AddPagination(pageIndex, pageSize);
            var tasks = await _unitOfWork.Tasks.ListAsync(builder.Build());
            
            return Ok(tasks.ToResponse(pageIndex,pageSize,count));
        }

        [HttpPut]
        [Route("UpdateTaskStatus/{id:int}")]
        public async Task<IActionResult> UpdateTaskStatusAsync(int id,int status)
        {
            if (status <= 0 || status > Helper.MaxTaskStatus)
                return BadRequest(new ApiValidationErrorResponse 
                { Errors=new List<string> { $"Status must be grater than 0 and less than or equal {Helper.MaxTaskStatus}" } });

            var specification = TaskSpecifications.GetTaskWithIdSpecification(id);
            var task = await _unitOfWork.Tasks.GetEntityAsync(specification);

            if (task == null)
                return NotFound(new ApiErrorResponse(404));
            
            task.Status=(TaskManagement.API.Helpers.Enums.TaskStatus)status;
            await _unitOfWork.SaveChangesAsync();

            return Ok(task.ToDetailsDto());
        }

        [HttpPut]
        [Route("AssignUser/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin")]
        public async Task<IActionResult> AssignUserToTaskAsync(int id,string userEmail)
        {
            var specification = TaskSpecifications.GetTaskWithIdSpecification(id);
            var task = await _unitOfWork.Tasks.GetEntityAsync(specification);
            
            if (task == null)
                return NotFound(new ApiErrorResponse(404,"No task With this id"));

            var errors = await _dTOValidator.AssignUserToTaskValidateAsync(userEmail,task);
            if (errors != null)
                return BadRequest(new ApiValidationErrorResponse { Errors = errors });
            
            task.AssignedUserEmail = userEmail;    
            await _unitOfWork.SaveChangesAsync();

            BackgroundJob.Enqueue<ITaskNotificationService>(x =>
                            x.SendTaskAssignedEmail(task.Title, task.Description, userEmail));
            
            return Ok(task.ToDetailsDto());
        }
        [HttpPut]
        [Route("UnassignUser/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin")]
        public async Task<IActionResult> UnassignUserToTaskAsync(int id)
        {
            var specification = TaskSpecifications.GetTaskWithIdSpecification(id);
            var task = await _unitOfWork.Tasks.GetEntityAsync(specification);
            if (task == null)
                return NotFound(new ApiErrorResponse(404, "No task With this id"));

            if (task.AssignedUserEmail == null)
                return BadRequest(new ApiValidationErrorResponse
                { Errors = new List<string> { "Task is already Unassigned" } });

            var previousUserEmail = task.AssignedUserEmail;
            task.AssignedUserEmail = null;
            task.AssignedUser = null;
            await _unitOfWork.SaveChangesAsync();

            BackgroundJob.Enqueue<ITaskNotificationService>(x =>
                            x.SendTaskUnAssignedEmail(task.Title, task.Description, previousUserEmail));
            return Ok(task.ToDetailsDto());
        }

        [HttpPost]
        [Route("AddTask")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin")]
        public async Task<IActionResult> AddTaskAsync(AddTaskDto dto)
        {
            var errors = await _dTOValidator.AddTaskValidateAsync(dto);
            if (errors != null)
            {
                return BadRequest(new ApiValidationErrorResponse { Errors = errors });
            }

            var task = dto.ToModel();

            var tagSpecification = new TagsWithIdsSpecifications(dto.TagsIds);
            var tags = await _unitOfWork.Tags
                .ListAsync(tagSpecification);
            
            task.Tags = tags;
            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            if (task.AssignedUser != null)
            {
                BackgroundJob.Enqueue<ITaskNotificationService>(x =>
                                x.SendTaskAssignedEmail(task.Title,task.Description,
                                                  task.AssignedUserEmail!));
                //_taskNotificationService.SendTaskAssignedEmailAsync(task.AssignedUser,task);
            }
            return Ok(task.ToDto());
        }

        [HttpPut]
        [Route("UpdateTask/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin")]
        public async Task<IActionResult> UpdateTaskAsync(int id,UpdateTaskDto dto)
        {
            var specification = TaskSpecifications.GetTaskWithIdSpecification(id);
            var task = await _unitOfWork.Tasks.GetEntityAsync(specification);
            if (task == null) 
                return NotFound(new ApiErrorResponse(404,NotFoundMessage));
            
            var errors = await _dTOValidator.UpdateTaskValidateAsync(dto,task);
            if (errors != null)
            {
                return BadRequest(new ApiValidationErrorResponse { Errors = errors });
            }

            if (dto.TagsIds != null)
            {
                var tagSpecification = new TagsWithIdsSpecifications(dto.TagsIds);
                var tags = await _unitOfWork.Tags
                    .ListAsync(tagSpecification);
                task.Tags = tags;
            }
            task = dto.ToModel(task);
            await _unitOfWork.SaveChangesAsync();
            
            if(!string.IsNullOrEmpty(dto.AssignedUserEmail))
            {
                BackgroundJob.Enqueue<ITaskNotificationService>(x =>
                                    x.SendTaskAssignedEmail(task.Title, task.Description,
                                                       task.AssignedUserEmail!));
            }
            
            return Ok(task.ToDto());
        }
        
        [HttpDelete]
        [Route("DeleteTask/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin")]
        public async Task<IActionResult> DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null) 
                return NotFound(new ApiErrorResponse(404,NotFoundMessage));
            task = _unitOfWork.Tasks.Delete(task);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Task Deleted Successfully");
        }
    }
}
