using FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.ViewModels;

namespace FWU.Nagarik.Api.Mappers;

public static class TranscriptMapper
{
    public static SubjectViewModel ToViewModel(Subject subject)
    {
        return new SubjectViewModel
        {
            SubjectName = subject.SubjectName,
            SubjectCode = subject.SubjectCode,
            CreditHours = subject.CreditHours,
            Grade = subject.Grade,
            GradeValue = subject.GradeValue,
            GradePoint = subject.GradePoint,
            CourseType = subject.CourseType,
            SortOrder = subject.SortOrder
        };
    }

    public static SemesterViewModel ToViewModel(Semester semester)
    {
        var subjects = semester.Subjects.Select(ToViewModel).ToList();
        var totalCreditHours = subjects.Sum(s => s.CreditHours);
        var totalGradePoints = subjects.Sum(s => s.GradePoint);
        var gpa = totalCreditHours > 0 ? Math.Round(totalGradePoints / totalCreditHours, 2) : 0;

        return new SemesterViewModel
        {
            Name = semester.Name,
            SemesterNumber = semester.SemesterNumber,
            AcademicYear = semester.AcademicYear,
            SortOrder = semester.SortOrder,
            TotalCreditHours = totalCreditHours,
            TotalGradePoints = totalGradePoints,
            Gpa = gpa,
            TotalGradeValue = subjects.Sum(s => s.GradeValue),
            Subjects = subjects
        };
    }

    public static TranscriptViewModel ToViewModel(Transcript transcript, Student student)
    {
        var semesters = transcript.Semesters
            .OrderBy(s => s.SortOrder)
            .Select(ToViewModel)
            .ToList();

        var totalCreditHours = semesters.Sum(s => s.TotalCreditHours);
        var totalGradePoints = semesters.Sum(s => s.TotalGradePoints);
        var cgpa = totalCreditHours > 0 ? Math.Round(totalGradePoints / totalCreditHours, 2) : 0;

        var config = transcript.Institution;

        return new TranscriptViewModel
        {
            IssueSerialNo = transcript.IssueSerialNo,
            IssueDate = transcript.IssueDate.ToString("MMMM dd, yyyy"),
            UniversityName = config?.Name ?? "Far Western University",
            OfficeName = config?.OfficeName ?? "Office of the Controller of Examinations",
            UniversityLocation = config?.Location ?? "Mahendranagar, Kanchanpur, Nepal",
            LogoPath = config?.LogoPath,
            DocumentTitle = config?.DocumentTitle ?? "Academic Transcript",
            StudentName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Trim(),
            RegdNo = student.RegdNo,
            Faculty = student.Faculty,
            ProgramName = student.ProgramName,
            CourseDuration = student.CourseDuration,
            CampusName = student.CampusName,
            CampusLocation = student.CampusLocation,
            TotalCreditHours = totalCreditHours,
            TotalGradePoints = totalGradePoints,
            Cgpa = cgpa,
            TotalSubjects = semesters.Sum(s => s.Subjects.Count),
            Semesters = semesters
        };
    }
}