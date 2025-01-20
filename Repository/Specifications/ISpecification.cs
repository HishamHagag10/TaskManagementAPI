using System.Linq.Expressions;

namespace TaskManagement.API.Repository.Specifications
{
    public interface ISpecification<T>
    {
        List<Expression<Func<T, bool>>> Criteria { get; }
        //Expression<Func<T, bool>>? Criteria { get; }
        List<Expression<Func<T,object>>> Includes { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
