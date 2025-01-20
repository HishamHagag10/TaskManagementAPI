using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using TaskManagement.API.Configuration;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<User?> FindByEmailAsync(string email)
        {

            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<bool> UserExistsInRoleByIdAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }
        public async Task<User?> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        public async Task<bool> IsUserWithEmailExistAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email);
        }
        public async Task<bool> IsUserWorkInProjectAsync(string email,int projectId)
        {
            return await _userManager.Users.AnyAsync(x => 
                                x.ProjectsWorkingAt.Any(y=>y.Id==projectId));
        }
        public async Task<int> CountAsync(Expression<Func<User, bool>>? e = null)
        {
            if (e == null)
            {
                return await _userManager.Users.CountAsync();
            }
            return await _userManager.Users.CountAsync(e);
        }
        public async Task<IEnumerable<User>> GetRecentUsersAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = _userManager.Users.
                OrderByDescending(x => x.CreatedAt);
            query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return await query.ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

    }
}
