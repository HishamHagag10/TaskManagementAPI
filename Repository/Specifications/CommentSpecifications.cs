using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repository.Specifications
{
    public class CommentSpecifications
        :BaseSpecification<Comment>
    {
        public CommentSpecifications()
        {

        }
        public CommentSpecifications(int? taskId, string? userEmail, string? sortBy = "updateddate", string? sortType = "desc")
        {
            AddCriteria(x=> (!taskId.HasValue || x.TaskId==taskId ) &&
                 (string.IsNullOrEmpty(userEmail) || x.UserEmail==userEmail));
            ApplyOrdering(sortBy,sortType);
        }
        public CommentSpecifications(string? userEmail, string? sortBy = "updateddate", string? sortType = "desc")
        {
            if (!string.IsNullOrEmpty(userEmail))
                AddCriteria(x => x.UserEmail == userEmail);
            ApplyOrdering(sortBy, sortType);
        }


        public void AddPagenation(int? pageIndex, int? pageSize)
        {
            if (pageIndex.HasValue && pageSize.HasValue)
                ApplyPagination(pageIndex.Value, pageSize.Value);
        }
        public void ApplyOrdering(string? sortBy, string? sortType)
        {
            if (string.IsNullOrEmpty(sortType)) sortType = "desc";
            if (string.IsNullOrEmpty(sortBy)) sortBy = "updateddate";
            var ordering = (sortType.ToLower(), sortBy.ToLower());
            switch (ordering)
            {
                case ("asc", "createddate"):
                    AddOrderBy(x => x.CreatedAt);
                    break;
                case ("asc", "updateddate"):
                    AddOrderBy(x => x.UpdatedAt);
                    break;

                case ("desc", "createddate"):
                    AddOrderByDescending(x => x.CreatedAt);
                    break;
                default:
                    AddOrderByDescending(x => x.UpdatedAt);
                    break;
            }
        }
    }
}
