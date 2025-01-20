using Microsoft.AspNetCore.Identity;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<RefreshToken> RefreshTokens { get;  }
        IRepository<Models.Task> Tasks { get;  }
        IRepository<Comment>Comments  { get; }
        IRepository<Project> Projects { get;  }
        IRepository<Tag> Tags { get; }
        IRepository<TaskTag> TaskTags { get; }
        IRepository<Notification> Notifications { get; }
        Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task BeginTransactionAsync();
        System.Threading.Tasks.Task CommitTransactionAsync();
        System.Threading.Tasks.Task RollbackTransactionAsync();

    }
}
