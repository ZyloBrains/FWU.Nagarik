namespace FWU.Nagarik.Api.ViewModels;

public class SemesterViewModel
{
    public string Name { get; set; } = string.Empty;
    public int SemesterNumber { get; set; }
    public string? AcademicYear { get; set; }
    public int SortOrder { get; set; }
    public double TotalCreditHours { get; set; }
    public double TotalGradePoints { get; set; }
    public double Gpa { get; set; }
    public double TotalGradeValue { get; set; }
    public List<SubjectViewModel> Subjects { get; set; } = new();
}