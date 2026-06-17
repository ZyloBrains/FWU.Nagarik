using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("AuditLogs")]
public class AuditLog
{
    [Key]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string EntityType { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? EntityId { get; set; }

    [MaxLength(50)]
    public string? ClientKeyId { get; set; }

    [MaxLength(100)]
    public string? ClientName { get; set; }

    [MaxLength(100)]
    public string? ClientOrg { get; set; }

    [MaxLength(45)]
    public string? ClientIp { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [Required]
    [MaxLength(10)]
    public string RequestMethod { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string RequestPath { get; set; } = string.Empty;

    public int ResponseCode { get; set; }

    public bool IsSuccess { get; set; }

    public string? Details { get; set; }

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }
}
