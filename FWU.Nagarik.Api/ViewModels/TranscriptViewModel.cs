namespace FWU.Nagarik.Api.ViewModels;

public class TranscriptViewModel
{
    public int IssueSerialNo { get; set; }
    public string IssueDate { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public string OfficeName { get; set; } = string.Empty;
    public string UniversityLocation { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;
    public string RegdNo { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string CourseDuration { get; set; } = string.Empty;
    public string CampusName { get; set; } = string.Empty;
    public string CampusLocation { get; set; } = string.Empty;

    public double TotalCreditHours { get; set; }
    public double TotalGradePoints { get; set; }
    public double Cgpa { get; set; }
    public int TotalSubjects { get; set; }

    public List<SemesterViewModel> Semesters { get; set; } = new();
}