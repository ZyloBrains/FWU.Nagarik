using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWU.Nagarik.Api.Models;

[Table("Institutions")]
public class Institution
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = "Far Western University";

    [MaxLength(200)]
    public string OfficeName { get; set; } = "Office of the Controller of Examinations";

    [MaxLength(200)]
    public string Location { get; set; } = "Mahendranagar, Kanchanpur, Nepal";

    [MaxLength(200)]
    public string? LogoPath { get; set; }

    [MaxLength(20)]
    public string DocumentTitle { get; set; } = "Academic Transcript";

    public int CurrentSerialNo { get; set; } = 7298;

    public bool IsActive { get; set; } = true;
}