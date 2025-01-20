using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.Helpers.Mapping;
using TaskManagement.API.Models;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using TaskManagement.API.Services.NotificationService;
using TaskManagement.API.Services.UserService;
using TaskManagementAPI.Erorrs;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        public NotificationsController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        [HttpGet]
        [Route("GetNotifications")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin,User")]
        public async Task<IActionResult> GetNotificationsAsync([FromQuery]int pageIndex=1,[FromQuery]int pageSize=10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var specifications = new NotificationSpecification(userId);
            var count= await _unitOfWork.Notifications.CountAsync(specifications);
            specifications.AddPagination(pageIndex, pageSize);
            var notifications = await _unitOfWork.Notifications
                .ListAsync(specifications);

            return Ok(notifications.ToResponse(pageIndex,pageSize,count));
        }


        [HttpPost]
        [Route("MarkAsRead/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin,User")]
        public async Task<IActionResult> MarkAsReadAsync(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var notification = await _unitOfWork.Notifications
                .GetEntityAsync(new NotificationSpecification(userId, Id));
            
            if (notification == null)
                return NotFound(new ApiErrorResponse(404));
            if(notification.IsRead)
                return BadRequest(new ApiErrorResponse(400, "Notification is already read"));
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
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
        [HttpPost]
        [Route("MarkAllAsRead")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme,
            Roles ="Admin,User")]
        public async Task<IActionResult> MarkAllAsReadAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));
            var specifications = new NotificationSpecification(user.Id,false);
            var notifications = await _unitOfWork.Notifications
                .ListAsync(specifications);
            notifications.ToList().ForEach(x => {
                x.IsRead = true;
                x.ReadAt = DateTime.UtcNow;
            });
            await _unitOfWork.Notifications.UpdateRangeAsync(notifications);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        [Route("SendNotification")]
        public async Task<IActionResult> SendNotificationAsync([FromBody] AddNotificationDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.FindByIdAsync(dto.ToUserId);
            if(user == null)
                return NotFound(new ApiErrorResponse(404, "User not found"));

                var notification = dto.ToModel();
            notification.From = user.Id;
            
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            BackgroundJob.Enqueue<NotificationsManager>(x=>x.SendNotification(user.Email,dto.Type,dto.Message));
            return Ok();
        }
        [HttpDelete]
        [Route("DeleteNotification/{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Admin,User")]
        public async Task<IActionResult> DeleteNotificationAsync(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));

            var notification = await _unitOfWork.Notifications
                .GetEntityAsync(new NotificationSpecification(user.Id, id));
            
            if (notification == null)
                return NotFound(new ApiErrorResponse(404));

            _unitOfWork.Notifications.Delete(notification);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
