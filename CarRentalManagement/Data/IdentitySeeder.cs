using Microsoft.AspNetCore.Identity;
namespace CarRentalManagement.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, string adminPassword)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Create roles if they do not exist

        string[] roles = ["Administrator", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                EnsureSuccess(result);
            }
        }

        // 2. Create the administrator if it does not exist

        const string adminEmail = "admin@localhost.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            EnsureSuccess(result);
        }

        // 3. Assign the administrator role

        if (!await userManager.IsInRoleAsync(admin, "Administrator"))
        {
            var result = await userManager.AddToRoleAsync(admin, "Administrator");
            EnsureSuccess(result);
        }

    }

    private static void EnsureSuccess(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}

