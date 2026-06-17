namespace FWU.Nagarik.Api.ViewModels;

public class SubjectViewModel
{
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public double CreditHours { get; set; }
    public string Grade { get; set; } = string.Empty;
    public double GradeValue { get; set; }
    public double GradePoint { get; set; }
    public string? CourseType { get; set; }
    public int SortOrder { get; set; }
}