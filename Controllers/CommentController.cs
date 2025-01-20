using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Helpers.Mapping;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagement.API.Services.UserService;
using TaskManagementAPI.Erorrs;
using TaskManagementAPI.Helpers;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, 
        Roles = "User,Admin")]
    public class CommentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly string NotFoundMessage = "No Comment With this Id";
        public CommentController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        [HttpGet]
        [Route("GetComment")]
        public async Task<IActionResult> GetComments([FromQuery]int? taskId
            ,[FromQuery] string? userEmail, [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize, [FromQuery] string? sortBy, [FromQuery] string? sortType) 
        {
            var commentSpecifications = new CommentSpecifications(taskId,userEmail,sortBy,sortType);
            var count = await _unitOfWork.Comments.CountAsync(commentSpecifications);
            
            commentSpecifications.AddPagenation(pageIndex, pageSize);
            var comments = await _unitOfWork.Comments.ListAsync(commentSpecifications);
            var response = comments.ToResponse(pageIndex, pageSize, count);
            
            return Ok(response);
        }
        [HttpGet]
        [Route("GetMyComment")]
        public async Task<IActionResult> GetMyComments([FromQuery] int? taskId
            , [FromQuery] int? pageIndex,[FromQuery] int? pageSize, 
            [FromQuery] string? sortBy, [FromQuery] string? sortType)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));
            
            if (user.Email == null) return Unauthorized(new ApiErrorResponse(401));

            var commentSpecifications = new CommentSpecifications(taskId, user.Email, sortBy, sortType);
            var count = await _unitOfWork.Comments.CountAsync(commentSpecifications);

            commentSpecifications.AddPagenation(pageIndex, pageSize);
            var comments = await _unitOfWork.Comments.ListAsync(commentSpecifications);
            var response = comments.ToResponse(pageIndex, pageSize, count);

            return Ok(response);
        }
        [HttpPost]
        [Route("AddComment")]
        public async Task<IActionResult> AddCommentAsync(AddCommentDto dto)
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));

            
            if (!await _unitOfWork.Tasks.AnyAsync(x => x.Id == dto.TaskId))
                return BadRequest(new ApiValidationErrorResponse { Errors = new List<string> { "Task does not Exist, Please set valid project Id" } });

            var comment = dto.ToModel(user.Email!);
            comment=await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return Ok(comment.ToDto());
        }
        private async Task<User?> GetCurrentUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(role))
                return null;

            var user = await _userService.FindByIdAsync(userId);
            if (user == null || !(await _userService.IsInRoleAsync(user, role)))
                return null;
            return user;
        }
        [HttpPut]
        [Route("UpdateComment/{id:int}")]
        public async Task<IActionResult> UpdateCommentAsync(int id,[FromBody]string content)
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));

            if (string.IsNullOrWhiteSpace(content))
                return BadRequest(new ApiErrorResponse(400, "Content must not be empty."));

            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            
            if (comment == null)
                return NotFound(new ApiErrorResponse(404,NotFoundMessage));
            
            if (user.Email != comment.UserEmail)
                return Forbid();
            
            comment.Content = content;
            comment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return Ok(comment.ToDto());
        }
        [HttpDelete]
        [Route("DeleteComment/{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var user = await GetCurrentUserAsync();
            
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));

            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            
            if (comment == null)
                return NotFound(new ApiErrorResponse(404, NotFoundMessage));
            
            var isAdmin = await _userService.IsInRoleAsync(user,Role.Admin);
            if (comment.UserEmail == user.Email || isAdmin)
            {
                _unitOfWork.Comments.Delete(comment);
                await _unitOfWork.SaveChangesAsync();
                return Ok("Comment Deleted Successfully");
            }
            return Forbid();
        }
    }
}
