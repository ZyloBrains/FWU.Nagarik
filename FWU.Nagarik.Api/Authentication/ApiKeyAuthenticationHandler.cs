using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FWU.Nagarik.Api.Data;

namespace FWU.Nagarik.Api.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";
}

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AppDbContext dbContext)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKeyValues))
            return AuthenticateResult.NoResult();

        var apiKey = apiKeyValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(apiKey))
            return AuthenticateResult.NoResult();

        var keyRecord = await dbContext.ApiKeys.FirstOrDefaultAsync(k => k.Key == apiKey && k.IsActive);

        if (keyRecord == null)
            return AuthenticateResult.Fail("Invalid API key.");

        if (keyRecord.ExpiresAt.HasValue && keyRecord.ExpiresAt < DateTime.UtcNow)
            return AuthenticateResult.Fail("API key has expired.");

        var claims = new[]
        {
            new Claim("keyId", keyRecord.Id.ToString()),
            new Claim("name", keyRecord.Name),
            new Claim("org", keyRecord.Organization ?? "")
        };

        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.DefaultScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.DefaultScheme);

        return AuthenticateResult.Success(ticket);
    }
}
