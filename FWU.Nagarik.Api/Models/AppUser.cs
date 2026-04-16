using Microsoft.AspNetCore.Identity;

public class AppUser: IdentityUser
{
    public string? FullName { get; set; }
    public string? Designation { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? Remarks { get; set; }
}