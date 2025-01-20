using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Services.UserService;
using TaskManagementAPI.Erorrs;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using TaskManagement.API.Services.Dashboard_Services;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Policy 
    public class DashboardController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService, IUserService userService)
        {
            _dashboardService = dashboardService;
            _userService = userService;
        }

        [HttpGet]
        [Route("UserDashBoard")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = "User")]
        public async Task<IActionResult> UserDashBoard()
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));

            var response = await _dashboardService.GetUserDashboardAsync(user);
            return Ok(response);
        }
        private async Task<User?> GetCurrentUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(role))
                return null;

            var user = await _userService.FindByIdAsync(userId);
            if(user == null || !(await _userService.IsInRoleAsync(user,role))) 
                return null;
            return user;
        }
        [HttpGet]
        [Route("AdminDashBoard")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401));

            var response = await _dashboardService.GetAdminDashboardAsync(user);
            return Ok(response);
        }

    }
}
