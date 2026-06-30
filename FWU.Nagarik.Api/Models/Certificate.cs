using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("Certificates")]
public class Certificate
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string RegdNo { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ProgramName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string CertificateType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string BlobName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string BlobUrl { get; set; } = string.Empty;

    [MaxLength(100)]
    public string OriginalFileName { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    [MaxLength(100)]
    public string UploadedBy { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
