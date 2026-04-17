using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FWU.Nagarik.Api.Data.Constants;
using FWU.Nagarik.Api.ViewModels;

namespace FWU.Nagarik.Api.Pages.Account;

[Authorize(Roles = AppRoles.Admin)]
public class UsersModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) : PageModel
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public List<UserViewModel> Users { get; set; } = new();

    public async Task OnGetAsync()
    {
        var users = _userManager.Users.ToList();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            Users.Add(new UserViewModel
            {
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email,
                Designation = user.Designation,
                Roles = [.. roles]
            });
        }
    }

    public async Task OnPostAssignRoleAsync(string userName, List<string> roles)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) return;

        var allRoles = new[] { AppRoles.Admin, AppRoles.User };
        
        foreach (var role in allRoles)
        {
            if (roles.Contains(role))
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
            else
            {
                if (await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }
        }

        TempData["Success"] = "Roles assigned successfully.";
        await OnGetAsync();
    }
}
