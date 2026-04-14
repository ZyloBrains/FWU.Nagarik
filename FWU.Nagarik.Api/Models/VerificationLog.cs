using System.ComponentModel.DataAnnotations;

namespace FWU.Nagarik.Api.Models;

public class VerificationLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string RegdNo { get; set; } = string.Empty;

    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string MiddleName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string DobAD { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ProgramName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string IntakeYear { get; set; } = string.Empty;

    [MaxLength(20)]
    public string StudentStatus { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Level { get; set; }

    [MaxLength(100)]
    public string? School { get; set; }

    public double? CgpaScore { get; set; }

    [MaxLength(10)]
    public string? GraduateYear { get; set; }

    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    public string VerificationStatus { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }
}