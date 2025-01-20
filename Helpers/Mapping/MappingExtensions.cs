using Microsoft.IdentityModel.Tokens;
using TaskManagement.API.Helpers.Enums;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagement.API.Models.DTOs.Response;
using TaskManagement.API.Models.DTOs.UpdateRequest;
using TaskManagement.API.Models;
using TaskManagement.API.Models.DTOs.DashboradDTOs;

namespace TaskManagement.API.Helpers.Mapping;
public static class MappingExtensions
{
    public static TaskResponseDto ToDto(this Models.Task task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority.ToString(),
            Status = task.Status.ToString(),
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            AssignedUserEmail = task.AssignedUserEmail,
            Tags = task.Tags.Select(t => t.Name).ToList(),
            
        };
    }
    public static TaskSimplifiedDto ToSimplifiedDto(this Models.Task task)
    {
        return new TaskSimplifiedDto
        {
            Id = task.Id,
            Title = task.Title,
            Status = task.Status.ToString(),
            UpdatedAt = task.UpdatedAt,
            DueDate = task.DueDate,
        };
    }
    public static TaskWithDetailsDto ToDetailsDto(this Models.Task task)
    {
        return new TaskWithDetailsDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority.ToString(),
            Status = task.Status.ToString(),
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            AssignedUserEmail = task.AssignedUserEmail,
            AssignedUserName = task.AssignedUser?.FullName,
            Project = task.Project.Name,
            Tags = task.Tags.Select(t => t.Name).ToList(),
            Comments = task.Comments.Select(x=>x.ToDto()).ToList()
        };
    }
    public static Models.Task ToModel(this AddTaskDto task)
    {
        return new Models.Task
        {
            Title = task.Title,
            Description = task.Description,
            Priority = (Priority)task.Priority,
            Status = (Enums.TaskStatus)task.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            AssignedUserEmail = task.AssignedUserEmail,
        };
    }
    public static Models.Task ToModel(this UpdateTaskDto updatedtask, Models.Task task)
    {
        if (!string.IsNullOrEmpty(updatedtask.Title))
            task.Title = updatedtask.Title;

        if (!string.IsNullOrEmpty(updatedtask.Description))
            updatedtask.Description = task.Description;

        if (updatedtask.Priority != null)
            task.Priority = (Priority)updatedtask.Priority;

        if (updatedtask.Status != null)
            task.Status = (Enums.TaskStatus)updatedtask.Status;

        if (updatedtask.DueDate != null)
            task.DueDate = updatedtask.DueDate.Value;

        if (updatedtask.ProjectId != null)
            task.ProjectId = (int)updatedtask.ProjectId;

        if (!string.IsNullOrEmpty(updatedtask.AssignedUserEmail))
            task.AssignedUserEmail = updatedtask.AssignedUserEmail;

        task.UpdatedAt = DateTime.UtcNow;
        return task;
    }

    public static ProjectResponseDto ToDto(this Project project)
    {
        return new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Start_date = project.Start_date,
            ProjectManagerEmail = project.ProjectManagerEmail,
            End_date = project.End_date,
            Status = project.Status.ToString(),
        };
    }
    public static ProjectWithDetailsDto ToDetailsDto(this Project project)
    {
        return new ProjectWithDetailsDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Start_date = project.Start_date,
            ProjectManagerEmail = project.ProjectManagerEmail,
            End_date = project.End_date,
            Status = project.Status.ToString(),
            WorkingUsers = project.WorkingUsers.Select(x => x.ToDto()),
            Tasks = project.Tasks.Select(x=>x.ToSimplifiedDto())
        };
    }
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id=user.Id,
            Name=user.FullName,
            Email=user.Email,
        };
    }
    public static Project ToModel(this AddProjectDto dto)
    {
        return new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            Start_date = dto.Start_date,
            ProjectManagerEmail = dto.ProjectManagerEmail,
            End_date = dto.End_date,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = (ProjectStatus)dto.Status,
        };
    }
    public static Project ToModel(this UpdateProjectDto dto, Project project)
    {
        if (dto == null) return project;

        if (!string.IsNullOrEmpty(dto.Name))
            project.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Description))
            project.Description = dto.Description!;

        if (dto.Start_date.HasValue)
            project.Start_date = dto.Start_date.Value;

        if (dto.Status.HasValue)
            project.Status = (ProjectStatus)dto.Status.Value;

        if (dto.End_date.HasValue)
            project.End_date = dto.End_date.Value;

        if (!string.IsNullOrEmpty(dto.ProjectManagerEmail))
            project.ProjectManagerEmail = dto.ProjectManagerEmail;

        project.UpdatedAt = DateTime.UtcNow;

        return project;
    }
    public static Comment ToModel(this AddCommentDto dto, string userEmail)
    {
        return new Comment
        {
            TaskId = dto.TaskId,
            Content = dto.Content,
            UserEmail = userEmail,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }
    public static CommentResponseDto ToDto(this Comment comment)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            TaskId = comment.TaskId,
            UserEmail = comment.UserEmail,
        };
    }
    public static NotificationResponseDto ToDto(this Notification notification)
    {
        return new NotificationResponseDto
        {
            Message = notification.Message,
            Type = notification.Type,
            From = notification.From,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
            IsRead = notification.IsRead,
        };
    }
    public static Notification ToModel(this AddNotificationDto dto)
    {
        return new Notification
        {
            Message = dto.Message,
            Type = dto.Type,
            ToUserId = dto.ToUserId,
            CreatedAt = DateTime.UtcNow,
            ReadAt = DateTime.UtcNow,
            IsRead = false,
        };
    }
    public static PagenatedResponse<TaskResponseDto> ToResponse(
        this IEnumerable<Models.Task> item
        , int? pageIndex, int? pageSize, int count)
    {
        var res = new PagenatedResponse<TaskResponseDto>
        {
            values = item.Select(x => x.ToDto()).ToList(),
            PageIndex = pageIndex ?? 1,
            PageSize = pageSize ?? count,
            TotalPages = pageSize.HasValue ? (count + pageSize.Value - 1) / pageSize.Value : 1,
        };
        return res;
    }
    public static PagenatedResponse<CommentResponseDto> ToResponse(
        this IEnumerable<Comment> item
        , int? pageIndex, int? pageSize, int count)
    {
        var res = new PagenatedResponse<CommentResponseDto>
        {
            values = item.Select(x => x.ToDto()).ToList(),
            PageIndex = pageIndex ?? 1,
            PageSize = pageSize ?? count,
            TotalPages = pageSize.HasValue ? (count + pageSize.Value - 1) / pageSize.Value : 1,
        };
        return res;
    }
    public static PagenatedResponse<ProjectResponseDto> ToResponse(
            this IEnumerable<Project> item
            , int? pageIndex, int? pageSize, int count)
    {
        var res = new PagenatedResponse<ProjectResponseDto>
        {
            values = item.Select(x => x.ToDto()).ToList(),
            PageIndex = pageIndex ?? 1,
            PageSize = pageSize ?? count,
            TotalPages = pageSize.HasValue ? (count + pageSize.Value - 1) / pageSize.Value : 1,
        };
        return res;
    }
    public static PagenatedResponse<TagResponseDto> ToResponse(
            this IEnumerable<Tag> item
            , int? pageIndex, int? pageSize, int count)
    {
        var res = new PagenatedResponse<TagResponseDto>
        {
            values = item.Select(x => new TagResponseDto { Id = x.Id, Name = x.Name }).ToList(),
            PageIndex = pageIndex ?? 1,
            PageSize = pageSize ?? count,
            TotalPages = pageSize.HasValue ? (count + pageSize.Value - 1) / pageSize.Value : 1,
        };
        return res;
    }

    public static PagenatedResponse<NotificationResponseDto> ToResponse(
            this IEnumerable<Notification> item
            , int? pageIndex, int? pageSize, int count)
    {
        var res = new PagenatedResponse<NotificationResponseDto>
        {
            values = item.Select(x => x.ToDto()).ToList(),
            PageIndex = pageIndex ?? 1,
            PageSize = pageSize ?? count,
            TotalPages = pageSize.HasValue ? (count + pageSize.Value - 1) / pageSize.Value : 1,
        };
        return res;
    }
    /*public static PagenatedResponse<T2> ToResponse<T, T2>(this IEnumerable<T> item
        , int? pageIndex, int? pageSize, int count)
    {
        var res = new PagenatedResponse<T2>();
        if (typeof(T2) == typeof(CommentResponseDto))
            res.values = item.OfType<Comment>()
                .Select(x => (T2)(object)x.ToDto()).ToList();

        if (typeof(T2) == typeof(ProjectResponseDto))
            res.values = item.OfType<Project>()
                .Select(x => (T2)(object)x.ToDto()).ToList();

        if (typeof(T2) == typeof(TaskResponseDto))
            res.values = item.OfType<Models.Task>()
                .Select(x => (T2)(object)x.ToDto()).ToList();

        res.PageIndex = pageIndex ?? 1;
        res.PageSize = pageSize ?? count;
        res.TotalPages = pageSize.HasValue ? (count + pageSize.Value - 1) / pageSize.Value : 1;
        return res;
    }*/
}
