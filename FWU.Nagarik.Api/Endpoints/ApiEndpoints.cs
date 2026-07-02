using Microsoft.AspNetCore.Authorization;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using FWU.Nagarik.Api.Authentication;
using FWU.Nagarik.Api.Services;
using FWU.Nagarik.Api.Pages.Certificates;

namespace FWU.Nagarik.Api.Endpoints;
public static class ApiEndpoints
{
    private static IBrowser? _browser;

    private static async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser == null || _browser.IsClosed)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        }
        return _browser;
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
                "/Pages/Certificates/Transcript.cshtml",
                new TranscriptModel(studentService) { TranscriptData = result.Transcript },
                httpContext);

            var cssPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "common.css");
            var cssContent = File.Exists(cssPath) ? await File.ReadAllTextAsync(cssPath) : string.Empty;

            var fullHtml = $"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8">
                    <style>{cssContent}</style>
                </head>
                <body>
                    {htmlContent}
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
    }
}
