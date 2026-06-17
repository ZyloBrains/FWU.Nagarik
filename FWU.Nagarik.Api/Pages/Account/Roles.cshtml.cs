using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FWU.Nagarik.Api.Data.Constants;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Pages.Account;

[Authorize(Roles = AppRoles.Admin)]
public class RolesModel(RoleManager<IdentityRole> roleManager, IAuditService auditService) : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IAuditService _auditService = auditService;

    public List<IdentityRole> Roles { get; set; } = new();

    public void OnGet()
    {
        Roles = _roleManager.Roles.ToList();
    }

    public async Task OnPostCreateAsync(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            ModelState.AddModelError(string.Empty, "Role name is required.");
            OnGet();
            return;
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (result.Succeeded)
        {
            await _auditService.LogAsync(HttpContext, "RoleCreated", "Role",
                roleName, true, 200);
            TempData["Success"] = $"Role '{roleName}' created successfully.";
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        OnGet();
    }

    public async Task OnPostDeleteAsync(string name)
    {
        var role = await _roleManager.FindByNameAsync(name);
        if (role != null)
        {
            await _roleManager.DeleteAsync(role);

            await _auditService.LogAsync(HttpContext, "RoleDeleted", "Role",
                name, true, 200);

            TempData["Success"] = $"Role '{name}' deleted successfully.";
        }

        OnGet();
    }
}