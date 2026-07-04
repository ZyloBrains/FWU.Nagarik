using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using FWU.Nagarik.Api.Authentication;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Mappers;
using FWU.Nagarik.Api.Services;
using FWU.Nagarik.Api.Data.Constants;

namespace FWU.Nagarik.Api.Endpoints;
public static class ApiEndpoints
{
    private static IBrowser? _browser;

    internal static void CloseBrowser()
    {
        if (_browser != null && !_browser.IsClosed)
        {
            _browser.Dispose();
            _browser = null;
        }
    }

    private static async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser == null || _browser.IsClosed)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage"]
            });
        }
        return _browser;
    }

    private static string GetLogoBase64()
    {
        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "fwu.png");
        if (!File.Exists(logoPath))
            return string.Empty;

        var bytes = File.ReadAllBytes(logoPath);
        var base64 = Convert.ToBase64String(bytes);
        return $"data:image/png;base64,{base64}";
    }

    public static void Map(WebApplication app)
    {
        app.MapGet("/api/student/verify", [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)] async (string registration_number, string dobAD, IStudentService studentService) =>
        {
            if (string.IsNullOrWhiteSpace(registration_number))
                return Results.BadRequest(new { message = "registration_number is required" });

            if (string.IsNullOrWhiteSpace(dobAD))
                return Results.BadRequest(new { message = "dobAD is required" });

            var result = await studentService.VerifyStudentAsync(registration_number, dobAD);

            if (result == null)
                return Results.NotFound(new { message = "No record found for the given registration number / DOB" });

            return Results.Ok(result);
        })
        .WithName("VerifyStudent")
        .WithSummary("Verifies student details")
        .WithDescription("Verifies student details based on registration number and date of birth.");

        app.MapGet("/api/student/transcript", [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)] async (
            string registration_number,
            string dobAD,
            IStudentService studentService,
            IRazorViewRenderer viewRenderer,
            HttpContext httpContext) =>
        {
            if (string.IsNullOrWhiteSpace(registration_number))
                return Results.BadRequest(new { message = "registration_number is required" });

            if (string.IsNullOrWhiteSpace(dobAD))
                return Results.BadRequest(new { message = "dobAD is required" });

            var result = await studentService.GetTranscriptAsync(registration_number, dobAD);

            if (result?.Transcript == null)
                return Results.NotFound(new { message = "No record found for the given registration number / DOB" });

            var htmlContent = await viewRenderer.RenderViewToStringAsync(
                "/Pages/Certificates/_TranscriptContent.cshtml",
                result.Transcript,
                httpContext);

            var cssPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "common.css");
            var cssContent = File.Exists(cssPath) ? await File.ReadAllTextAsync(cssPath) : string.Empty;

            var logoBase64 = GetLogoBase64();
            htmlContent = htmlContent.Replace("/images/fwu.png", logoBase64);

            var fullHtml = $"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8">
                    <style>{cssContent}</style>
                </head>
                <body>
                    <div class="certificate">
                        <div class="border-outer"></div>
                        <div class="border-inner"></div>
                        {htmlContent}
                    </div>
                </body>
                </html>
                """;

            var browser = await GetBrowserAsync();
            using var page = await browser.NewPageAsync();
            await page.SetContentAsync(fullHtml);
            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Width = "210mm",
                Height = "297mm",
                MarginOptions = new MarginOptions
                {
                    Top = "0.4in",
                    Right = "0.4in",
                    Bottom = "0.4in",
                    Left = "0.4in"
                },
                PrintBackground = true
            });

            return Results.File(pdfBytes, "application/pdf", $"{registration_number}_Transcript.pdf");
        })
        .WithName("GetTranscript")
        .WithSummary("Retrieves student transcript as PDF")
        .WithDescription("Retrieves the transcript for a student as a PDF document based on registration number and date of birth.");

        app.MapGet("/api/admin/transcript/html", [Authorize(Roles = AppRoles.Admin)] async (
            string regdNo,
            AppDbContext db,
            IRazorViewRenderer viewRenderer,
            HttpContext httpContext) =>
        {
            if (string.IsNullOrWhiteSpace(regdNo))
                return Results.BadRequest(new { message = "regdNo is required" });

            var student = await db.Students.FirstOrDefaultAsync(s => s.RegdNo == regdNo);
            if (student == null)
                return Results.NotFound(new { message = "Student not found" });

            var transcripts = await db.Transcripts
                .Where(t => t.RegdNo == regdNo)
                .OrderBy(t => t.SemesterNumber)
                .ThenBy(t => t.SortOrder)
                .ToListAsync();

            if (transcripts.Count == 0)
                return Results.NotFound(new { message = "No transcript data found" });

            var viewModel = TranscriptMapper.ToViewModel(transcripts, student);

            var htmlContent = await viewRenderer.RenderViewToStringAsync(
                "/Pages/Certificates/_TranscriptContent.cshtml",
                viewModel,
                httpContext);

            var cssPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "common.css");
            var cssContent = File.Exists(cssPath) ? await File.ReadAllTextAsync(cssPath) : string.Empty;

            var wrappedHtml = $"""
                <style>{cssContent}</style>
                <div class="certificate">
                    <div class="border-outer"></div>
                    <div class="border-inner"></div>
                    {htmlContent}
                </div>
                """;

            return Results.Content(wrappedHtml, "text/html");
        })
        .WithName("GetTranscriptHtml")
        .WithSummary("Retrieves student transcript as HTML (Admin use Only)")
        .WithDescription("Retrieves the transcript for a student as styled HTML for admin preview.");
    }
}
