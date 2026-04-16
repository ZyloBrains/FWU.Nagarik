using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Models;

namespace FWU.Nagarik.Api.Pages.Dashboard;

[Authorize]
public class KeysModel : PageModel
{
    private readonly AppDbContext _db;

    public KeysModel(AppDbContext db)
    {
        _db = db;
    }

    public List<ApiKey> Keys { get; set; } = new();

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
        }
        await OnGetAsync();
    }

    public async Task OnPostDeleteAsync(int id)
    {
        var key = await _db.ApiKeys.FindAsync(id);
        if (key != null)
        {
            _db.ApiKeys.Remove(key);
            await _db.SaveChangesAsync();
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