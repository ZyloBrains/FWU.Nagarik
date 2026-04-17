using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.Data.Constants;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class LogsModel(AppDbContext db) : PageModel
{
    private readonly AppDbContext _db = db;

    public List<VerificationLog> Logs { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string SearchRegdNo { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? SearchStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync()
    {
        var pageSize = 20;
        var query = _db.VerificationLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchRegdNo))
        {
            query = query.Where(l => l.RegdNo.Contains(SearchRegdNo));
        }

        if (!string.IsNullOrWhiteSpace(SearchStatus))
        {
            query = query.Where(l => l.VerificationStatus == SearchStatus);
        }

        Logs = await query
            .OrderByDescending(l => l.VerifiedAt)
            .Skip((CurrentPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}