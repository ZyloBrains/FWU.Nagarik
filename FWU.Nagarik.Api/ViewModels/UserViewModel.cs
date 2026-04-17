namespace FWU.Nagarik.Api.ViewModels;
public class UserViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Designation { get; set; }
    public List<string> Roles { get; set; } = [];
}