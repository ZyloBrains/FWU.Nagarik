using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("Subjects")]
public class Subject
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SemesterId { get; set; }

    [Required]
    [MaxLength(200)]
    public string SubjectName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string SubjectCode { get; set; } = string.Empty;

    [Column(TypeName = "decimal(5,2)")]
    public double CreditHours { get; set; }

    [MaxLength(10)]
    public string Grade { get; set; } = string.Empty;

    [Column(TypeName = "decimal(5,2)")]
    public double GradeValue { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public double GradePoint { get; set; }

    [MaxLength(10)]
    public string? CourseType { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey(nameof(SemesterId))]
    public virtual Semester? Semester { get; set; }
}