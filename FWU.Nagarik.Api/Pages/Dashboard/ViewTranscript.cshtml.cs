using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Data.Constants;
using Microsoft.EntityFrameworkCore;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class ViewTranscriptModel(AppDbContext dbContext) : PageModel
{
    private readonly AppDbContext _dbContext = dbContext;

    [BindProperty(SupportsGet = true)]
    public string? RegdNo { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(RegdNo))
        {
            TempData["Error"] = "Please enter a Registration Number.";
            return Page();
        }

        var student = await _dbContext.Students
            .FirstOrDefaultAsync(s => s.RegdNo == RegdNo.Trim());

        if (student == null)
        {
            TempData["Error"] = $"No student found with Registration Number '{RegdNo}'.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(student.DobAD))
        {
            TempData["Error"] = $"Student '{RegdNo}' has no Date of Birth on file. Transcript cannot be loaded.";
            return Page();
        }

        return Redirect($"/Certificates/Transcript?regdNo={student.RegdNo}&dobAD={student.DobAD}");
    }
}
