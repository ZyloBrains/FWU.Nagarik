using System.Text.Json.Serialization;

namespace FWU.Nagarik.Api.Models;

public class StudentResponse
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("otherData")]
    public List<StudentData> OtherData { get; set; } = new();
}

public class StudentData
{
    [JsonPropertyName("regdNo")]
    public string RegdNo { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("middleName")]
    public string MiddleName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("dobAD")]
    public string DobAD { get; set; } = string.Empty;

    [JsonPropertyName("programName")]
    public string ProgramName { get; set; } = string.Empty;

    [JsonPropertyName("intakeYear")]
    public string IntakeYear { get; set; } = string.Empty;

    [JsonPropertyName("studentStatus")]
    public string StudentStatus { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public string? Level { get; set; }

    [JsonPropertyName("school")]
    public string? School { get; set; }

    [JsonPropertyName("cgpaScore")]
    public double? CgpaScore { get; set; }

    [JsonPropertyName("graduateYear")]
    public string? GraduateYear { get; set; }
}