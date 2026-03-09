using Microsoft.EntityFrameworkCore;
using QLVPP.Models;

namespace QLVPP.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(AppDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Metadata Manager" },
                    new Role { Name = "Warehouse Keeper" },
                    new Role { Name = "Warehouse Staff" },
                    new Role { Name = "Department Head" },
                    new Role { Name = "Regular User" },
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
        }
    }
}
