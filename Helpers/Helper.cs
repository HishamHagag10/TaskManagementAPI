using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Helpers.Enums;
using TaskManagement.API.Models;

namespace TaskManagementAPI.Helpers
{
    public static class Helper
    {
        public const int MaxPriority = (int)Priority.high;
        public const int MaxProjectStatus = (int)ProjectStatus.Completed;
        public const int MaxTaskStatus = (int)TaskManagement.API.Helpers.Enums.TaskStatus.Completed;

        public static async System.Threading.Tasks.Task SeedAsync(IServiceProvider serviceProvider)
        {
            await SeedRoleAsync(serviceProvider.GetRequiredService<RoleManager<IdentityRole>>());
            await SeedUsersAsync(serviceProvider.GetRequiredService<UserManager<User>>());
        }
        private static async System.Threading.Tasks.Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { Role.Admin, Role.User };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        private static async System.Threading.Tasks.Task SeedUsersAsync(UserManager<User> userManager)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }
            var user = new User
            {
                Email = "hishamhagag18@gmail.com",
                UserName = "hishamhagag18@gmail.com",
                PhoneNumber = "01011111",
                FullName = "Hisham Hagag",
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed=true,
            };
            var is_created=await userManager.CreateAsync (user,"Admin@1");
            var is_roleAdded = await userManager.AddToRoleAsync(user, Role.Admin);
        }
    }
}
