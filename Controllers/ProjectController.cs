using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Helpers.Mapping;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagement.API.Models.DTOs.UpdateRequest;
using TaskManagement.API.Services.UserService;
using TaskManagementAPI.Erorrs;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using System.Security.Claims;
using TaskManagement.API.Helpers.Enums;
using TaskManagementAPI.Helpers;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme
       ,Roles ="Admin,User")]
    public class ProjectController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly string NotFoundMessage = "No Project With this Id";
        public ProjectController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        [HttpGet]
        [Route("GetProjects")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> GetProjectsAsync(
            [FromQuery] string? userEmail,
            [FromQuery] int? status,
            [FromQuery] int? pageIndex, [FromQuery] int? pageSize,
            [FromQuery] string? sortBy, [FromQuery] string? sortType
            )
        {
            var specification = new ProjectsSpecifications(status, userEmail);

            var count = await _unitOfWork.Projects.CountAsync(specification);
            specification.AddPagenation(pageIndex, pageSize);
            specification.ApplyOrdering(sortBy,sortType);
            var projects = await _unitOfWork.Projects.ListAsync(specification);
            var response = projects.ToResponse(pageIndex, pageSize, count);

            return Ok(response);
        }

        [HttpGet]
        [Route("GetProjectById/{id:int}")]
        public async Task<IActionResult> GetProjectByIdAsync(int id)
        {
            var projectSpecification = new ProjectsSpecifications(id);
            var project = await _unitOfWork.Projects.
                GetEntityAsync(projectSpecification);
            if (project == null)
                return NotFound(new ApiErrorResponse(404));
            return Ok(project.ToDetailsDto());
        }
        [HttpGet]
        [Route("MyProjects")]
        public async Task<IActionResult> GetMyProjectsAsync(
            [FromQuery] int? pageIndex, [FromQuery] int? pageSize,
            [FromQuery] string? sortBy, [FromQuery] string? sortType)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new ApiErrorResponse(401));

            var specification = new ProjectsSpecifications(userEmail);

            var count = await _unitOfWork.Projects.CountAsync(specification);
            specification.AddPagenation(pageIndex, pageSize);
            specification.ApplyOrdering(sortBy, sortType);

            var projects = await _unitOfWork.Projects.ListAsync(specification);
            var response = projects.ToResponse(pageIndex, pageSize, count);
            return Ok(response);
        }

        [HttpPut]
        [Route("UpdateProjectStatus/{id:int}")]
        public async Task<IActionResult> UpdateProjectStatusAsync(int id,int status)
        {
            if (status <= 0 || status > Helper.MaxProjectStatus)
                return BadRequest(new ApiValidationErrorResponse { Errors = new List<string> { $"Status must be grater than 0 and less than or equal {Helper.MaxProjectStatus}" } });

            var specifications = new ProjectsSpecifications(id);
            var project = await _unitOfWork.Projects
                .GetEntityAsync(specifications);
            if (project == null)
                return NotFound(new ApiErrorResponse(404));

            project.Status = (ProjectStatus)status;
            await _unitOfWork.SaveChangesAsync();

            return Ok(project.ToDetailsDto());
        }

        [HttpPut]
        [Route("AssignManager/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> AssignManagerToProjectAsync(int id,string managerEmail)
        {
            var specifications = new ProjectsSpecifications(id);
            var project = await _unitOfWork.Projects
                .GetEntityAsync(specifications);

            if (project == null)
                return NotFound(new ApiErrorResponse(404));

            var manager = await _userService.FindByEmailAsync(managerEmail);
            if (manager == null)
                return NotFound(new ApiErrorResponse(404));

            var isManger = await _userService.IsInRoleAsync(manager,Role.Admin);
            if (!isManger)
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string>{"This User is not a manager"}
                });

            project.ProjectManagerEmail = managerEmail;
            await _unitOfWork.SaveChangesAsync();
            return Ok(project.ToDetailsDto());
        }
        [HttpPut]
        [Route("UnassignManager/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> UnassignManagerToProjectAsync(int id)
        {
            var specifications = new ProjectsSpecifications(id);
            var project = await _unitOfWork.Projects
                .GetEntityAsync(specifications);

            if (project == null)
                return NotFound(new ApiErrorResponse(404));
            
            if (project.ProjectManagerEmail == null)
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "This Project doesn't have a Manager, already!" }
                });
            
            project.ProjectManagerEmail = null;
            await _unitOfWork.SaveChangesAsync();
            return Ok(project.ToDetailsDto());
        }
        [HttpPut]
        [Route("AssignUser/{id:int}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> AssignUserToProjectAsync(int id,string userEmail)
        {
            var specifications = new ProjectsSpecifications(id);
            var project = await _unitOfWork.Projects
                .GetEntityAsync(specifications);

            if (project == null)
                return NotFound(new ApiErrorResponse(404));
            
            var user = await _userService.FindByEmailAsync(userEmail);
            if (user == null)
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "The user does not exist" }
                });
            var isWorking = await _userService.IsUserWorkInProjectAsync(userEmail, project.Id);
            if (isWorking)
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "The user is already working in this project!" }
                });

            project.WorkingUsers.Add(user);
            await _unitOfWork.SaveChangesAsync();
            return Ok(project.ToDetailsDto());
        }
        [HttpPut]
        [Route("UnassignUser/{id:int}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> UnassignUserToProjectAsync(int id,string userEmail)
        {
            var specifications = new ProjectsSpecifications(id);
            var project = await _unitOfWork.Projects
                .GetEntityAsync(specifications);

            if (project == null)
                return NotFound(new ApiErrorResponse(404));

            var user = await _userService.FindByEmailAsync(userEmail);
            if (user == null)
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "The user does not exist" }
                });
            
            var isWorking = await _userService
                .IsUserWorkInProjectAsync(userEmail, project.Id);

            if (!isWorking)
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "The user does not work in this project!" }
                });

            project.WorkingUsers.Remove(user);
            await _unitOfWork.SaveChangesAsync();
            return Ok(project.ToDetailsDto());
        }

        [HttpPost]
        [Route("AddProject")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> AddProjectAsync(AddProjectDto dto)
        {
            if (!string.IsNullOrEmpty(dto.ProjectManagerEmail) && !await _userService.IsUserWithEmailExistAsync(dto.ProjectManagerEmail))
                return BadRequest(new ApiValidationErrorResponse { Errors = new List<string> { "User does not Exist, Please set valid user Id" } });

            var project = dto.ToModel();
            project = await _unitOfWork.Projects.AddAsync(project);
            
            await _unitOfWork.SaveChangesAsync();

            return Ok(project.ToDto());
        }

        [HttpPut]
        [Route("UpdateProject/{id:int}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> UpdateProjectAsync(int id, UpdateProjectDto dto)
        {
            if (dto.ProjectManagerEmail!=null && !await _userService.IsUserWithEmailExistAsync(dto.ProjectManagerEmail))
                return BadRequest(new ApiValidationErrorResponse { Errors = new List<string> { "User does not Exist, Please set valid user Id" } });

            var specifications = new ProjectsSpecifications(id);
            var project = await _unitOfWork.Projects
                .GetEntityAsync(specifications);

            if (project == null)
                return NotFound(new ApiErrorResponse(404, NotFoundMessage));

            project = dto.ToModel(project);

            if (project.End_date <= project.Start_date.AddDays(7))
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new List<string> { "Please set Valid End Date, It must be greater than the start Date by at least a Week" },
                });

            await _unitOfWork.SaveChangesAsync();

            return Ok(project.ToDetailsDto());
        }

        [HttpDelete]
        [Route("DeleteProject/{id:int}")]

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
       , Roles = "Admin")]
        public async Task<IActionResult> DeleteProjectAsync(int id)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(id);

            if (project == null)
                return NotFound(new ApiErrorResponse(404, NotFoundMessage));

            project = _unitOfWork.Projects.Delete(project);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Project Deleted Successfully");
        }
    }
}
