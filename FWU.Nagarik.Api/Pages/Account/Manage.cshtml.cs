using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FWU.Nagarik.Api.Pages.Account;

[Authorize]
public class ManageModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : PageModel
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;

    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Designation { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            FullName = user.FullName ?? string.Empty;
            Email = user.Email ?? string.Empty;
            Designation = user.Designation ?? string.Empty;
        }
    }

    public async Task OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        user.FullName = FullName;
        user.Designation = Designation;

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Profile updated successfully.";
        await OnGetAsync();
    }

    public async Task OnPostChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        if (newPassword != confirmPassword)
        {
            TempData["PasswordError"] = "New passwords do not match.";
            await OnGetAsync();
            return;
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            TempData["Success"] = "Password changed successfully.";
            await _signInManager.RefreshSignInAsync(user);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                TempData["PasswordError"] = error.Description;
            }
        }

        await OnGetAsync();
    }
}