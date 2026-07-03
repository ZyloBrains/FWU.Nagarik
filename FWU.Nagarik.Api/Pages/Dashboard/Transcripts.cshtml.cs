using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Data.Constants;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class TranscriptsModel(AppDbContext db) : PageModel
{
    private readonly AppDbContext _db = db;

    public List<TranscriptEntry> TranscriptEntries { get; set; } = [];
    public List<string> Programs { get; set; } = [];
    public List<string> Faculties { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchRegdNo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ProgramName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Faculty { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public const int PageSize = 100;

    public async Task OnGetAsync()
    {
        var query = _db.Transcripts
            .AsNoTracking()
            .GroupBy(t => new { t.RegdNo, t.IssueSerialNo })
            .Select(g => new TranscriptEntry
            {
                RegdNo = g.Key.RegdNo,
                IssueSerialNo = g.Key.IssueSerialNo,
                StudentName = g.Max(t => t.StudentName),
                ProgramName = g.Max(t => t.ProgramName),
                FacultyName = g.Max(t => t.FacultyName),
                CollegeName = g.Max(t => t.CollegeName),
                CGPA = g.Max(t => t.CGPA),
                SubjectCount = g.Count(),
                SemesterCount = g.Select(t => t.SemesterNumber).Distinct().Count(),
                IssueDate = g.Min(t => t.IssueDate)
            })
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchRegdNo))
        {
            var regdNo = SearchRegdNo.ToLower();
            query = query.Where(t => t.RegdNo.ToLower().Contains(regdNo));
        }

        if (!string.IsNullOrWhiteSpace(SearchName))
        {
            var name = SearchName.ToLower();
            query = query.Where(t => t.StudentName.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(ProgramName))
        {
            query = query.Where(t => t.ProgramName == ProgramName);
        }

        if (!string.IsNullOrWhiteSpace(Faculty))
        {
            query = query.Where(t => t.FacultyName == Faculty);
        }

        Programs = await _db.Transcripts
            .AsNoTracking()
            .Select(t => t.ProgramName)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync();

        Faculties = await _db.Transcripts
            .AsNoTracking()
            .Select(t => t.FacultyName)
            .Distinct()
            .OrderBy(f => f)
            .ToListAsync();

        TotalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        TranscriptEntries = await query
            .OrderBy(t => t.RegdNo)
            .ThenBy(t => t.IssueSerialNo)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public class TranscriptEntry
    {
        public string RegdNo { get; set; } = string.Empty;
        public int IssueSerialNo { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public double? CGPA { get; set; }
        public int SubjectCount { get; set; }
        public int SemesterCount { get; set; }
        public DateTime IssueDate { get; set; }
    }
}
