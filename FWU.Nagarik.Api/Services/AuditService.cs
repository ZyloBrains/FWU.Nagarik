using System.Security.Claims;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Models;

namespace FWU.Nagarik.Api.Services;

public interface IAuditService
{
    Task LogAsync(HttpContext httpContext, string action, string entityType,
                  string? entityId, bool isSuccess, int responseCode = 200,
                  string? details = null, string? errorMessage = null);
}

public class AuditService(AppDbContext dbContext) : IAuditService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task LogAsync(HttpContext httpContext, string action, string entityType,
                               string? entityId, bool isSuccess, int responseCode = 200,
                               string? details = null, string? errorMessage = null)
    {
        var log = new AuditLog
        {
            Timestamp = DateTime.UtcNow,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            ClientKeyId = httpContext.User.FindFirst("keyId")?.Value,
            ClientName = httpContext.User.FindFirst("name")?.Value
                         ?? httpContext.User.Identity?.Name,
            ClientOrg = httpContext.User.FindFirst("org")?.Value,
            ClientIp = httpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            RequestMethod = httpContext.Request.Method,
            RequestPath = httpContext.Request.Path,
            ResponseCode = responseCode,
            IsSuccess = isSuccess,
            Details = details,
            ErrorMessage = errorMessage
        };

        _dbContext.AuditLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }
}
