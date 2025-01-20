namespace TaskManagement.API.Repository.Specifications
{
    public class TagsWithIdsSpecifications 
        : BaseSpecification<Models.Tag>
    {
        public TagsWithIdsSpecifications(IEnumerable<int>ids)
        {
            AddCriteria(x => ids.Contains(x.Id));
        }
        public TagsWithIdsSpecifications(string? sortType="asc")
        {
            
            if (sortType == "desc")
                AddOrderByDescending(x => x.Name);
            else AddOrderBy(x => x.Name);
        }
        public void AddPagenation(int? pageIndex, int? pageSize)
        {
            if (pageIndex.HasValue && pageSize.HasValue)
                ApplyPagination(pageIndex.Value, pageSize.Value);
        }
    }
}
