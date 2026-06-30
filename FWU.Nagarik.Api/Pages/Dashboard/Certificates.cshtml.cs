using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Data.Constants;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class CertificatesModel(AppDbContext db, ICertificateService certificateService, IAuditService auditService) : PageModel
{
    private readonly AppDbContext _db = db;
    private readonly ICertificateService _certificateService = certificateService;
    private readonly IAuditService _auditService = auditService;

    public List<Models.Certificate> Certificates { get; set; } = [];
    public List<string> CertificateTypes { get; set; } = ["Transcript", "Migration", "Provisional"];

    [BindProperty(SupportsGet = true)]
    public string? SearchRegdNo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FilterCertificateType { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public const int PageSize = 20;

    public async Task OnGetAsync()
    {
        TotalCount = await _certificateService.GetCertificateCountAsync(SearchRegdNo, FilterCertificateType);
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        Certificates = await _certificateService.GetCertificatesAsync(SearchRegdNo, FilterCertificateType, CurrentPage, PageSize);
    }

    public async Task<IActionResult> OnPostUploadAsync(IFormFile file, string regdNo, string programName, string certificateType)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a file to upload.";
            await OnGetAsync();
            return Page();
        }

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Only PDF files are allowed.";
            await OnGetAsync();
            return Page();
        }

        if (string.IsNullOrWhiteSpace(regdNo) || string.IsNullOrWhiteSpace(programName) || string.IsNullOrWhiteSpace(certificateType))
        {
            TempData["Error"] = "Registration number, program name, and certificate type are required.";
            await OnGetAsync();
            return Page();
        }

        using var stream = file.OpenReadStream();
        var uploadedBy = User.Identity?.Name ?? "admin";

        await _certificateService.UploadCertificateAsync(
            regdNo.Trim(), programName.Trim(), certificateType,
            stream, file.FileName, file.Length, uploadedBy);

        await _auditService.LogAsync(HttpContext, "CertificateUploaded", "Certificate",
            regdNo, true, 200,
            $"{{\"regdNo\":\"{regdNo}\",\"programName\":\"{programName}\",\"certificateType\":\"{certificateType}\"}}");

        TempData["Success"] = $"Certificate uploaded successfully for {regdNo}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostBulkUploadAsync(IFormFileCollection files, string bulkCertificateType)
    {
        if (files == null || files.Count == 0)
        {
            TempData["Error"] = "Please select files to upload.";
            await OnGetAsync();
            return Page();
        }

        if (string.IsNullOrWhiteSpace(bulkCertificateType))
        {
            TempData["Error"] = "Certificate type is required for bulk upload.";
            await OnGetAsync();
            return Page();
        }

        var uploadedBy = User.Identity?.Name ?? "admin";
        var fileDataList = new List<(System.IO.Stream Stream, string FileName, long FileSize)>();
        var fileStreams = new List<System.IO.Stream>();

        try
        {
            foreach (var file in files)
            {
                if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    continue;

                var memoryStream = new System.IO.MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                fileStreams.Add(memoryStream);
                fileDataList.Add((memoryStream, file.FileName, file.Length));
            }

            var uploaded = await _certificateService.BulkUploadAsync(bulkCertificateType, fileDataList, uploadedBy);

            await _auditService.LogAsync(HttpContext, "BulkCertificatesUploaded", "Certificate",
                bulkCertificateType, true, 200,
                $"{{\"certificateType\":\"{bulkCertificateType}\",\"count\":{uploaded.Count}}}");

            TempData["Success"] = $"{uploaded.Count} certificate(s) uploaded successfully.";
        }
        finally
        {
            foreach (var stream in fileStreams)
                await stream.DisposeAsync();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _certificateService.DeleteCertificateAsync(id);

        if (result)
        {
            await _auditService.LogAsync(HttpContext, "CertificateDeleted", "Certificate",
                id.ToString(), true, 200, "{}");
            TempData["Success"] = "Certificate deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Certificate not found.";
        }

        return RedirectToPage();
    }
}
