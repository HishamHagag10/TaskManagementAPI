using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TaskManagement.API.Configuration;
using TaskManagement.API.Helpers;
using TaskManagement.API.Repository.Specifications;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TaskManagement.API.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T item)
        {
            await _context.AddAsync(item);
            return item;
        }

        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            await _context.BulkInsertAsync(items);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().AsNoTracking().AnyAsync(criteria);
        }

        public T Delete(T item)
        {
            _context.Remove(item);
            return item;
        }

        public async Task DeleteRangeAsync(IEnumerable<T> items)
        {
            await _context.BulkDeleteAsync(items);
        }
        public T Update(T item)
        {
            _context.Update(item);
            return item;
        }
        public async Task UpdateRangeAsync(IEnumerable<T> items)
        {
            await _context.BulkUpdateAsync(items);
        }
        /*public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> criteria, Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null)
        {
            var query = _context.Set<T>().Where(criteria);
            includes?.Invoke(query);
            return await query.ToListAsync();
        }

        public Task<PagenatedResponse<T>> FindPagenatedAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<T?> FirstAsync(Expression<Func<T, bool>> criteria, Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null)
        {
            var query = _context.Set<T>().AsNoTracking();
            includes?.Invoke(query);
            return await query.FirstOrDefaultAsync(criteria);
        }

        public async Task<PagenatedResponse<T>> GetAll(int pageIndex,int pageSize, Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null)
        {
            var query = _context.Set<T>().AsNoTracking();
            var items = query.Skip((pageIndex-1)*pageSize)
                .Take(pageSize);
            includes?.Invoke(items);
            var count = query.Count();
            var response = new PagenatedResponse<T>
            {
                values = await items.ToListAsync(),
                PageIndex=pageIndex,
                PageSize=pageSize,
                TotalPages=(count+pageSize-1)/pageSize,
            };
            return response;
        }
        
        Task<ICollection<T>> IRepository<T>.FindAsync(Expression<Func<T, bool>> criteria, Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null)
        {
            throw new NotImplementedException();
        }
         */

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetEntityAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async Task<ICollection<T>> ListAsync()
        {
            return await _context.Set<T>().ToListAsync();  
        }

        public async Task<ICollection<T>> ListAsync(ISpecification<T> specification)
        {
            var query = ApplySpecification(specification);
            return await query.ToListAsync();
              
        }
        public async Task<int> CountAsync(ISpecification<T>? specification=null)
        {
            if(specification == null) 
                return await _context.Tasks.CountAsync();
            return await ApplySpecification(specification)
                .CountAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }
        

    }
}
