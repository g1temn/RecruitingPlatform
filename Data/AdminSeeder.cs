using Microsoft.AspNetCore.Identity;
using RecruitingPlatform.Entities;
using RecruitingPlatform.Enums;

namespace RecruitingPlatform.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();

        var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var adminRoleName = nameof(PossibleUserRole.Admin);

        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new UserRole { Name = adminRoleName });
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            var newAdmin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var createResult = await userManager.CreateAsync(newAdmin, adminPassword);

            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, adminRoleName);
            }
            else
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                Console.WriteLine($"Помилка створення адміна: {errors}");
            }
        }
    }
}