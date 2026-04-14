using System.Text.Json.Serialization;

namespace FWU.Nagarik.Api.Models;

public class TranscriptResponse
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("otherData")]
    public List<TranscriptData> OtherData { get; set; } = new();
}

public class TranscriptData
{
    [JsonPropertyName("regdNo")]
    public string RegdNo { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string MiddleName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("programName")]
    public string ProgramName { get; set; } = string.Empty;

    [JsonPropertyName("intakeYear")]
    public string IntakeYear { get; set; } = string.Empty;

    [JsonPropertyName("marks")]
    public List<SubjectMark> Marks { get; set; } = new();
}

public class SubjectMark
{
    [JsonPropertyName("subjectName")]
    public string SubjectName { get; set; } = string.Empty;

    [JsonPropertyName("subjectCode")]
    public string? SubjectCode { get; set; }

    [JsonPropertyName("marks")]
    public double? Marks { get; set; }

    [JsonPropertyName("grade")]
    public string? Grade { get; set; }

    [JsonPropertyName("semester")]
    public string? Semester { get; set; }

    [JsonPropertyName("academicYear")]
    public string? AcademicYear { get; set; }
}