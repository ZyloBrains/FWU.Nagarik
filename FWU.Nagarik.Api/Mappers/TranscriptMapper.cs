using FWU.Nagarik.Api.Models;
using FWU.Nagarik.Api.ViewModels;

namespace FWU.Nagarik.Api.Mappers;

public static class TranscriptMapper
{
    private const string UniversityName = "Far Western University";
    private const string OfficeName = "Office of the Controller of Examinations";
    private const string UniversityLocation = "Mahendranagar, Kanchanpur, Nepal";
    private const string DocumentTitle = "Academic Transcript";

    public static TranscriptViewModel ToViewModel(List<Transcript> transcripts, Student student)
    {
        if (transcripts.Count == 0)
            return new TranscriptViewModel();

        var first = transcripts.First();

        var semesterGroups = transcripts
            .GroupBy(t => t.SemesterNumber)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var subjects = g.OrderBy(t => t.SortOrder).Select(t => new SubjectViewModel
                {
                    SubjectName = t.SubjectName,
                    SubjectCode = t.SubjectCode,
                    CreditHours = t.CreditHours,
                    Grade = t.Grade,
                    GradeValue = t.GradeValue,
                    GradePoint = t.GradePoint,
                    CourseType = t.CourseType,
                    SortOrder = t.SortOrder
                }).ToList();

                var totalCreditHours = subjects.Sum(s => s.CreditHours);
                var totalGradePoints = subjects.Sum(s => s.GradePoint);
                var gpa = totalCreditHours > 0 ? Math.Round(totalGradePoints / totalCreditHours, 2) : 0;

                return new SemesterViewModel
                {
                    Name = g.First().SemesterName,
                    SemesterNumber = g.Key,
                    AcademicYear = g.First().AcademicYearName,
                    ExamRollNo = g.First().ExamRollNo,
                    SortOrder = g.Key,
                    TotalCreditHours = totalCreditHours,
                    TotalGradePoints = totalGradePoints,
                    Gpa = gpa,
                    TotalGradeValue = subjects.Sum(s => s.GradeValue),
                    Subjects = subjects
                };
            })
            .ToList();

        var totalAllCreditHours = semesterGroups.Sum(s => s.TotalCreditHours);
        var totalAllGradePoints = semesterGroups.Sum(s => s.TotalGradePoints);
        var cgpa = first.CGPA ?? 0;

        return new TranscriptViewModel
        {
            IssueSerialNo = first.IssueSerialNo,
            IssueDate = first.IssueDate.ToString("MMMM dd, yyyy"),
            UniversityName = UniversityName,
            OfficeName = OfficeName,
            UniversityLocation = UniversityLocation,
            LogoPath = null,
            DocumentTitle = DocumentTitle,
            StudentName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Trim(),
            RegdNo = student.RegdNo,
            Faculty = student.Faculty,
            ProgramName = student.ProgramName,
            CourseDuration = student.CourseDuration,
            CampusName = student.CampusName,
            CampusLocation = student.CampusLocation,
            TotalCreditHours = totalAllCreditHours,
            TotalGradePoints = totalAllGradePoints,
            Cgpa = cgpa,
            TotalSubjects = semesterGroups.Sum(s => s.Subjects.Count),
            Semesters = semesterGroups
        };
    }
}
