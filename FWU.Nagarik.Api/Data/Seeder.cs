using Microsoft.AspNetCore.Identity;

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
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (result.Succeeded)
            {
                // check if the "Admin" role exists, if not create it
                var roleExists = await roleManager.RoleExistsAsync("Admin");
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}