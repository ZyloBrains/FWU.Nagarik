using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("StudentMarks")]
public class StudentMark
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string RegdNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string SubjectName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string SubjectCode { get; set; } = string.Empty;

    [Column(TypeName = "decimal(5,2)")]
    public double? Marks { get; set; }

    [MaxLength(10)]
    public string? Grade { get; set; }

    [MaxLength(50)]
    public string? Semester { get; set; }

    [MaxLength(50)]
    public string? AcademicYear { get; set; }
}