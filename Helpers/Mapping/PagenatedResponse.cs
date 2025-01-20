using TaskManagement.API.Models;

namespace TaskManagement.API.Helpers.Mapping
{
    public class PagenatedResponse<T>
    {
        public ICollection<T> values { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

}
