using TaskManagement.API.Models;
using TaskManagement.API.Models.DTOs.DashboradDTOs;
using static TaskManagement.API.Services.Dashboard_Services.DashbaordService;

namespace TaskManagement.API.Services.Dashboard_Services
{
    public interface IDashboardService
    {
        Task<UserDashboardResponseDto> GetUserDashboardAsync(User user);
        Task<AdminDashboardResponseDto> GetAdminDashboardAsync(User user);
    }
}
