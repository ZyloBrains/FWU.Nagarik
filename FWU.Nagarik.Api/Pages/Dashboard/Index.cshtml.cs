using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Data.Constants;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class IndexModel(AppDbContext db, UserManager<AppUser> userManager) : PageModel
{
    private readonly AppDbContext _db = db;
    private readonly UserManager<AppUser> _userManager = userManager;

    public int TotalStudents { get; set; }
    public int TotalVerifications { get; set; }
    public int TotalKeys { get; set; }
    public int TotalStudentRequests { get; set; }
    public int TotalAdmins { get; set; }

    public async Task OnGetAsync()
    {
        TotalStudents = await _db.Students.AsNoTracking().CountAsync();
        TotalVerifications = await _db.VerificationLogs.AsNoTracking().CountAsync();
        TotalKeys = await _db.ApiKeys.AsNoTracking().CountAsync();
        TotalStudentRequests = await _db.StudentRequests.AsNoTracking().CountAsync();
        TotalAdmins = (await _userManager.GetUsersInRoleAsync(AppRoles.Admin)).Count;
    }
}