using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Data.Constants;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class IndexModel(AppDbContext db) : PageModel
{
    private readonly AppDbContext _db = db;

    public int TotalStudents { get; set; }
    public int TotalVerifications { get; set; }
    public int TotalKeys { get; set; }

    public async Task OnGetAsync()
    {
        TotalStudents = await _db.Students.AsNoTracking().CountAsync();
        TotalVerifications = await _db.VerificationLogs.AsNoTracking().CountAsync();
        TotalKeys = await _db.ApiKeys.AsNoTracking().CountAsync();
    }
}