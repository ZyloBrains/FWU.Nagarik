using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Data.Constants;
using FWU.Nagarik.Api.Enums;
using FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class StudentRequestsModel(AppDbContext db, IStudentRequestSyncService syncService) : PageModel
{
    private readonly AppDbContext _db = db;
    private readonly IStudentRequestSyncService _syncService = syncService;

    public List<StudentRequest> StudentRequests { get; set; } = [];
    public int TotalCount { get; set; }
    public bool IsSyncing { get; set; }
    public string? SyncMessage { get; set; }
    public DateTime? LastSyncTime { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string? DocumentType { get; set; }

    public const int PageSize = 50;

    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        var lastRequest = await _db.StudentRequests.OrderByDescending(r => r.RequestedDate).FirstOrDefaultAsync();
        LastSyncTime = lastRequest?.RequestedDate;

        var query = _db.StudentRequests.AsQueryable();

        if (!string.IsNullOrWhiteSpace(DocumentType))
        {
            if (Enum.TryParse<DocumentType>(DocumentType, out var docType))
            {
                query = query.Where(r => r.DocumentType == docType);
            }
        }

        TotalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        StudentRequests = await query
            .OrderByDescending(r => r.RequestedDate)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task OnPostSyncAsync()
    {
        IsSyncing = true;
        
        var result = await _syncService.SyncStudentRequestsAsync();
        
        SyncMessage = result.Message;
        
        await OnGetAsync();

        IsSyncing = false;
    }
}