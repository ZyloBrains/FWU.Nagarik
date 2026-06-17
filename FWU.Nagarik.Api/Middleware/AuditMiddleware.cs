using System.Diagnostics;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Middleware;

public class AuditMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    private static readonly Dictionary<string, string> ActionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/api/student/verify"] = "StudentVerified",
        ["/api/student/transcript"] = "TranscriptRetrieved",
        ["/api/auth/token"] = "TokenGenerated"
    };

    private static readonly Dictionary<string, string> EntityTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["/api/student/verify"] = "Student",
        ["/api/student/transcript"] = "Student",
        ["/api/auth/token"] = "Token"
    };

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        var path = context.Request.Path.Value ?? string.Empty;

        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return;

        if (!ActionMap.TryGetValue(path, out var action))
            return;

        var auditService = context.RequestServices.GetRequiredService<IAuditService>();

        var responseCode = context.Response.StatusCode;
        var isSuccess = responseCode >= 200 && responseCode < 400;

        string? entityId = null;
        if (path.Equals("/api/student/verify", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/api/student/transcript", StringComparison.OrdinalIgnoreCase))
        {
            entityId = context.Request.Query["registration_number"].FirstOrDefault();
        }

        string? details = null;
        if (path.Equals("/api/auth/token", StringComparison.OrdinalIgnoreCase))
        {
            var apiKey = context.Request.Query["apiKey"].FirstOrDefault();
            if (!string.IsNullOrEmpty(apiKey))
            {
                details = $"{{\"apiKey\":\"{apiKey[..Math.Min(8, apiKey.Length)]}***\"}}";
            }
        }

        await auditService.LogAsync(
            context,
            action,
            EntityTypeMap.TryGetValue(path, out var entityType) ? entityType : "Api",
            entityId,
            isSuccess,
            responseCode,
            details);
    }
}
