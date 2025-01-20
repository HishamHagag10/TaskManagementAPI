using System.Linq.Expressions;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services.UserService
{
    public interface IUserService
    {
        Task<User?> FindByEmailAsync(string email);
        Task<bool> UserExistsInRoleByIdAsync(string userId, string role);
        Task<User?> FindByIdAsync(string id);
        Task<bool> IsUserWithEmailExistAsync(string email);
        Task<int> CountAsync(Expression<Func<User, bool>>? e = null);
        Task<IEnumerable<User>> GetRecentUsersAsync(int pageIndex = 1, int pageSize = 10);
        Task<bool> IsInRoleAsync(User user, string role);
        Task<bool> IsUserWorkInProjectAsync(string email, int projectId);
    }
}
