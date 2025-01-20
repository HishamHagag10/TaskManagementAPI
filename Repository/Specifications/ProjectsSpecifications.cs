using System.Net.NetworkInformation;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repository.Specifications
{
    public class ProjectsSpecifications
    :BaseSpecification<Project>
    {
        public ProjectsSpecifications()
        {
        }
        public ProjectsSpecifications(int id)
        {
            AddCriteria(x => x.Id == id);
            AddIncludes(x => x.WorkingUsers.Take(5)
                 .OrderByDescending(x=>x.UpdatedAt));
            AddIncludes(x => x.Tasks.Take(5).OrderByDescending(x => x.UpdatedAt));
        }
        public ProjectsSpecifications(string userEmail)
        {
            AddCriteria(x => x.WorkingUsers.Any(u => u.Email == userEmail));
        }
        public ProjectsSpecifications(int? status, string? managerEmail)
        {
            if (status.HasValue)
                AddCriteria(x => (int)x.Status == status);
            if(!string.IsNullOrEmpty(managerEmail))
                AddCriteria(x => x.ProjectManagerEmail == managerEmail);
            
        }
        public void AddPagenation(int? pageIndex=1, int? pageSize=10)
        {
            if (!pageIndex.HasValue)
                pageIndex = 1;
            if(!pageSize.HasValue)
                pageSize = 10;
           ApplyPagination(pageIndex.Value, pageSize.Value);
        }
        public void ApplyOrdering(string? sortBy, string? sortType)
        {
            if (string.IsNullOrEmpty(sortType)) sortType = "asc";
            if (string.IsNullOrEmpty(sortBy)) sortBy = "name";
            switch ((sortType.ToLower(), sortBy.ToLower()))
            {
                case ("asc", "createddate"):
                    AddOrderBy(x => x.CreatedAt);
                    break;
                case ("asc", "updateddate"):
                    AddOrderBy(x => x.UpdatedAt);
                    break;
                case ("asc", "startdate"):
                    AddOrderBy(x => x.Start_date);
                    break;
                case ("asc", "enddate"):
                    AddOrderBy(x => x.Start_date);
                    break;

                case ("desc", "createddate"):
                    AddOrderByDescending(x => x.CreatedAt);
                    break;
                case ("desc", "updateddate"):
                    AddOrderByDescending(x => x.UpdatedAt);
                    break;
                case ("desc", "startdate"):
                    AddOrderByDescending(x => x.Start_date);
                    break;
                case ("desc", "enddate"):
                    AddOrderByDescending(x => x.Start_date);
                    break;
                case ("desc", "name"):
                    AddOrderByDescending(x => x.Name);
                    break;
                default:
                    AddOrderBy(x => x.Name);
                    break;
            }
        }

    }
}
