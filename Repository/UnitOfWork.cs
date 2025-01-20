using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;
using TaskManagement.API.Configuration;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            RefreshTokens = new BaseRepository<RefreshToken>(_context);
            Tags = new BaseRepository<Tag>(_context);
            Tasks = new BaseRepository<Models.Task>(_context);
            Projects = new BaseRepository<Project>(_context);
            Comments = new BaseRepository<Comment>(_context);
            TaskTags = new BaseRepository<TaskTag>(_context);
            Notifications = new BaseRepository<Notification>(_context);
        }

        public IRepository<RefreshToken> RefreshTokens { get;  }
        public IRepository<Models.Task> Tasks {  get; }
        public IRepository<Comment> Comments {  get; }
        public IRepository<Project> Projects {  get; }
        public IRepository<Tag> Tags {  get; }
        public IRepository<TaskTag> TaskTags { get; }

        public IRepository<Notification> Notifications { get; }

        public async System.Threading.Tasks.Task BeginTransactionAsync()
        {
            _transaction =await _context.Database.BeginTransactionAsync();
        }

        public async System.Threading.Tasks.Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                _transaction = null;
            }
        }
        public async System.Threading.Tasks.Task RollbackTransactionAsync()
        {
            if( _transaction != null )
            {
                await _transaction.RollbackAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
        }

        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        
    }
}
