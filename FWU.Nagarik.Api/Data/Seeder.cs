using Microsoft.AspNetCore.Identity;
using FWU.Nagarik.Api.Data.Constants;

public class Seeder
{
    public static async Task SeedAdminUser(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var adminEmail = "admin@fwu.edu.np";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (result.Succeeded)
            {
                var roleExists = await roleManager.RoleExistsAsync(AppRoles.Admin);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                }
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }
    }
}