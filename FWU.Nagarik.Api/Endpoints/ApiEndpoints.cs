using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Endpoints;
public static class ApiEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/api/auth/token", async (string apiKey, AppDbContext db, IConfiguration config) =>
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return Results.BadRequest(new { message = "apiKey is required" });

            var keyRecord = await db.ApiKeys.FirstOrDefaultAsync(k => k.Key == apiKey && k.IsActive);

            if (keyRecord == null)
                return Results.Unauthorized();

            if (keyRecord.ExpiresAt.HasValue && keyRecord.ExpiresAt < DateTime.UtcNow)
                return Results.Unauthorized();

            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "YourSuperSecretKeyHere12345!");
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim("name", keyRecord.Name),
                new System.Security.Claims.Claim("org", keyRecord.Organization ?? ""),
                new System.Security.Claims.Claim("keyId", keyRecord.Id.ToString())
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: config["Jwt:Issuer"] ?? "FWU.Nagarik.Api",
                audience: config["Jwt:Audience"] ?? "FWU.Nagarik.Api",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            return Results.Ok(new { token = tokenHandler.WriteToken(token) });
        })
        .WithName("GetToken")
        .WithSummary("Generates JWT token for API key authentication")
        .WithDescription("Generates a JWT token for valid API key authentication.");

        app.MapGet("/api/student/verify", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string registration_number, string dobAD, IStudentService studentService) =>
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

        app.MapGet("/api/student/transcript", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] async (string registration_number, string dobAD, IStudentService studentService) =>
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
    }
}