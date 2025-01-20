using Microsoft.EntityFrameworkCore;
namespace TaskManagement.API.Repository.Specifications
{
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T>inputQuery,
            ISpecification<T>spec)
        {
            var query= inputQuery.AsQueryable();
             
            query=spec.Criteria.Aggregate(query,(current,criteria)
                    =>current.Where(criteria));
            
            if(spec.OrderBy != null)
                query=query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
            query = spec.Includes.Aggregate(query,(current, include) 
                => current.Include(include));

            return query;
        }
    }
}
