using Microsoft.EntityFrameworkCore.Query;
using Microsoft.VisualBasic;
using System.Linq.Expressions;
using TaskManagement.API.Helpers;
using TaskManagement.API.Models.DTOs;

namespace TaskManagement.API.Repository.Specifications
{
    public class TasksSpecificationBuilder
        : BaseSpecification<Models.Task>
    {

        public TasksSpecificationBuilder()
        {
            AddIncludes(x => x.Tags);
        }
        public void IncludeAssignedUser()
        {
            AddIncludes(x => x.AssignedUser);
        }
        public void IncludeComments()
        {
            AddIncludes(x => x.Comments);
        }
        public void IncludeProject()
        {
            AddIncludes(x => x.Project);
        }
        public void AddPagination(int? pageIndex, int? pageSize)
        {
            if (pageSize.HasValue && pageIndex.HasValue)
                ApplyPagination(pageIndex.Value, pageSize.Value);
        }
        public void AddDeadlineFilter(DateTime date)
        {
            AddCriteria(x => x.DueDate.Date == date.Date);
        }
        public void AddDeadlineDateRangeFilter(DateTime startDate, DateTime endDate)
        {
            AddCriteria(x => x.DueDate >= startDate && x.DueDate <= endDate);
        }
        public void AddIdFilter(int id)
        {
            AddCriteria(x => x.Id == id);
        }
        public void AddProjectFilter(int? projectId)
        {
            if (projectId.HasValue)
                AddCriteria(x => x.ProjectId == projectId);
        }
        public void AddPriorityFilter(int? priority)
        {
            if (priority.HasValue)
                AddCriteria(x => (int)x.Priority == priority);
        }
        public void AddStatusFilter(int? status)
        {
            if (status.HasValue)
                AddCriteria(x => (int)x.Status == status);
        }
        public void AddUserFilter(string? UserEmail)
        {
            AddCriteria(x => x.AssignedUserEmail == UserEmail);
        }
        public void ApplyOrdering(string? sortBy, string? sortType)
        {
            if (string.IsNullOrEmpty(sortType)) sortType = "asc";
            if (string.IsNullOrEmpty(sortBy)) sortBy = "title";
            var ordering = (sortType.ToLower(), sortBy.ToLower());
            switch (ordering)
            {
                case ("asc", "createddate"):
                    AddOrderBy(x => x.CreatedAt);
                    break;
                case ("asc", "updateddate"):
                    AddOrderBy(x => x.UpdatedAt);
                    break;
                case ("asc", "duedate"):
                    AddOrderBy(x => x.DueDate);
                    break;

                case ("desc", "createddate"):
                    AddOrderByDescending(x => x.CreatedAt);
                    break;
                case ("desc", "updateddate"):
                    AddOrderByDescending(x => x.UpdatedAt);
                    break;
                case ("desc", "duedate"):
                    AddOrderByDescending(x => x.DueDate);
                    break;
                case ("desc", "title"):
                    AddOrderByDescending(x => x.Title);
                    break;
                default:
                    AddOrderBy(x => x.Title);
                    break;
            }
        }
        public ISpecification<Models.Task> Build()
        {
            return this;
        }
    }
}
