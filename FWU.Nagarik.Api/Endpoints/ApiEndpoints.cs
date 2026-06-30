using Microsoft.AspNetCore.Authorization;
using FWU.Nagarik.Api.Authentication;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Endpoints;
public static class ApiEndpoints
{
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

        app.MapGet("/api/student/transcript", [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)] async (string registration_number, string dobAD, IStudentService studentService) =>
        {
            if (string.IsNullOrWhiteSpace(registration_number))
                return Results.BadRequest(new { message = "registration_number is required" });

            if (string.IsNullOrWhiteSpace(dobAD))
                return Results.BadRequest(new { message = "dobAD is required" });

            var result = await studentService.GetTranscriptAsync(registration_number, dobAD);

            if (result == null)
                return Results.NotFound(new { message = "No record found for the given registration number / DOB" });

            return Results.Ok(result);
        })
        .WithName("GetTranscript")
        .WithSummary("Retrieves student transcript")
        .WithDescription("Retrieves the transcript for a student based on registration number and date of birth.");

        app.MapGet("/api/student/certificate", [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)] async (string registration_number, string program_name, string certificate_type, ICertificateService certificateService, IAzureBlobStorageService blobStorageService) =>
        {
            if (string.IsNullOrWhiteSpace(registration_number))
                return Results.BadRequest(new { message = "registration_number is required" });

            if (string.IsNullOrWhiteSpace(program_name))
                return Results.BadRequest(new { message = "program_name is required" });

            if (string.IsNullOrWhiteSpace(certificate_type))
                return Results.BadRequest(new { message = "certificate_type is required" });

            var allowedTypes = new[] { "Transcript", "Migration", "Provisional" };
            if (!allowedTypes.Contains(certificate_type, StringComparer.OrdinalIgnoreCase))
                return Results.BadRequest(new { message = "certificate_type must be one of: Transcript, Migration, Provisional" });

            var certificate = await certificateService.GetCertificateAsync(
                registration_number.Trim(),
                program_name.Trim(),
                certificate_type);

            if (certificate == null)
                return Results.NotFound(new { message = "No certificate found for the given registration number, program name, and certificate type." });

            var stream = await blobStorageService.DownloadAsync(certificate.BlobName);
            if (stream == null)
                return Results.NotFound(new { message = "Certificate file not found in storage." });

            return Results.File(stream, "application/pdf", certificate.BlobName);
        })
        .WithName("GetCertificate")
        .WithSummary("Downloads a student certificate")
        .WithDescription("Retrieves a PDF certificate (Transcript, Migration, or Provisional) based on registration number, program name, and certificate type.");
    }
}
