using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TaskManagement.API.Helpers;
using TaskManagement.API.Models;
using TaskManagement.API.Repository.Specifications;

namespace TaskManagement.API.Repository
{
    public interface IRepository<T> where T : class
    {
        //Task<PagenatedResponse<T>> GetAll(int pageIndex=1, int pageSize=10, Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null);
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetEntityAsync(ISpecification<T>specification);
        Task<ICollection<T>> ListAsync();
        Task<ICollection<T>> ListAsync(ISpecification<T> specification);
        //Task<IEnumerable<>> ListAsync(ISpecification<T> specification);
        Task<int> CountAsync(ISpecification<T>? specification=null);
        Task<T> AddAsync(T item);
        System.Threading.Tasks.Task AddRangeAsync(IEnumerable<T> item);
        T Update(T item);
        T Delete(T item);
        System.Threading.Tasks.Task DeleteRangeAsync(IEnumerable<T> items);
        System.Threading.Tasks.Task UpdateRangeAsync(IEnumerable<T> items);
        Task<bool> AnyAsync(Expression<Func<T, bool>> criteria);
        

    }
}
