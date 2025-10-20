using Microsoft.EntityFrameworkCore;
using QLVPP.Models;
using QLVPP.Security;

namespace QLVPP.Data
{
    public static class AppDataSeed
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            await context.Database.MigrateAsync();

            if (!context.Employees.Any())
            {
                var defaultPassword = "admin@123";
                var admin = new Employee
                {
                    Name = "System Administrator",
                    Email = "admin@qlvpp.local",
                    Phone = "0123456789",
                    Account = "admin",
                    Password = PasswordHasher.HashPassword(defaultPassword),
                    CreatedBy = "system",
                };

                context.Employees.Add(admin);
                await context.SaveChangesAsync();

                logger.LogWarning("============================================");
                logger.LogWarning("THE SYSTEM HAS JUST CREATED A DEFAULT ADMIN ACCOUNT");
                logger.LogWarning("Account : {Account}", admin.Account);
                logger.LogWarning("Password: {Password}", defaultPassword);
                logger.LogWarning("Please log in and change the password immediately!");
                logger.LogWarning("============================================");
            }
        }
    }
}
