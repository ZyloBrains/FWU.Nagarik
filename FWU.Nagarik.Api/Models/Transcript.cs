using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("Transcripts")]
public class Transcript
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string RegdNo { get; set; } = string.Empty;

    public int IssueSerialNo { get; set; }

    public DateTime IssueDate { get; set; }

    public bool IsPrinted { get; set; }

    [MaxLength(200)]
    public string StudentName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ProgramName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string FacultyName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string CollegeName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string AcademicYearName { get; set; } = string.Empty;

    public int SemesterNumber { get; set; }

    [MaxLength(50)]
    public string SemesterName { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Year { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Part { get; set; } = string.Empty;

    [MaxLength(50)]
    public string ExamRollNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string SubjectCode { get; set; } = string.Empty;

    [MaxLength(200)]
    public string SubjectName { get; set; } = string.Empty;

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

    [Column(TypeName = "decimal(5,2)")]
    public double? CGPA { get; set; }

    public int SortOrder { get; set; }
}
