using TaskManagement.API.Helpers.Mapping;
using TaskManagement.API.Models;
using TaskManagement.API.Models.DTOs.DashboradDTOs;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;
using TaskManagement.API.Services.UserService;

namespace TaskManagement.API.Services.Dashboard_Services
{
    public partial class DashbaordService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public DashbaordService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<AdminDashboardResponseDto> GetAdminDashboardAsync(User user)
        {
            var usersSummary = await GetUsersSummary();
            var taskSummary = await GetTaskSummaryAsync();
            
            var projectSummary = await GetProjectSummaryAsync();

            var recentComments = await GetRecentCommentAsync();
            return new AdminDashboardResponseDto
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.FullName
                },
                UsersSummary = usersSummary,
                TaskSummary = taskSummary,
                ProjectSummary = projectSummary,
                RecentComments = recentComments,
            };
        }
        private async Task<UsersSummaryDto> GetUsersSummary()
        {
            var totalUsers = await _userService.CountAsync();
            var recentUsers = await _userService.GetRecentUsersAsync(1, 5);
            return new UsersSummaryDto
            {
                TotalUsers = totalUsers,
                RecentUsers = recentUsers.Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.FullName,
                    Email = x.Email
                })
            };
        }
        private async Task<ProjectSummaryDto> GetProjectSummaryAsync()
        {
            var projectSpecification = new ProjectsSpecifications();
            var totalProjects = await _unitOfWork.Projects.CountAsync(projectSpecification);
            projectSpecification.ApplyOrdering("updatedate", "desc");
            projectSpecification.AddPagenation(1, 10);
            var recentProjects = await _unitOfWork.Projects.ListAsync(projectSpecification);
            return new ProjectSummaryDto
            {
                TotalProjects = totalProjects,
                RecentProjects = recentProjects.Select(x => new ProjectDashboardDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                })
            };
        }
        public async Task<UserDashboardResponseDto> GetUserDashboardAsync(User user)
        {

            var summary = await GetTaskSummaryAsync(user.Email);   
            
            var upcomingTasks = (await _unitOfWork.Tasks
                .ListAsync(TaskSpecifications.GetUpComingTasksOfUserSpecification(user.Email)))
                .Select(x => x.ToSimplifiedDto());

            var recentTasks = await GetRecentTasksAsync(user.Email);
            
            var recentComments = await GetRecentCommentAsync(user.Email);
            return new UserDashboardResponseDto
            {
                User=new UserDto
                {
                   Id=user.Id,
                   Email=user.Email,
                   Name=user.FullName
                },
                TaskSummaryDto = summary,
                UpcomingTasks = upcomingTasks,
                RecentComments = recentComments
            };
        }
        private async Task<IEnumerable<CommentResponseDto>> GetRecentCommentAsync(string? userEmail=null)
        {
            var commentSpecification = new CommentSpecifications(userEmail, "updateddate", "desc");
            commentSpecification.AddPagenation(1, 5);
            var recentComments = await _unitOfWork.Comments.ListAsync(commentSpecification);
            return recentComments.Select(x=>x.ToDto());
        }
        private async Task<TaskSummaryDto> GetTaskSummaryAsync(string? userEmail=null)
        {
            var userTasksSpecification = TaskSpecifications.GetTaskOfUserSpecification(userEmail);
            var totalTasksCount = await _unitOfWork.Tasks.CountAsync(userTasksSpecification);

            var completedTasksCount = await _unitOfWork.Tasks.CountAsync(TaskSpecifications.GetCompletedTasksOfUserSpecification(userEmail));

            var tasksDueToday = await _unitOfWork.Tasks.CountAsync(TaskSpecifications.GetTasksDueDateTodayOfUserSpecification(userEmail));
            var recentTasks = await GetRecentTasksAsync(userEmail);
            return new TaskSummaryDto
            {
                TotalTasksCount=totalTasksCount,
                CompletedTasksCount=completedTasksCount,
                TasksDueToday=tasksDueToday,
                RecentTasks = recentTasks
            };
        }
        private async Task<IEnumerable<TaskSimplifiedDto>> GetRecentTasksAsync(string? userEmail=null)
        {
            var userTasksSpecification = TaskSpecifications.GetTaskOfUserSpecification(userEmail);
            userTasksSpecification.AddPagination(1, 5);
            userTasksSpecification.ApplyOrdering("desc", "updateddate");
            var recentTasks = await _unitOfWork.Tasks.ListAsync(userTasksSpecification);
            return recentTasks.Select(x=>x.ToSimplifiedDto());
        }

    }
}
