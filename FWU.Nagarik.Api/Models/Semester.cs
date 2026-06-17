using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("Semesters")]
public class Semester
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TranscriptId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public int SemesterNumber { get; set; }

    [MaxLength(50)]
    public string? AcademicYear { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey(nameof(TranscriptId))]
    public virtual Transcript? Transcript { get; set; }

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}