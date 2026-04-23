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

    public int? InstitutionId { get; set; }

    [ForeignKey(nameof(RegdNo))]
    public virtual Student? Student { get; set; }

    [ForeignKey(nameof(InstitutionId))]
    public virtual Institution? Institution { get; set; }

    public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();
}