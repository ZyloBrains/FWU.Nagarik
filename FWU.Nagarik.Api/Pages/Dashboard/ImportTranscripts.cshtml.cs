using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FWU.Nagarik.Api.Data.Constants;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
[RequestSizeLimit(50 * 1024 * 1024)] // 50 MB
public class ImportTranscriptsModel(ICsvTranscriptImportService importService, IAuditService auditService) : PageModel
{
    private readonly ICsvTranscriptImportService _importService = importService;
    private readonly IAuditService _auditService = auditService;

    [BindProperty]
    public IFormFile? CsvFile { get; set; }

    public ImportResult? Result { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (CsvFile == null || CsvFile.Length == 0)
        {
            TempData["Error"] = "Please select a CSV file to upload.";
            return Page();
        }

        if (!CsvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Only CSV files are allowed.";
            return Page();
        }

        using var stream = CsvFile.OpenReadStream();
        var uploadedBy = User.Identity?.Name ?? "admin";

        Result = await _importService.ImportAsync(stream, uploadedBy);

        await _auditService.LogAsync(HttpContext, "TranscriptsImported", "Transcript",
            CsvFile.FileName, true, 200,
            $"{{\"studentsCreated\":{Result.StudentsCreated},\"studentsUpdated\":{Result.StudentsUpdated},\"transcriptsCreated\":{Result.TranscriptsCreated},\"transcriptsUpdated\":{Result.TranscriptsUpdated},\"rowsSkipped\":{Result.RowsSkipped}}}");

        if (Result.Errors.Any())
            TempData["Warning"] = $"Import completed with {Result.Errors.Count} error(s) and {Result.RowsSkipped} row(s) skipped.";
        else
            TempData["Success"] = "Transcripts imported successfully.";

        return Page();
    }
}
