using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public int TotalStudents { get; set; }
    public int TotalVerifications { get; set; }
    public int TotalKeys { get; set; }

    public async Task OnGetAsync()
    {
        TotalStudents = await _db.Students.CountAsync();
        TotalVerifications = await _db.VerificationLogs.CountAsync();
        TotalKeys = await _db.ApiKeys.CountAsync();
    }
}