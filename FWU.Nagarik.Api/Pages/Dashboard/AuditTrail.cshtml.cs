using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.Data.Constants;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class AuditTrailModel(AppDbContext db) : PageModel
{
    private readonly AppDbContext _db = db;

    public List<AuditLog> Logs { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchAction { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchClient { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        var pageSize = 50;
        var query = _db.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchAction))
        {
            query = query.Where(l => l.Action == SearchAction);
        }

        if (!string.IsNullOrWhiteSpace(SearchClient))
        {
            var search = SearchClient.ToLower();
            query = query.Where(l =>
                (l.ClientName != null && l.ClientName.ToLower().Contains(search)) ||
                (l.ClientOrg != null && l.ClientOrg.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(SearchStatus))
        {
            var isSuccess = SearchStatus == "Success";
            query = query.Where(l => l.IsSuccess == isSuccess);
        }

        var totalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        Logs = await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((CurrentPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
