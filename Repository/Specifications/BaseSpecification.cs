using System.Linq.Expressions;

namespace TaskManagement.API.Repository.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public List<Expression<Func<T, bool>>> Criteria { get; } = new List<Expression<Func<T, bool>>>(); 

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>>? OrderBy { get; private set; }

        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled {  get; private set; }


        public BaseSpecification()
        {

        }
        /*public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }*/
        protected void AddCriteria(Expression<Func<T,bool>>criteria)
        {
            Criteria.Add(criteria);
        }
        protected void AddIncludes(Expression<Func<T, object>> include)
        {
            Includes.Add(include);
        }
        protected void AddOrderBy(Expression<Func<T, object>> orderby)
        {
            OrderBy = orderby;
        }
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDesc)
        {
            OrderByDescending = orderByDesc;
        }

        protected void ApplyPagination(int pageIndex,int pageSize)
        {
            pageIndex = Math.Max(pageIndex, 1);
            pageSize = Math.Max(pageSize, 1);
            pageSize = Math.Min(pageSize, 100);
            Skip = (pageIndex-1)*pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
        }

    }
}
