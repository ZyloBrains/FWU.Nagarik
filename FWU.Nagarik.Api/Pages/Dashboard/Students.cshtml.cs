using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Data.Constants;
using FWU.Nagarik.Api.Models;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class StudentsModel(AppDbContext db) : PageModel
{
    private readonly AppDbContext _db = db;

    public List<Student> Students { get; set; } = [];
    public List<string> Levels { get; set; } = [];
    public List<string> Programs { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string StudentName { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string IntakeYear { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string Level { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string ProgramName { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public const int PageSize = 100;

    public async Task OnGetAsync()
    {
        var query = _db.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(StudentName))
        {
            var name = StudentName.ToLower();
            query = query.Where(s => s.FirstName.ToLower().Contains(name) || 
                             (s.MiddleName != null && s.MiddleName.ToLower().Contains(name)) || 
                             s.LastName.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(IntakeYear))
        {
            query = query.Where(s => s.IntakeYear == IntakeYear);
        }

        if (!string.IsNullOrWhiteSpace(Level))
        {
            query = query.Where(s => s.Level == Level);
        }

        if (!string.IsNullOrWhiteSpace(ProgramName))
        {
            query = query.Where(s => s.ProgramName == ProgramName);
        }

        Levels = await _db.Students
            .Where(s => s.Level != null)
            .Select(s => s.Level!)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        Programs = await _db.Students
            .Where(s => s.ProgramName != null)
            .Select(s => s.ProgramName!)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        TotalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        Students = await query
            .OrderBy(s => s.FirstName)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

    public async Task OnPostUpdateAsync(string regdNo, string firstName, string middleName, string lastName, string dobAD, string level, string intakeYear, string programName, string school, double? cgpaScore, string graduateYear, string status)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.RegdNo == regdNo);
        if (student == null)
        {
            ModelState.AddModelError(string.Empty, "Student not found.");
            await OnGetAsync();
            return;
        }

        student.FirstName = firstName;
        student.MiddleName = middleName;
        student.LastName = lastName;
        student.DobAD = dobAD;
        student.Level = level;
        student.IntakeYear = intakeYear;
        student.ProgramName = programName;
        student.School = school;
        student.CgpaScore = cgpaScore;
        student.GraduateYear = graduateYear;
        student.StudentStatus = status;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Student updated successfully.";
        await OnGetAsync();
    }
}