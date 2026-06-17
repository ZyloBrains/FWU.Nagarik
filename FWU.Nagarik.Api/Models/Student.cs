using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("Students")]
public class Student
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string RegdNo { get; set; } = string.Empty;

    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? MiddleName { get; set; }

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

    [Column(TypeName = "decimal(5,2)")]
    public double? CgpaScore { get; set; }

    [MaxLength(10)]
    public string? GraduateYear { get; set; }

    [MaxLength(50)]
    public string Faculty { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CampusName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CampusLocation { get; set; } = string.Empty;

    [MaxLength(50)]
    public string CourseDuration { get; set; } = string.Empty;
}