using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FWU.Nagarik.Api.Services;
using FWU.Nagarik.Api.ViewModels;

namespace FWU.Nagarik.Api.Pages.Certificates;

public class TranscriptModel : PageModel
{
    private readonly IStudentService _studentService;

    public TranscriptModel(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public TranscriptViewModel? TranscriptData { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? RegdNo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? DobAD { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(RegdNo) || string.IsNullOrWhiteSpace(DobAD))
            return Page();

        var response = await _studentService.GetTranscriptAsync(RegdNo, DobAD);
        if (response?.Transcript == null)
            return Page();

        TranscriptData = response.Transcript;
        return Page();
    }
}
