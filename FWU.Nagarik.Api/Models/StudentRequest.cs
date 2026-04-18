namespace FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.Enums;

public class StudentRequest
{
    public int Id { get; set; }
    public DocumentType DocumentType { get; set; } = DocumentType.Transcript;
    public DateTime RequestedDate { get; set; }
    public string Requestedby { get; set; } = "Self";

    public int StudentAdmissionId { get; set; }
    public Student? StudentAdmission { get; set; }
}