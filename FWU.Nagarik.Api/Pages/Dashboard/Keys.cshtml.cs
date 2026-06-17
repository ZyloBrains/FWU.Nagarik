using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.Data.Constants;
using FWU.Nagarik.Api.Services;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize(Roles = AppRoles.Admin)]
public class KeysModel(AppDbContext db, IAuditService auditService) : PageModel
{
    private readonly AppDbContext _db = db;
    private readonly IAuditService _auditService = auditService;

    public List<ApiKey> Keys { get; set; } = [];

    public async Task OnGetAsync()
    {
        Keys = await _db.ApiKeys.OrderByDescending(k => k.CreatedAt).ToListAsync();
    }

    public async Task OnPostCreateAsync()
    {
        var name = Request.Form["Name"];
        var org = Request.Form["Organization"];
        var expires = Request.Form["ExpiresAt"];
        var desc = Request.Form["Description"];

        var key = new ApiKey
        {
            Name = name,
            Organization = org,
            Key = GenerateKey(),
            Description = desc,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.TryParse(expires, out var dt) ? dt : (DateTime?)null
        };

        _db.ApiKeys.Add(key);
        await _db.SaveChangesAsync();

        await _auditService.LogAsync(HttpContext, "ApiKeyCreated", "ApiKey",
            key.Id.ToString(), true, 200,
            $"{{\"name\":\"{name}\",\"organization\":\"{org}\"}}");

        TempData["Success"] = $"API Key created: {key.Key}";
        await OnGetAsync();
    }

    public async Task OnPostToggleAsync(int id)
    {
        var key = await _db.ApiKeys.FindAsync(id);
        if (key != null)
        {
            key.IsActive = !key.IsActive;
            await _db.SaveChangesAsync();

            await _auditService.LogAsync(HttpContext, "ApiKeyToggled", "ApiKey",
                id.ToString(), true, 200,
                $"{{\"name\":\"{key.Name}\",\"isActive\":{key.IsActive.ToString().ToLower()}}}");
        }
        await OnGetAsync();
    }

    public async Task OnPostDeleteAsync(int id)
    {
        var key = await _db.ApiKeys.FindAsync(id);
        if (key != null)
        {
            var keyName = key.Name;
            _db.ApiKeys.Remove(key);
            await _db.SaveChangesAsync();

            await _auditService.LogAsync(HttpContext, "ApiKeyDeleted", "ApiKey",
                id.ToString(), true, 200,
                $"{{\"name\":\"{keyName}\"}}");
        }
        await OnGetAsync();
    }

    private string GenerateKey()
    {
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").Replace("=", "")[..32];
    }
}